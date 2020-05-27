namespace EverybodyEdits.Game.Chat.Commands
{
    internal class ArtContestCommand : ChatCommand
    {
        public ArtContestCommand(EverybodyEdits game)
            : base(game, CommandAccess.Moderator, "markcontest", "removecontest")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            switch (commandInput[0]) {
                case "markcontest":
                    var maximumBlockAssets = commandInput.Length > 1 ? int.Parse(commandInput[1]) : 21;

                    this.Game.MarkArtContestWorld(maximumBlockAssets);
                    this.BroadcastSystemMessage("The world has been successfully marked as an art contest.");
                    break;

                case "removecontest":
                    this.BroadcastSystemMessage("The world has been successfully removed from being an art contest.");
                    this.Game.BaseWorld.IsArtContest = false;
                    break;
            }
        }
    }
}
