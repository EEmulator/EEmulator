using System;
using System.Linq;
using EverybodyEdits.Common;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class TempBanCommand : ChatCommand
    {
        public TempBanCommand(EverybodyEdits game)
            : base(game, CommandAccess.Moderator, "tempban", "tmpban")
        {
            this.LimitArguments(1, "Please specify a player to ban.");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            this.Game.PlayerIO.BigDB.Load("usernames", commandInput[1].ToLower(), delegate (DatabaseObject o)
            {
                if (o == null || o.GetString("owner", null) == null)
                {
                    this.SendSystemMessage(player, "User " + commandInput[1].ToUpper() + " not found.");
                    return;
                }

                this.Game.PlayerIO.BigDB.Load("PlayerObjects", o.GetString("owner"),
                    delegate (DatabaseObject user)
                    {
                        if (commandInput.Length <= 2)
                        {
                            this.SendSystemMessage(player, "You must specify the duration parameter. The format is '0d0h0m0s' - for example, 5 minutes 30 seconds would be 5m30s.");
                            return;
                        }

                        var duration = new SimpleTimeParser().ParseSimpleTimeSpan(commandInput[2]);
                        var reason = commandInput.Length >= 3 ? string.Join(" ", commandInput.Skip(3)) : "";

                        if (duration == TimeSpan.Zero)
                        {
                            this.SendSystemMessage(player, "You specified an invalid duration format. The format is '0d0h0m0s' - for example, 5 minutes 30 seconds would be 5m30s.");
                            return;
                        }

                        if (user.GetBool("isAdministrator", false) || user.GetBool("isModerator", false))
                        {
                            this.SendSystemMessage(player, "No thanks.");
                            return;
                        }

                        this.CommitBanToDatabase(player, user, duration, reason, commandInput);
                        this.Game.KickUser(user.Key, duration.Minutes);
                    });
            });
        }

        private void CommitBanToDatabase(Player player, DatabaseObject user, TimeSpan duration, string reason, string[] commandInput)
        {
            this.Game.PlayerIO.BigDB.LoadOrCreate("TempBans", user.Key, delegate (DatabaseObject tempBan)
            {
                var currentDate = DateTime.Now;

                tempBan.Set("Name", user.GetString("name", ""));
                tempBan.Set("Latest", currentDate);

                if (!tempBan.Contains("Bans"))
                {
                    tempBan.Set("Bans", new DatabaseArray());
                }

                var banEntries = tempBan.GetArray("Bans");
                var expiration = currentDate.Add(duration);

                var banRow = new DatabaseObject();
                banRow.Set("Date", currentDate);
                banRow.Set("Id", player.Name.Substring(0, 2).ToUpper() + currentDate.ToString("yyMMddHHmmss"));
                banRow.Set("BannedBy", player.Name);
                banRow.Set("Duration", duration.ToPrettyFormat());
                banRow.Set("ExpiryDuration", duration.TotalSeconds);
                banRow.Set("Reason", reason);
                banRow.Set("Active", this.Game.FilteredPlayers.Any(p => p.Name.ToLower() == commandInput[1].ToLower()));
                banEntries.Add(banRow);
                tempBan.Save();

                // disconnect the culprit
                foreach (var p in this.Game.FilteredPlayers.Where(p => p.Name.ToLower() == commandInput[1].ToLower()))
                {
                    p.SendMessage("info", "Banned", "You are banned for the next: " + duration.ToPrettyFormat() + ".\nReason: \"" + reason + "\"");
                    p.PlayerObject.Set("tempbanned", true);
                    p.PlayerObject.Save(delegate { p.Disconnect(); });
                }

                this.SendSystemMessage(player,
                    "You banned " + user.GetString("name", "unknown") + " for a duration of " + duration.ToPrettyFormat() + ", ban expires on: " + expiration + " (server time).");
            });
        }

        public static void CheckTempBanned(Client client, BasePlayer player, Callback<bool> callback)
        {
            client.BigDB.Load("TempBans", player.ConnectUserId, delegate (DatabaseObject o)
            {
                if (o == null)
                {
                    callback(false);
                    return;
                }

                if (o.Contains("Bans"))
                {
                    var tempBans = o.GetArray("Bans");

                    var orderedBans = tempBans.OrderByDescending(x => ((DatabaseObject)x).GetDateTime("Date")).ToArray();
                    var lastExpiration = orderedBans[0] as DatabaseObject;

                    if (lastExpiration == null)
                    {
                        Console.WriteLine("No 'lastExpiration' object: " + player.ConnectUserId);
                        callback(false);
                        return;
                    }

                    // deprecated
                    if (lastExpiration.Contains("Expires"))
                    {
                        Console.WriteLine("Last ban expires: " + lastExpiration.GetDateTime("Expires") + " (" +
                                      (lastExpiration.GetDateTime("Expires") > DateTime.Now) + ")");
                    }
                    else
                    {
                        Console.WriteLine("Last ban expires: " + lastExpiration.GetDateTime("Date").AddSeconds(lastExpiration.GetDouble("ExpiryDuration")) + " (" +
                                          (lastExpiration.GetDateTime("Date").AddSeconds(lastExpiration.GetDouble("ExpiryDuration")) > DateTime.Now) + ")");
                    }

                    // deprecated
                    var tempBanned = false;
                    if (lastExpiration.Contains("Expires"))
                    {
                        tempBanned = lastExpiration.GetDateTime("Expires") > DateTime.Now;
                    }
                    else
                    {
                        if (lastExpiration.GetBool("Active", false) == false)
                        {
                            var currentDate = DateTime.Now;

                            lastExpiration.Set("Date", currentDate);
                            lastExpiration.Set("Active", true);
                            o.Save();

                            tempBanned = currentDate.AddSeconds(lastExpiration.GetDouble("ExpiryDuration")) > DateTime.Now;
                        }
                        else
                        {
                            tempBanned = lastExpiration.GetDateTime("Date").AddSeconds(lastExpiration.GetDouble("ExpiryDuration")) > DateTime.Now;
                        }
                    }

                    player.PlayerObject.Set("tempbanned", tempBanned);

                    player.PlayerObject.Save(() =>
                    {
                        if (tempBanned)
                        {
                            if (lastExpiration.Contains("Expires"))
                            {
                                var timeleft = (lastExpiration.GetDateTime("Expires") - DateTime.Now);

                                player.Send("info", "Banned",
                                    "Your have been temporarily banned. Ban will be lifted in " +
                                    timeleft.Days + " day" + (timeleft.Days != 1 ? "s" : "") + ", " +
                                    timeleft.Hours + " hour" + (timeleft.Hours != 1 ? "s" : "") + " and " +
                                    timeleft.Minutes + " minute" + (timeleft.Minutes != 1 ? "s" : "") + ". " +
                                    "\n\nReason: \"" + lastExpiration.GetString("Reason", "No reason given.") + "\"");
                            }
                            else
                            {
                                player.Send("info", "Banned",
                                    "You have been temporarily banned. Ban will be lifted in " + lastExpiration.GetDateTime("Date").AddSeconds(lastExpiration.GetDouble("ExpiryDuration", 0)).Subtract(DateTime.Now).ToPrettyFormat() + ".\n" +
                                    "Reason: \"" + lastExpiration.GetString("Reason", "No reason provided.") + "\"");
                            }
                        }

                        callback(tempBanned);
                    });
                }
                else
                {
                    Console.WriteLine("No 'Bans' object: " + player.ConnectUserId);
                    callback(false);
                }
            });
        }
    }
}