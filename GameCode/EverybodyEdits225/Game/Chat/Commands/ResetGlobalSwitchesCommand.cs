namespace EverybodyEdits.Game.Chat.Commands
{
    class ResetGlobalSwitchesCommand : ChatCommand
    {
        public ResetGlobalSwitchesCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "resetswitches")
        {

        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            this.Game.ResetOrangeSwitches();
            this.SendSystemMessage(player, "All global switches were reset to their initial state.");
        }
    }
}
