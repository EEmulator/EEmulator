using System.Linq;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class RespawnPlayerCommand : ChatCommand
    {
        public RespawnPlayerCommand(EverybodyEdits game) :
            base(game, CommandAccess.CrewMember, "respawn")
        { }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            if (!this.Game.Owned)
            {
                return;
            }

            if (commandInput.Length < 2)
            {
                this.RespawnPlayer(player);
            }
            else
            {
                var playerName = commandInput[1].ToLower();
                foreach (var p in this.Game.FilteredPlayers.Where(p => p.Name.ToLower() == playerName))
                {
                    this.RespawnPlayer(p);
                }
            }
        }

        public void RespawnPlayer(Player p)
        {
            if (p.IsInAdminMode || p.IsInGodMode || p.IsInModeratorMode)
            {
                return;
            }

            var tele = Message.Create("tele", false, false);

            this.Game.AddRespawnToMessage(p, tele);
            this.Game.BroadcastMessage(tele);
        }
    }
}