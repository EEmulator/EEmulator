using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class MuteCommand : ChatCommand
    {
        public MuteCommand(EverybodyEdits game) : base(game, CommandAccess.Public, "mute")
        {
            this.LimitArguments(1, "Please specify a player to mute.");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var playerNameToMute = commandInput[1].Trim().ToLower();

            if (playerNameToMute.ToLower().Equals(player.Name)) {
                this.SendSystemMessage(player, "You can not mute yourself.");
                return;
            }

            if (playerNameToMute == "*") {
                var playersToMute = this.Game.FilteredPlayers.Where(p => !p.IsAdmin && !p.IsModerator && !p.Name.Equals(player.Name)).ToList();
                foreach (var p in playersToMute) {
                    player.MutedUsers.Remove(p.ConnectUserId);
                    player.SendMessage("muted", p.Id, true);
                }

                player.MutedUsers.AddRange(playersToMute.Select(p => p.ConnectUserId));
                this.SendSystemMessage(player, "muted {0} player{1}.", playersToMute.Count, playersToMute.Count != 1 ? "s" : "");
                return;
            }

            var target = this.Game.FilteredPlayers.FirstOrDefault(p => p.Name.ToLower().Equals(playerNameToMute));

            if (target != null) {
                if (target.IsAdmin || target.IsModerator) {
                    this.SendSystemMessage(player, "Nice try, but you cannot mute staff.");
                    return;
                }

                if (player.MutedUsers.Contains(target.ConnectUserId)) {
                    this.SendSystemMessage(player, "Player is already muted.");
                    return;
                }

                player.MutedUsers.Add(target.ConnectUserId);
                player.SendMessage("muted", target.Id, true);
                this.SendSystemMessage(player, "{0} is now muted.", playerNameToMute);
            }
            else {
                this.SendSystemMessage(player, "Player with name {0} was not found.", playerNameToMute);
            }
        }
    }
}