using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class PlayerInfoCommand : ChatCommand
    {
        public PlayerInfoCommand(EverybodyEdits game) : base(game, CommandAccess.Admin, "playerinfo", "pinfo") { }
        protected override void OnExecute(Player player, string[] commandInput)
        {
            var targets = commandInput.Length > 1 ? this.Game.FilteredPlayers.Where(p => p.Name.ToString().ToLower() == commandInput[1].ToLower()) : this.Game.Players;
            var output = targets.Aggregate("", (current, target) => current + string.Format("{0} [{3}]: IsBot: {1} Client: {2}\n", target.Name, target.IsBot, target.ClientType, target.IPAddress));

            player.SendMessage("info", "Player Info", output.Substring(0, output.Length - 1));
        }
    }
}