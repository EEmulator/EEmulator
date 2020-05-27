using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class MuteKickCommand : ChatCommand
    {
        public MuteKickCommand(EverybodyEdits game) : base(game, CommandAccess.Admin, "mkick")
        {
            this.LimitArguments(1, "You must specify a player to mute-kick.");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var wereKicked = false;

            this.Game.FilteredPlayers.Where(p => p.Name.ToLower() == commandInput[1].ToLower() && !p.IsAdmin).ToList().ForEach(target => {
                target.HasBeenKicked = true;
                target.Disconnect();
                wereKicked = true;
            });

            if (!wereKicked)
            {
                this.SendSystemMessage(player, "Unknown user {0}", commandInput[1]);
            }
        }
    }
}
