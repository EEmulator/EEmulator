using System;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.ChatCommands
{
    internal class TempBan : ChatCommand
    {
        public TempBan(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;
            this.banUser(_player, commandInput);
        }

        protected void banUser(Player player, string[] commandInput)
        {
            if (!player.canbemod && !player.CanBeGuardian)
            {
                return;
            }

            if (commandInput.Length < 2)
            {
                player.Send("write", this._game.SystemName, "You must define a user to ban");
            }

            this._game.PlayerIO.BigDB.Load("usernames", commandInput[1].ToLower(), delegate(DatabaseObject o)
            {
                if (o == null || o.GetString("owner", null) == null)
                {
                    player.Send("write", this._game.SystemName, "User " + commandInput[1].ToUpper() + " not found");
                    return;
                }

                this._game.PlayerIO.BigDB.Load("PlayerObjects", o.GetString("owner", "waggag"),
                    delegate(DatabaseObject user)
                    {
                        if (o == null)
                        {
                            player.Send("write", this._game.SystemName, "Crap, something went horriably wrong...");
                            return;
                        }

                        if (user.GetBool("isModerator", false) || user.GetBool("isGuardian", false))
                        {
                            player.Send("write", this._game.SystemName, "Dude, stop that!");
                        }
                        var duration = 1;
                        try
                        {
                            duration = int.Parse(commandInput[2]);
                        }
                        catch
                        {
                            player.Send("write", this._game.SystemName, "Ban duration is malformed or missing");
                        }
                        this.CommitBanToDatabase(player, duration, commandInput, user);
                        this._game.KickUser(user.Key, duration);
                    });
            });
        }

        private void CommitBanToDatabase(Player player, int duration, string[] commandInput, DatabaseObject user)
        {
            this._game.PlayerIO.BigDB.LoadOrCreate("TempBans", user.Key, delegate(DatabaseObject tempBans)
            {
                tempBans.Set("Name", user.GetString("name", ""));
                tempBans.Set("Latest", DateTime.Now);

                if (!tempBans.Contains("Bans"))
                {
                    tempBans.Set("Bans", new DatabaseArray());
                }
                var banEntries = tempBans.GetArray("Bans");

                var banRow = new DatabaseObject();

                var reason = "";
                if (commandInput.Length >= 3)
                {
                    for (var a = 3; a < commandInput.Length; a++)
                    {
                        reason += commandInput[a] + " ";
                    }
                }

                var now = DateTime.Now;
                var expirationDate = now.AddDays(duration);

                banRow.Set("Date", now);
                banRow.Set("Id", player.name.Substring(0, 2).ToUpper() + now.ToString("yyMMddHHmmss"));
                banRow.Set("BannedBy", player.name);
                banRow.Set("Duration", duration);
                banRow.Set("Expires", expirationDate);
                banRow.Set("Reason", reason);
                banEntries.Add(banRow);

                tempBans.Save();

                // disconnet the culprit
                foreach (var p in this._game.Players)
                {
                    if (p.name.ToLower() == commandInput[1].ToLower())
                    {
                        p.Send("info", "Banned",
                            "You are banned for the next: " + duration + " day" + (duration > 1 ? "s" : "") + ".\nId:" +
                            banRow.GetString("Id") + "\n\"" + reason + "\"");
                        p.PlayerObject.Set("tempbanned", true);
                        p.PlayerObject.Save(delegate { p.Disconnect(); });
                    }
                }

                player.Send("write", this._game.SystemName,
                    "You banned " + user.GetString("name", "unknown") + " for a duration of " + duration + " day" +
                    (duration > 1 ? "s" : "") + ", ban expires on: " + expirationDate + " (servertime).");
            });
        }
    }
}