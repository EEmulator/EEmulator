using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class CheckIpCommand : ChatCommand
    {
        public CheckIpCommand(EverybodyEdits game)
            : base(game, CommandAccess.Moderator, "checkip")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var ip = new Dictionary<IPAddress, List<string>>();

            foreach (var p in this.Game.Players)
            {
                if (!ip.ContainsKey(p.IPAddress))
                {
                    ip.Add(p.IPAddress, new List<string>());
                }
                ip[p.IPAddress].Add(p.Name);
            }

            var names = "";
            var count = 0;
            foreach (var nameList in ip.Values.Where(nameList => nameList.Count > 1))
            {
                count++;
                names += count + ": " + string.Join(",", nameList) + "\n";
            }

            if (count == 0)
            {
                player.SendMessage("info", "IP Check", "No similar IP Addresses");
            }
            else
            {
                player.SendMessage("info", "IP Check", "Same Ip: \n " + names);
            }
        }
    }
}