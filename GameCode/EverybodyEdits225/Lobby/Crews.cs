using System;
using System.Linq;
using System.Text.RegularExpressions;
using EverybodyEdits.Game.Crews;
using EverybodyEdits.Game.Mail;
using EverybodyEdits.Lobby.Invitations;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby
{
    public class Crews
    {
        private readonly Client client;

        public Crews(Client client)
        {
            this.client = client;
        }

        private void LoadCrew(string crewId, Callback<Crew> callback)
        {
            var crew = new Crew(client);
            crew.Load(crewId, () => callback(crew));
        }

        public void HandleMessage(LobbyPlayer player, Message m)
        {
            var rtn = Message.Create(m.Type);
            switch (m.Type)
            {
                case "getCrew":
                {
                    this.LoadCrew(Regex.Replace(m.GetString(0), @"\s+", "").ToLower(),
                        crew => crew.SendGetMessage(player));
                    break;
                }
                case "getMyCrews":
                {
                    this.GetMyCrews(rtn, player);
                    break;
                }
                case "createCrew":
                {
                    this.CreateCrew(rtn, player, m.GetString(0).Trim());
                    break;
                }

                case "getCrewInvites":
                {
                    InvitationHelper.GetInvitationsTo(this.client.BigDB, InvitationType.Crew,
                        player.Name, invites =>
                        {
                            var invitesIds =
                                invites.Where(it => it.Status == InvitationStatus.Pending)
                                    .Select(it => it.Sender)
                                    .ToArray();
                            if (invitesIds.Length > 0)
                            {
                                this.client.BigDB.LoadKeys("Crews", invitesIds, crews =>
                                {
                                    foreach (var crew in crews.Where(crew => crew != null))
                                    {
                                        rtn.Add(crew.Key);
                                        rtn.Add(crew.GetString("Name"));
                                        rtn.Add(crew.GetString("LogoWorld", ""));
                                    }

                                    player.Send(rtn);
                                });
                            }
                            else
                            {
                                player.Send(rtn);
                            }
                        });
                    break;
                }

                case "blockCrewInvites":
                {
                    var crewId = m.GetString(0).ToLower();
                    var shouldBlock = m.GetBoolean(1);

                    InvitationBlocking.BlockCrew(this.client.BigDB, player.ConnectUserId, crewId, shouldBlock, () =>
                    {
                        if (shouldBlock)
                        {
                            InvitationHelper.GetInvitation(this.client.BigDB, InvitationType.Crew, crewId, player.Name,
                                invitation =>
                                {
                                    if (invitation.Exists)
                                    {
                                        invitation.Status = InvitationStatus.Rejected;
                                        invitation.Save(() => player.Send(m.Type, true));
                                    }
                                    else
                                    {
                                        player.Send(m.Type, true);
                                    }
                                });
                        }
                        else
                        {
                            player.Send(m.Type, true);
                        }
                    });
                    break;
                }

                case "blockAllCrewInvites":
                {
                    var shouldBlock = m.GetBoolean(0);

                    InvitationBlocking.BlockAllCrews(this.client.BigDB, player.ConnectUserId, shouldBlock);
                    player.Send(m.Type, shouldBlock);
                    break;
                }
            }
        }

        private void GetMyCrews(Message rtn, BasePlayer player)
        {
            this.client.BigDB.LoadRange("Crews", "ByCreator", new object[] {player.ConnectUserId}, null, null, 100,
                ownedCrews =>
                {
                    player.PayVault.Refresh(() =>
                    {
                        var canCreateNewCrew = player.PayVault.Count("crew") >
                                               ownedCrews.Count(it => !it.GetBool("Disbanded", false));

                        rtn.Add(canCreateNewCrew);
                        this.client.BigDB.Load("CrewMembership", player.ConnectUserId, membership =>
                        {
                            if (membership != null)
                            {
                                foreach (var crew in membership.Properties)
                                {
                                    rtn.Add(crew);
                                    rtn.Add(membership.GetString(crew));
                                }
                            }
                            player.Send(rtn);
                        });
                    });
                });
        }

        private void CreateCrew(Message rtn, LobbyPlayer player, string crewName)
        {
            player.PayVault.Refresh(() =>
            {
                this.client.BigDB.LoadRange("Crews", "ByCreator", new object[] {player.ConnectUserId}, null, null, 100,
                    ownedCrews =>
                    {
                        var canCreateNewCrew = player.PayVault.Count("crew") >
                                               ownedCrews.Count(it => !it.GetBool("Disbanded", false));
                        if (!canCreateNewCrew)
                        {
                            this.SendErrorReply(rtn, player, "Cannot create crew. First buy crew item in shop.");
                            return;
                        }

                        // Replace repeating spaces with one space
                        crewName = new Regex(@"[ ]{2,}").Replace(crewName, " ");

                        var test = new Regex("^[A-Za-z0-9 ]*$");
                        var crewId = Regex.Replace(crewName, @"\s+", "").ToLower();
                        if (!test.IsMatch(crewName))
                        {
                            this.SendErrorReply(rtn, player,
                                "Selected name contains invalid charaters. Valid charaters are 0-9, A-Z, a-z and space.");
                        }
                        else if (crewName.Length > 25)
                        {
                            this.SendErrorReply(rtn, player, "Crew name cannot be more than 25 characters long.");
                        }
                        else if (crewName.Length < 2)
                        {
                            this.SendErrorReply(rtn, player, "Crew name must be at least 2 characters long.");
                        }
                        else if (BadWords.ContainsBadWord(crewName) || BadWords.ContainsBadWord(crewId))
                        {
                            this.SendErrorReply(rtn, player, "Crew name contains inappropriate words.");
                        }
                        else
                        {
                            Console.WriteLine("Id for \"{0}\" crew = {1}", crewName, crewId);

                            var dbo = new DatabaseObject()
                                .Set("Creator", player.ConnectUserId)
                                .Set("Name", crewName)
                                .Set("Subscribers", (uint) 1)
                                .Set("Ranks", new DatabaseArray()
                                    .Add(new DatabaseObject()
                                        .Set("Name", "Owner"))
                                    .Add(new DatabaseObject()
                                        .Set("Name", "Member")
                                        .Set("Powers", "0")))
                                .Set("Members", new DatabaseObject()
                                    .Set(player.ConnectUserId, new DatabaseObject()
                                        .Set("Rank", 0)));

                            this.client.BigDB.CreateObject("Crews", crewId, dbo, newDbo =>
                            {
                                rtn.Add(true);
                                rtn.Add(newDbo.Key);

                                NotificationHelper.AddSubscription(this.client.BigDB, player.ConnectUserId,
                                    "crew" + crewId);

                                this.client.BigDB.LoadOrCreate("CrewMembership", player.ConnectUserId, membership =>
                                {
                                    membership.Set(crewId, crewName);
                                    membership.Save(() => player.PlayerObject.Save(() => player.Send(rtn)));
                                });
                            }, error =>
                            {
                                // TODO: Improve error messages ?
                                this.SendErrorReply(rtn, player, error.Message);
                            });
                        }
                    });
            });
        }

        private void SendErrorReply(Message rtn, LobbyPlayer player, string error)
        {
            rtn.Add(false);
            rtn.Add(error);
            player.Send(rtn);
        }
    }
}