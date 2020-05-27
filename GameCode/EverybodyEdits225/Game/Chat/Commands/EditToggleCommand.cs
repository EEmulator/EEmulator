using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class EditToggleCommand : ChatCommand
    {
        public EditToggleCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "giveedit", "removeedit", "gedit", "redit", "ge", "re")
        {
            this.LimitArguments(1, "You must specify a player to give/remove edit");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var command = commandInput[0].ToLower();
            var edit = command == "giveedit" || command == "gedit" || command == "ge";

            if (commandInput[1].ToLower() == this.Game.LevelOwnerName.ToLower())
            {
                this.SendSystemMessage(player, "You can not alter your own edit rights");
                return;
            }

            foreach (var p in this.Game.FilteredPlayers.Where(p => p.Name.ToLower() == commandInput[1].ToLower()))
            {
                if (this.Game.TrySetEditRights(p, edit))
                {
                    var write = (edit ? " can now" : " can no longer") + " edit this world.";
                    if (player.Id != p.Id)
                    {
                        this.SendSystemMessage(player, p.Name.ToUpper() + write);
                    }
                    this.SendSystemMessage(p, "You{0}", write);
                }
                else
                {
                    this.SendSystemMessage(player, "This player {0} edit rights.", edit ? "already has" : "doesn't have");
                }
                return;
            }
        }
    }
}