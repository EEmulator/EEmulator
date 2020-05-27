using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class StealthCommand : ChatCommand
    {
        public StealthCommand(EverybodyEdits game) : base(game, CommandAccess.Moderator, "stealth") { }
        protected override void OnExecute(Player player, string[] commandInput)
        {
            var outStealth = player.Stealthy;
            player.TempStealth = commandInput.Length > 1;
            player.Stealthy = !player.Stealthy;
            player.CurrentWorldName = "";
            player.CurrentWorldId = "";
            player.Save();

            player.ChatColor = player.Stealthy ? 1337420 : 0u;
            foreach (var p in this.Game.Players.Where(p => p.Id != player.Id))
            {
                if (player.Stealthy && !outStealth)
                    p.SendMessage("left", player.Id);

                if (p.IsAdmin || p.IsModerator)
                {
                    p.SendMessage("write", ChatUtils.SystemName, player.Name.ToUpper() + " is " + (player.Stealthy ? "now" : "no longer") + " stealthy!");
                    this.Game.SendAddMessage(player, p);

                    continue;
                }

                if (!player.Stealthy)
                {
                    this.Game.SendAddMessage(player, p);
                }
                else
                {
                    if (this.Game.CrownId == player.Id)
                    {
                        this.Game.CrownId = -1;
                    }
                }
            }

            this.SendSystemMessage(player, "You're {0} stealthy!", player.Stealthy ? "now" : "no longer");
        }
    }
}