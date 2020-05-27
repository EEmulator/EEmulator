using System.Collections.Generic;
using System.Linq;
using EverybodyEdits.Common;
using EverybodyEdits.Game.Crews;
using EverybodyEdits.Game.Mail;
using EverybodyEdits.Lobby.Invitations;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Crews
{
    [RoomType("CrewLobby" + Config.VersionString)]
    public class CrewLobby : Game<CommonPlayer>, IUpgradeRoom<CommonPlayer>
    {
        private readonly List<QueueItem> queue = new List<QueueItem>();
        private Crew crew;
        private string crewId;
        private bool initialized;
        private bool isInvalid;
        private CrewShop shop;

        public UpgradeChecker<CommonPlayer> UpgradeChecker { get; private set; }

        public override void GameStarted()
        {
            this.PreloadPayVaults = true;
            this.PreloadPlayerObjects = true;

            this.crew = new Crew(this.PlayerIO);

            if (!this.RoomId.StartsWith("crew"))
            {
                this.SetAsInvalid();
                return;
            }

            this.crewId = this.RoomId.Substring(4);

            this.crew.Load(this.crewId, () =>
            {
                if (this.crew.DatabaseObject == null)
                {
                    this.SetAsInvalid();
                }
                else
                {
                    shop = new CrewShop(this.PlayerIO, this.crew, () =>
                    {
                        this.initialized = true;
                        this.ProcessQueue();
                    });
                }
            });

            this.UpgradeChecker = new UpgradeChecker<CommonPlayer>(this);
        }

        private void SetAsInvalid()
        {
            this.isInvalid = true;
            foreach (var player in this.Players)
            {
                player.Disconnect();
            }
        }

        public override void GameClosed()
        {
            this.crew.Save();
        }

        public override bool AllowUserJoin(CommonPlayer player)
        {
            if (player.PlayerObject.Contains("linkedTo"))
            {
                player.Send("linked");
                return false;
            }
            foreach (var p in this.Players.Where(pl => pl.ConnectUserId == player.ConnectUserId))
            {
                p.Disconnect();
            }

            return !this.isInvalid;
        }

        private void ProcessQueue()
        {
            if (!this.initialized)
            {
                return;
            }

            foreach (var item in this.queue)
            {
                this.GotMessage(item.Player, item.Message);
            }
        }

        public override void GotMessage(CommonPlayer player, Message message)
        {
            if (!this.initialized)
            {
                this.queue.Add(new QueueItem(player, message));
                this.ProcessQueue();
                return;
            }

            if (message.Type == "getCrew")
            {
                this.crew.SendGetMessage(player);
                return;
            }

            if (player.IsGuest)
            {
                return;
            }

            switch (message.Type)
            {
                case "swapUsers":
                {
                    if (player.ConnectUserId != "merge")
                    {
                        break;
                    }

                    this.crew.SwapMembers(message.GetString(0), message.GetString(1), () => player.Send(message.Type));
                    break;
                }

                case "subscribe":
                {
                    if (crew.isContest)
                    {
                        this.SendErrorReply(Message.Create(message.Type), player, "Subscribing to contest crews is disabled.");
                        return;
                    }
                    if (this.crewId == "everybodyeditsstaff")
                    {
                        break;
                    }

                    NotificationHelper.AddSubscription(this.PlayerIO.BigDB, player.ConnectUserId, "crew" + this.crewId,
                        subscribed =>
                        {
                            if (subscribed)
                            {
                                this.crew.Subscribers++;
                                this.crew.Save();
                            }
                            player.Send(message.Type, this.crew.Subscribers);
                        });
                    break;
                }

                case "unsubscribe":
                {
                    if (this.crewId == "everybodyeditsstaff")
                    {
                        break;
                    }

                    NotificationHelper.RemoveSubscription(this.PlayerIO.BigDB, player.ConnectUserId,
                        "crew" + this.crewId,
                        unsubscribed =>
                        {
                            if (unsubscribed)
                            {
                                if (crew.Subscribers > 0)
                                {
                                    this.crew.Subscribers--;
                                    this.crew.Save();
                                }
                            }
                            player.Send(message.Type, this.crew.Subscribers);
                        });
                    break;
                }

                case "answerInvite":
                {
                    if (crew.isContest)
                    {
                        this.SendErrorReply(Message.Create(message.Type), player, "Joining is disabled for contest crews.");
                        return;
                    }
                    var accept = message.GetBoolean(0);

                    InvitationHelper.GetInvitation(this.PlayerIO.BigDB, InvitationType.Crew,
                        this.crewId, player.Name, invitation =>
                        {
                            var rtn = Message.Create(message.Type);

                            if (!invitation.Exists)
                            {
                                this.SendErrorReply(rtn, player, "Crew invite not found.");
                                return;
                            }
                            if (invitation.Status != InvitationStatus.Pending)
                            {
                                this.SendErrorReply(rtn, player, "Invitation already answered.");
                                return;
                            }
                            if (crew.IsMember(player))
                            {
                                this.SendErrorReply(rtn, player, "Already member of crew.");
                                return;
                            }

                            this.PlayerIO.BigDB.Load("CrewMembership", player.ConnectUserId, membership =>
                            {
                                if (accept)
                                {
                                    if (membership != null && membership.Count >= 10)
                                    {
                                        this.SendErrorReply(rtn, player, "You can't be in more than 10 crews.");
                                        return;
                                    }

                                    invitation.Status = InvitationStatus.Accepted;
                                    crew.AddMember(player);
                                }
                                else
                                {
                                    invitation.Status = InvitationStatus.Rejected;
                                }

                                rtn.Add(true);
                                invitation.Save(() => player.Send(rtn));
                            });
                        });
                    break;
                }
            }

            if (!this.crew.IsMember(player))
            {
                return;
            }

            switch (message.Type)
            {
                case "setMemberInfo":
                {
                    var username = message.GetString(0).ToLower();
                    var about = message.GetString(1);
                    if (about.Length > 100)
                    {
                        about = about.Substring(100);
                    }

                    this.SetMemberInfo(Message.Create(message.Type), player, username, about);
                    break;
                }
                case "setMemberRank":
                {
                    var username = message.GetString(0).ToLower();
                    var rank = message.GetInt(1);

                    this.SetMemberRank(Message.Create(message.Type), player, username, rank);
                    break;
                }
                case "editRank":
                {
                    var rankId = message.GetInt(0);
                    var name = message.GetString(1).Trim();
                    var powers = message.GetString(2);

                    this.EditRank(Message.Create(message.Type), player, rankId, name, powers);
                    break;
                }
                case "removeMember":
                {
                    if (crew.isContest)
                    {
                        this.SendErrorReply(Message.Create(message.Type), player, "Removing members is disabled for contest crews.");
                        return;
                    }
                    var username = message.GetString(0).ToLower();

                    this.RemoveMember(Message.Create(message.Type), player, username);
                    break;
                }
                case "leaveCrew":
                {
                    if (crew.isContest)
                    {
                        this.SendErrorReply(Message.Create(message.Type), player, "Leaving the crew is disabled for contest crews.");
                        return;
                    }

                    if (crew.IsMember(player) && !crew.GetRank(player).IsOwner)
                    {
                        crew.DatabaseObject.GetObject("Members").Remove(player.ConnectUserId);
                        this.PlayerIO.BigDB.LoadOrCreate("CrewMembership", player.ConnectUserId, membership =>
                        {
                            membership.Remove(this.crewId);
                            membership.Save();
                            this.crew.Save();
                            player.Send(message.Type);
                            player.Disconnect();
                        });
                    }
                    break;
                }

                case "inviteMember":
                {
                    if (crew.isContest)
                    {
                        this.SendErrorReply(Message.Create(message.Type), player, "Inviting members is disabled for contest crews.");
                        return;
                    }

                    var username = message.GetString(0).ToLower();

                    this.InviteMember(player, username,
                        () => player.Send(message.Type, true),
                        error =>
                        {
                            switch (error)
                            {
                                case InvitationError.AlreadyAdded:
                                {
                                    player.Send(message.Type, false,
                                        "This user is already a member of this crew.");
                                    break;
                                }
                                case InvitationError.AlreadySent:
                                {
                                    player.Send(message.Type, false,
                                        "This user already has a pending invitation.");
                                    break;
                                }
                                case InvitationError.PlayerNotFound:
                                {
                                    player.Send(message.Type, false,
                                        "Unknown user. Please check your spelling.");
                                    break;
                                }
                                case InvitationError.Blocked:
                                {
                                    player.Send(message.Type, false,
                                        "This user is blocking all crew invitations.");
                                    break;
                                }
                                case InvitationError.NotAllowed:
                                {
                                    player.Send(message.Type, false,
                                        "You have not been given rights to invite new members to this crew.");
                                    break;
                                }
                                case InvitationError.LimitReached:
                                {
                                    player.Send(message.Type, false,
                                        "Crew members limit reached.");
                                    break;
                                }
                                default:
                                {
                                    player.Send(message.Type, false,
                                        "Something went wrong. Try again later.");
                                    break;
                                }
                            }
                        });
                    break;
                }

                case "deleteInvite":
                {
                    var recipientName = message.GetString(0).ToLower();

                    if (!crew.HasPower(player, CrewPower.MembersManagement))
                    {
                        player.Send(message.Type, false);
                    }
                    else
                    {
                        InvitationHelper.DeleteInvitation(this.PlayerIO.BigDB, InvitationType.Crew,
                            this.crewId, recipientName, success =>
                            {
                                if (!success)
                                {
                                    this.PlayerIO.ErrorLog.WriteError(
                                        "Error deleting crew invitation from " + this.crewId + " to " +
                                        recipientName, "Invite not found",
                                        "Error deleting pending invitation", null);
                                }
                                player.Send(message.Type, success);
                            });
                    }
                    break;
                }

                case "getPendingInvites":
                {
                    if (!crew.HasPower(player, CrewPower.MembersManagement))
                    {
                        player.Send(message.Type, false);
                    }
                    else
                    {
                        InvitationHelper.GetInvitationsFrom(this.PlayerIO.BigDB, InvitationType.Crew,
                            this.crewId, invites =>
                            {
                                var rtn = Message.Create(message.Type);
                                rtn.Add(true);
                                foreach (var invite in invites)
                                {
                                    rtn.Add(invite.Recipient);
                                    rtn.Add((int)invite.Status);
                                }
                                player.Send(rtn);
                            });
                    }
                    break;
                }

                case "sendAlert":
                {
                    if (!this.crew.HasPower(player, CrewPower.AlertSending))
                    {
                        player.Send(message.Type, "You don't have rights to send alerts from this crew.");
                        break;
                    }

                    var text = message.GetString(0).Trim();
                    if (text.Length > 140)
                    {
                        text = text.Substring(0, 140);
                    }

                    this.crew.PublishNotification(text, notification =>
                    {
                        player.Send("info", "Success", "Alert sent to all crew subscribers.");
                        player.Send(message.Type);
                    });
                    break;
                }
                case "setColors":
                {
                    if (!this.crew.HasPower(player, CrewPower.ProfileCustomization))
                    {
                        break;
                    }

                    if (!this.crew.Unlocked("ColorPick"))
                    {
                        break;
                    }

                    this.crew.TextColor = message.GetUInt(0);
                    this.crew.PrimaryColor = message.GetUInt(1);
                    this.crew.SecondaryColor = message.GetUInt(2);
                    this.crew.SetUnlocked("ColorPick", false);

                    this.crew.Save();
                    break;
                }
                case "setFaceplate":
                {
                    if (!this.crew.HasPower(player, CrewPower.ProfileCustomization))
                    {
                        break;
                    }

                    var id = message.GetString(0).ToLower();
                    var color = message.GetInt(1);

                    if ((id != "none" && id != "" && !this.crew.Faceplates.Contains(id)) || color < 0 || color > 9)
                    {
                        break;
                    }

                    if (id == "none")
                    {
                        id = "";
                    }

                    this.crew.Faceplate = id;
                    this.crew.FaceplateColor = color;
                    this.crew.Save();
                    break;
                }
                case "disband":
                {
                    if (crew.isContest)
                    {
                        this.SendErrorReply(Message.Create(message.Type), player, "Disbanding the crew is disabled for contest crews.");
                        return;
                    }

                    if (!this.crew.GetRank(player).IsOwner)
                    {
                        break;
                    }

                    // Cleanup membership of all members
                    var members = this.crew.DatabaseObject.GetObject("Members").Properties.ToArray();
                    this.PlayerIO.BigDB.LoadKeys("CrewMembership", members, membership =>
                    {
                        foreach (var m in membership.Where(m => m != null))
                        {
                            m.Remove(this.crewId);
                            m.Save();
                        }
                    });

                    // Cleanup all pending invitations. People can't join here anymore
                    InvitationHelper.GetInvitationsFrom(this.PlayerIO.BigDB, InvitationType.Crew,
                        this.crewId, invites => { InvitationHelper.DeleteInvitations(this.PlayerIO.BigDB, invites); });

                    // Consume one "crew" PayVault item to allow user to buy another crew since this one is no longer active
                    player.PayVault.Refresh(() =>
                    {
                        var crewItem = player.PayVault.First("crew");
                        if (crewItem != null)
                        {
                            player.PayVault.Consume(new[] { crewItem }, this.SetAsInvalid,
                                error => { this.SetAsInvalid(); });
                        }
                        else
                        {
                            this.SetAsInvalid();
                        }
                    });

                    // Delete the crew object
                    this.crew.Disband();

                    player.Send(message.Type);

                    break;
                }

                default:
                {
                    this.shop.GotMessage(player, message);
                    break;
                }
            }
        }

        private void SendErrorReply(Message rtn, BasePlayer player, string error)
        {
            rtn.Add(false);
            rtn.Add(error);
            player.Send(rtn);
        }

        private void SetMemberInfo(Message rtn, BasePlayer player, string username, string about)
        {
            if (!this.crew.HasPower(player, CrewPower.MembersManagement))
            {
                this.SendErrorReply(rtn, player, "You don't have rights to change informations about this crew member.");
            }
            else
            {
                if (!this.crew.Unlocked("Descriptions"))
                {
                    this.SendErrorReply(rtn, player, "This crew didn't buy ability to set member descriptions.");
                }
                else
                {
                    this.crew.GetMemberByName(username, member =>
                    {
                        if (member == null)
                        {
                            this.SendErrorReply(rtn, player, "Member not found.");
                            return;
                        }

                        member.About = about;

                        rtn.Add(true);
                        rtn.Add(about);
                        player.Send(rtn);
                        this.crew.Save();
                    });
                }
            }
        }

        private void SetMemberRank(Message rtn, BasePlayer player, string username, int rankId)
        {
            if (!crew.HasPower(player, CrewPower.MembersManagement))
            {
                this.SendErrorReply(rtn, player, "You don't have rights to change rank of this crew member.");
            }
            else
            {
                crew.GetMembers(members =>
                {
                    var me = members.FirstOrDefault(it => it.Id == player.ConnectUserId);
                    var rank = crew.GetRank(rankId);

                    if (me == null || rank == null)
                    {
                        this.SendErrorReply(rtn, player, "Rank not found.");
                    }
                    else if (rank.HasPower(CrewPower.MembersManagement) && !me.Rank.IsOwner)
                    {
                        this.SendErrorReply(rtn, player, "Only crew owner can give rank with member management rights.");
                    }
                    else
                    {
                        var member = members.FirstOrDefault(it => it.Name == username);

                        if (member == null || member.Rank == null || member.Rank.IsOwner ||
                            (!me.Rank.IsOwner && member.Rank.HasPower(CrewPower.MembersManagement)))
                        {
                            this.SendErrorReply(rtn, player,
                                "You can't change rank of owner or other members with member management rights.");
                        }
                        else
                        {
                            member.Rank = rank;

                            rtn.Add(true);
                            rtn.Add(rank.Id);
                            player.Send(rtn);
                            this.crew.Save();
                        }
                    }
                });
            }
        }

        private void EditRank(Message rtn, BasePlayer player, int rankId, string name, string powers)
        {
            if (!crew.HasPower(player, CrewPower.RanksManagement))
            {
                this.SendErrorReply(rtn, player, "You don't have rights to edit ranks in this crew.");
            }
            else
            {
                var rank = crew.GetRank(rankId);
                if (rank == null)
                {
                    this.SendErrorReply(rtn, player, "Rank not found.");
                    return;
                }

                if (name.Length < 3 || name.Length > 20)
                {
                    this.SendErrorReply(rtn, player, "Rank name should have at least 3 characters and not more than 20.");
                    return;
                }

                rtn.Add(true);

                rank.Name = name;
                rtn.Add(name);

                var myRankId = crew.GetRankId(player);
                if (rank.Id > 0 && myRankId != rank.Id && (!rank.HasPower(CrewPower.RanksManagement) || myRankId == 0))
                {
                    if (powers.Length > 0)
                    {
                        foreach (var power in powers.Split(','))
                        {
                            int p;
                            if (int.TryParse(power, out p))
                            {
                                if (p >= 0 && p < (int)CrewPower.Count)
                                {
                                    continue;
                                }

                                this.SendErrorReply(rtn, player, "Invalid powers.");
                                return;
                            }
                            this.SendErrorReply(rtn, player, "Invalid powers.");
                            return;
                        }
                    }

                    rank.PowersString = powers;
                }
                rtn.Add(rank.PowersString);

                player.Send(rtn);
                this.crew.Save();
            }
        }

        private void RemoveMember(Message rtn, BasePlayer player, string username)
        {
            crew.GetMembers(members =>
            {
                var me = members.FirstOrDefault(it => it.Id == player.ConnectUserId);
                var member = members.FirstOrDefault(it => it.Name == username);

                if (me == null || member == null)
                {
                    player.Send(rtn);
                    return;
                }

                // Only members with member management power can remove players with lower powers
                if (me.Rank.HasPower(CrewPower.MembersManagement) &&
                    (!member.Rank.HasPower(CrewPower.MembersManagement) || me.Rank.IsOwner) && !member.Rank.IsOwner)
                {
                    crew.DatabaseObject.GetObject("Members").Remove(member.Id);
                    this.PlayerIO.BigDB.Load("CrewMembership", member.Id, membership =>
                    {
                        membership.Remove(this.crewId);
                        membership.Save();
                        player.Send(rtn);
                        this.crew.Save();
                    });
                }
                else
                {
                    player.Send(rtn);
                }
            });
        }

        private void InviteMember(BasePlayer player, string username, Callback successCallback,
            Callback<InvitationError> errorCallback)
        {
            if (!this.crew.HasPower(player, CrewPower.MembersManagement))
            {
                errorCallback(InvitationError.NotAllowed);
                return;
            }

            this.crew.GetMembers(members =>
            {
                if (members.Count >= 25)
                {
                    errorCallback(InvitationError.LimitReached);
                    return;
                }

                var member = members.FirstOrDefault(it => it.Name == username);
                if (member != null)
                {
                    errorCallback(InvitationError.AlreadyAdded);
                    return;
                }

                CommonPlayer.GetId(this.PlayerIO.BigDB, username, userId =>
                {
                    if (userId == null)
                    {
                        errorCallback(InvitationError.PlayerNotFound);
                        return;
                    }

                    InvitationHelper.GetInvitation(this.PlayerIO.BigDB, InvitationType.Crew,
                        this.crewId, username, oldInvitation =>
                        {
                            if (oldInvitation.Exists)
                            {
                                errorCallback(InvitationError.AlreadySent);
                                return;
                            }

                            InvitationBlocking.IsCrewBlocked(this.PlayerIO.BigDB, username, this.crewId, blocked =>
                            {
                                if (blocked)
                                {
                                    errorCallback(InvitationError.Blocked);
                                    return;
                                }

                                var newInvitation = new DatabaseObject();
                                newInvitation.Set("Sender", this.crewId);
                                newInvitation.Set("Recipient", username);
                                newInvitation.Set("Status", (int)InvitationStatus.Pending);

                                InvitationHelper.CreateInvitation(this.PlayerIO.BigDB, InvitationType.Crew,
                                    this.crewId, username,
                                    invitation => successCallback());
                            });
                        });
                });
            });
        }
    }
}