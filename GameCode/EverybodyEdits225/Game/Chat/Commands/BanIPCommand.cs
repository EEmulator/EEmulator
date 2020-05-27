using PlayerIO.GameLibrary;
using System.Linq;
using System;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class BanIpCommand : ChatCommand
    {
        public BanIpCommand(EverybodyEdits game) : base(game, CommandAccess.Moderator, "banip", "ipban") { this.LimitArguments(2, "You must specify a username and duration."); }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            this.Game.PlayerIO.BigDB.LoadSingle("OnlineStatus", "name", new object[] { commandInput[1].ToLower() }, onlineStatusObj => {
                if (onlineStatusObj == null) {
                    this.SendSystemMessage(player, "User {0} not found.", commandInput[1]);
                    return;
                }
                string ipAddress = onlineStatusObj.GetString("ipAddress", "");
                if (ipAddress == "") {
                    this.SendSystemMessage(player, "User {0} does not have an IpAddress o_O", commandInput[1]);
                    return;
                }

                this.Game.PlayerIO.BigDB.LoadOrCreate("IPBans", ipAddress, value => {
                    value.Set("ID", player.Name.ToUpper() + Guid.NewGuid().ToString("N").ToUpper().Substring(0, 8));
                    value.Set("BannedAt", DateTime.UtcNow);

                    int duration;
                    if (int.TryParse(commandInput[2], out duration)) {
                        var expDate = DateTime.UtcNow + new TimeSpan(duration, 0, 0, 0, 0);
                        value.Set("ExpirationDate", expDate);

                        this.SendSystemMessage(player, "User {0} banned with IP address {1}. It will expire on {2}.", commandInput[1], ipAddress, expDate.ToShortDateString());
                    }
                    else {
                        if (commandInput[2] == "eliminate!") {
                            value.Set("ExpirationDate", DateTime.UtcNow.AddDays(90));
                            value.Set("isEliminated", true);

                            this.Game.PlayerIO.BigDB.LoadRange("OnlineStatus", "ipAddress", null, ipAddress, ipAddress, 1000, accounts => {
                                foreach (var account in accounts.Where(o => DateTime.UtcNow.Subtract(o.GetDateTime("lastUpdate", DateTime.UtcNow)).TotalHours <= 2)) {
                                    if (account.Key == "simpleguest") {
                                        continue;
                                    }

                                    this.Game.PlayerIO.BigDB.Load("PlayerObjects", account.Key, playerObject => {
                                        playerObject.Set("banned", true);
                                        playerObject.Set("ban_reason", "Eliminated.");

                                        playerObject.Save();
                                    }, error => this.Game.PlayerIO.ErrorLog.WriteError("[IPBan] " + error.Message));
                                }

                                this.SendSystemMessage(player, "User {0} banned with IP address {1}.\nEliminated {2} other accounts", commandInput[1], ipAddress, accounts.Length);
                            });
                        }
                        else {
                            this.SendSystemMessage(player, "Durations are in days... not anything else.");

                            return;
                        }
                    }

                    value.Save();
                    var target = this.Game.FilteredPlayers.FirstOrDefault(p => p.Name.ToLower() == commandInput[1].ToLower());
                    if (target != null) {
                        target.SendMessage("banned");
                        target.SendMessage("info", "Account Banned", "Your IP Address has been banned for " + duration + " day" + (duration != 1 ? "s" : "") + "!");
                        target.Disconnect();
                    }
                });
            });
        }

        public static void CheckIpBanned(Client client, BasePlayer player, Callback<bool> callback)
        {
            client.BigDB.Load("IPBans", player.IPAddress.ToString(), o => {
                if (o == null) {
                    callback(false);
                    return;
                }

                if (o.Contains("ExpirationDate")) {
                    var expiration = o.GetDateTime("ExpirationDate");

                    var ipBanned = expiration > DateTime.Now;
                    player.PlayerObject.Save(() => {
                        if (ipBanned) {
                            var timeleft = expiration - DateTime.Now;
                            if (o.GetBool("isEliminated", false) && player.ConnectUserId!= "simpleguest" && o.Key != "simpleguest") {
                                player.PlayerObject.Set("banned", true);
                                player.PlayerObject.Set("ban_reason", "Auto Eliminated.");
                                player.PlayerObject.Save();
                            }

                            player.Send("info", "Banned",
                                "Your ip-address has been banned. Ban will be lifted in " +
                                timeleft.Days + " day" + (timeleft.Days != 1 ? "s" : "") + ", " +
                                timeleft.Hours + " hour" + (timeleft.Hours != 1 ? "s" : "") + " and " +
                                timeleft.Minutes + " minute" + (timeleft.Minutes != 1 ? "s" : "") + ". ");

                            player.Send("banned");
                        }

                        callback(ipBanned);
                    }, value => callback(false));
                }
                else callback(false);
            });
        }
    }
}