using System;

namespace EverybodyEdits.Game.ChatCommands
{
    internal class UnMuteCommand : ChatCommand
    {
        public UnMuteCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            // usage: /unmute some_name
            if (commandInput.Length < 2)
            {
                // no parameters provided, return error message
                Console.WriteLine("MuteCommand >> No parameters provided, return error message");
                player.Send("write", this._game.SystemName, "Please specify a username to unmute");
                return;
            }

            var playerNameToUnMute = commandInput[1].Trim().ToLower();

            if (playerNameToUnMute.ToLower().Equals(player.name))
            {
                // you cannot mute yourself, notify player
                // Console.WriteLine("MuteCommand >> You cannot unmute yourself, notify player");
                player.Send("write", this._game.SystemName, "You are trying to unmute yourself?");
                return;
            }

            Player playerObjectToUnMute = null;
            foreach (var p in this._game.Players)
            {
                if (p.name.ToLower().Equals(playerNameToUnMute))
                {
                    playerObjectToUnMute = p;
                    break;
                }
            }

            if (playerObjectToUnMute != null)
            {
                if (!player.mutedUsers.Contains(playerObjectToUnMute.ConnectUserId))
                {
                    // Player is in muted list
                    // Console.WriteLine("UnMuteCommand >> Player is not muted");
                    player.Send("write", this._game.SystemName, "Player is not muted");
                    return;
                }
                // Console.WriteLine("UnMuteCommand >> Removing player from muted list");
                player.mutedUsers.Remove(playerObjectToUnMute.ConnectUserId);
                player.Send("write", this._game.SystemName, playerNameToUnMute + " is unmuted");
            }
            else
            {
                // Console.WriteLine("UnMuteCommand >> Player with name " + playerNameToUnMute + " was not found");
                player.Send("write", this._game.SystemName, "Player with name " + playerNameToUnMute + " was not found");
            }
        }
    }
}