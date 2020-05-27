using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class UnMuteCommand : ChatCommand
    {
        public UnMuteCommand(EverybodyEdits game)
            : base(game, CommandAccess.Public, "unmute")
        {
            this.LimitArguments(1, "Please specify a player to unmute.");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var playerNameToUnMute = commandInput[1].Trim().ToLower();

            if (playerNameToUnMute.ToLower().Equals(player.Name))
            {
                this.SendSystemMessage(player, "Are you trying to unmute yourself?");
                return;
            }

            if (playerNameToUnMute == "*")
            {
                foreach (var p in this.Game.FilteredPlayers.Where(p => player.MutedUsers.Contains(p.ConnectUserId)))
                {
                    player.SendMessage("muted", p.Id, false);
                }

                this.SendSystemMessage(player, "unmuted {0} player{1}.", player.MutedUsers.Count, player.MutedUsers.Count != 1 ? "s" : "");
                player.MutedUsers.Clear();
                return;
            }

            var playerObjectToUnMute = this.Game.FilteredPlayers.FirstOrDefault(p => p.Name.ToLower().Equals(playerNameToUnMute));

            if (playerObjectToUnMute != null)
            {
                if (!player.MutedUsers.Contains(playerObjectToUnMute.ConnectUserId))
                {
                    this.SendSystemMessage(player, "Player is not muted.");
                    return;
                }

                player.MutedUsers.Remove(playerObjectToUnMute.ConnectUserId);
                player.SendMessage("muted", playerObjectToUnMute.Id, false);
                this.SendSystemMessage(player, playerNameToUnMute + " is now unmuted.");
            }
            else
            {
                this.SendSystemMessage(player, "Player with name " + playerNameToUnMute + " was not found.");
            }
        }
    }
}