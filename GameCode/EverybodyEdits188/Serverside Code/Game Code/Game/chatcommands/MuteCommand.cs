namespace EverybodyEdits.Game.ChatCommands
{
    internal class MuteCommand : ChatCommand
    {
        public MuteCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            // usage: /mute some_name
            if (commandInput.Length < 2)
            {
                // no parameters provided, return error message
                // Console.WriteLine("MuteCommand >> No parameters provided, return error message");
                player.Send("write", this._game.SystemName, "Please specify a username to mute");
                return;
            }

            var playerNameToMute = commandInput[1].Trim().ToLower();

            if (playerNameToMute.ToLower().Equals(player.name))
            {
                // you cannot mute yourself, notify player
                // Console.WriteLine("MuteCommand >> You cannot mute yourself, notify player");
                player.Send("write", this._game.SystemName, "You can not mute yourself.");
                return;
            }

            Player playerObjectToMute = null;
            foreach (var p in this._game.Players)
            {
                if (p.name.ToLower().Equals(playerNameToMute))
                {
                    playerObjectToMute = p;
                    break;
                }
            }


            if (playerObjectToMute != null)
            {
                // You cannot mute mods
                if (playerObjectToMute.canbemod)
                {
                    player.Send("write", this._game.SystemName, "Nice try, but you cannot mute moderators.");
                    return;
                }

                if (player.mutedUsers.Contains(playerObjectToMute.ConnectUserId))
                {
                    // Player is in muted list
                    // Console.WriteLine("MuteCommand >> Player is already muted");
                    player.Send("write", this._game.SystemName, "Player is already muted.");
                    return;
                }
                // Console.WriteLine("MuteCommand >> Adding player to muted list");
                player.mutedUsers.Add(playerObjectToMute.ConnectUserId);
                player.Send("write", this._game.SystemName, playerNameToMute + " is now muted.");
            }

            else
            {
                // Console.WriteLine("MuteCommand >> Player with name " + playerNameToMute + " was not found");
                player.Send("write", this._game.SystemName, "Player with name " + playerNameToMute + " was not found");
            }
        }
    }
}