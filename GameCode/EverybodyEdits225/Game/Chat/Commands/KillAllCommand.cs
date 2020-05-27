using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class KillAllCommand : ChatCommand
    {
        public KillAllCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "killall")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            foreach (var p in this.Game.FilteredPlayers.Where(p => !p.IsInGodMode && !p.IsInModeratorMode && !p.IsInAdminMode))
            {
                this.Game.BroadcastMessage("kill", p.Id);
            }
        }
    }
}