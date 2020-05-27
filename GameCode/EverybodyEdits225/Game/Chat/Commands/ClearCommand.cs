namespace EverybodyEdits.Game.Chat.Commands
{
    internal class ClearCommand : ChatCommand
    {
        public ClearCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "clear")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            if (!this.Game.Owned)
            {
                return;
            }

            this.Game.OrangeSwitches.Clear();

            this.Game.BaseWorld.Reset();
            this.Game.BroadcastMessage("clear", this.Game.BaseWorld.Width, this.Game.BaseWorld.Height,
                this.Game.BaseWorld.BorderType,
                this.Game.BaseWorld.FillType);
        }
    }
}