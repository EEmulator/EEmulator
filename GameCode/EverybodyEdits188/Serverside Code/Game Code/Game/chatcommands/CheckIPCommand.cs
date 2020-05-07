using System.Collections.Generic;
using System.Net;

namespace EverybodyEdits.Game.ChatCommands
{
    public class CheckIPCommand : ChatCommand
    {
        public CheckIPCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;

            var ip = new Dictionary<IPAddress, List<string>>();

            foreach (var p in this._game.Players)
            {
                if (!ip.ContainsKey(p.IPAddress))
                {
                    ip.Add(p.IPAddress, new List<string>());
                }
                ip[p.IPAddress].Add(p.name);
            }

            var names = "";
            var count = 0;
            foreach (var nameList in ip.Values)
            {
                if (nameList.Count > 1)
                {
                    count++;
                    names += count + ": " + string.Join(",", nameList) + "\n";
                }
            }

            if (count == 0)
            {
                player.Send("info", "IP Check", "No similar IP Addresses");
            }
            else
            {
                player.Send("info", "IP Check", "Same Ip: \n " + names);
            }
        }
    }
}