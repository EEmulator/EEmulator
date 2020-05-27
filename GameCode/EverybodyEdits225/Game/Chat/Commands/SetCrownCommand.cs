using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class SetCrownCommand : ChatCommand
    {
        public SetCrownCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "givecrown", "removecrown")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            if (commandInput[0] == "removecrown")
            {
                this.Game.CrownId = -1;
                this.SendSystemMessage(player, "Removed crown from its owner.");
            }
            else if (commandInput[0] == "givecrown" && commandInput.Length >= 2)
            {
                foreach (var pl in this.Game.FilteredPlayers.Where(pl => pl.Name == commandInput[1].ToLower()))
                {
                    this.Game.CrownId = pl.Id;
                    this.SendSystemMessage(player, "Gave crown to " + pl.Name.ToUpper());
                    break;
                }
            }
            else
            {
                this.SendSystemMessage(player, "Please specify a player to give crown to.");
            }
        }
    }
}