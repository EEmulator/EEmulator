using EverybodyEdits.Common;
using System.Globalization;
using System.Linq;
using System;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class TeleportCommand : ChatCommand
    {
        public TeleportCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "teleport", "tp")
        {
            this.LimitArguments(3, "Please specify a player to teleport, x and y target coordinates.");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            double x, y;

            try {
                x = (double.Parse(commandInput[2], new CultureInfo("en-GB"))) * Config.TileWidth;
                y = (double.Parse(commandInput[3], new CultureInfo("en-GB"))) * Config.TileHeight;
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                this.SendSystemMessage(player, "Couldn't determine target coordinates.");
                return;
            }

            if (x < 0 || y < 0 ||
                x > (this.Game.BaseWorld.Width - 1) * Config.TileWidth ||
                y > (this.Game.BaseWorld.Height - 1) * Config.TileHeight) {
                this.SendSystemMessage(player, "You cannot teleport players outside of the world.");
                return;
            }

            foreach (var p in this.Game.FilteredPlayers.Where(p => p.Name.ToLower() == commandInput[1].ToLower())) {
                if (!player.IsAdmin) {
                    if (p.IsInAdminMode || p.IsInModeratorMode) {
                        return;
                    }
                }

                p.X = x;
                p.Y = y;
                p.SpeedX = 0;
                p.SpeedY = 0;
                this.Game.LeaveCampaignMode(p);
                this.Game.BroadcastMessage("teleport", p.Id, x, y);
                return;
            }
        }
    }
}