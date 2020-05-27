namespace EverybodyEdits.Game.Chat.Commands
{
    internal class CopyWorldCommand : ChatCommand
    {
        public CopyWorldCommand(EverybodyEdits game)
            : base(game, CommandAccess.Admin, "copylevel")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            this.Game.CopyLevel(player, commandInput[1]);
        }
    }
}