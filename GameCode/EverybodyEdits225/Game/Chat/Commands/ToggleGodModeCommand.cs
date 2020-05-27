using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class ToggleGodModeCommand : ChatCommand
    {
        public ToggleGodModeCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "givegod", "removegod")
        {
            this.LimitArguments(1, "Please specify a player.");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            foreach (var p in this.Game.FilteredPlayers.Where(p => p.Name.ToLower() == commandInput[1].ToLower()))
            {
                if (p.CanEdit)
                {
                    this.SendSystemMessage(player,
                        "Can't use this command on players that are allowed to edit the world.");
                    return;
                }

                var giveGod = commandInput[0] == "givegod";

                if (p.CanToggleGodMode == giveGod)
                {
                    this.SendSystemMessage(player, "This player {0} god mode rights.", giveGod ? "already has" : "doesn't have");
                    return;
                }

                this.Game.BroadcastMessage("toggleGod", p.Id, giveGod);
                p.CanToggleGodMode = giveGod;
                if (!giveGod)
                {
                    p.IsInGodMode = false;
                }

                this.SendSystemMessage(player, giveGod ? p.Name.ToUpper() + " may now use god mode." : p.Name.ToUpper() + " can no longer use god mode.");
                this.SendSystemMessage(p, giveGod ? "You may now use god mode." : "You can no longer use god mode.");
                break;
            }
        }
    }
}