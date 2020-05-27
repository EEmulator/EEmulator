using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    class NameCommand : ChatCommand
    {
        public NameCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "name")
        {
            this.LimitArguments(1, "You must provide a new world name.");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var newName = string.Join(" ", commandInput.Skip(1));
            if (newName.Length <= 20)
            {
                this.Game.SetWorldName(newName);
                this.SendSystemMessage(player, "World name changed to: " + newName);
            }
            else
            {
                this.SendSystemMessage(player, "That name is too long. The maximum length of a world name is 20 characters.");
            }
        }
    }
}
