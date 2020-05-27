using System.Linq;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class ResetCommand : ChatCommand
    {
        public ResetCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "reset")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            if (!this.Game.Owned)
            {
                return;
            }

            if (commandInput.Length < 2)
            {
                this.ResetPlayer(player);
            }
            else
            {
                var playerName = commandInput[1].ToLower();
                foreach (var p in this.Game.FilteredPlayers.Where(p => p.Name.ToLower() == playerName))
                {
                    this.ResetPlayer(p);
                }
            }
        }

        private void ResetPlayer(Player player)
        {
            var tele = Message.Create("tele", true, false);
            if (this.Game.AddPlayerResetToMessage(tele, player))
            {
                this.Game.BroadcastMessage(tele);
            }
        }
    }
}