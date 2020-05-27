using System.Collections.Generic;
using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class ListPortalsCommand : ChatCommand
    {
        public ListPortalsCommand(EverybodyEdits game)
            : base(game, CommandAccess.PlayerWithEdit, "listportals")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var output = string.Empty;

            var portals = this.Game.BaseWorld.GetPortals();
            if (portals.Count > 0)
            {
                output += "Portals:\n";
                this.AddOutput(portals, ref output);
            }

            var invisiblePortals = this.Game.BaseWorld.GetInvisiblePortals();
            if (invisiblePortals.Count > 0)
            {
                output += "\nInvisible Portals:\n";
                this.AddOutput(invisiblePortals, ref output);
            }

            if (output == string.Empty)
            {
                output = "No portals found";
            }

            this.SendSystemMessage(player, output);
        }

        private void AddOutput(List<Item> portals, ref string output)
        {
            portals.Sort((a, b) =>
            {
                if (a.Block.PortalId != b.Block.PortalId)
                {
                    return a.Block.PortalId.CompareTo(b.Block.PortalId);
                }
                if (a.Block.PortalTarget != b.Block.PortalTarget)
                {
                    return a.Block.PortalTarget.CompareTo(b.Block.PortalTarget);
                }
                return a.X != b.X ? a.X.CompareTo(b.X) : a.Y.CompareTo(b.Y);
            });

            var data = new Dictionary<string, string>();
            foreach (var portal in portals)
            {
                var key = portal.Block.PortalId + " -> " + portal.Block.PortalTarget;
                var found = data.Keys.FirstOrDefault(it => it == key);

                if (found == null)
                {
                    data[key] = key + ":\n   ";
                }
                else
                {
                    data[key] += ", ";
                }

                data[key] += string.Format("{0}x{1}", portal.X, portal.Y);
            }

            output = data.Aggregate(output, (current, item) => current + item.Value + "\n");
        }
    }
}