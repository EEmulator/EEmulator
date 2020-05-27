namespace EverybodyEdits.Game.Chat.Commands
{
    internal class ResetAllCommand : ChatCommand
    {
        public ResetAllCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "resetall")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            if (this.Game.Owned)
            {
                this.Game.ResetPlayers();
            }
        }
    }
}