using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class LoadLevelCommand : ChatCommand
    {
        public LoadLevelCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "loadlevel")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            if (!this.Game.Owned || this.Game.BaseWorld == null)
            {
                return;
            }

            this.Game.BaseWorld.Reload();

            var mess = Message.Create("reset");

            this.Game.BaseWorld.AddToMessageAsComplexList(mess);
            this.Game.BroadcastMessage(mess);

            this.Game.ResetPlayers();
        }
    }
}