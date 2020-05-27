namespace EverybodyEdits.Game.Chat.Commands
{
    internal class HideLobbyCommand : ChatCommand
    {
        public HideLobbyCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "hidelobby")
        {
            this.LimitArguments(1, "Please specify whether the world should be hidden in lobby or not. (true/false)");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            if (this.Game.BaseWorld.IsArtContest)
            {
                this.SendSystemMessage(player, "This option cannot be changed in a contest world.");
                return;
            }

            this.Game.BaseWorld.HideLobby = commandInput[1].ToLower() == "true";
            this.Game.BaseWorld.Save(false);
            this.Game.SetVisibility(!this.Game.BaseWorld.HideLobby && this.Game.BaseWorld.Visible &&
                                this.Game.BaseWorld.CrewVisibleInLobby && !this.Game.BaseWorld.FriendsOnly);
            this.SendSystemMessage(player,
                "World is hidden from the lobby: " + this.Game.BaseWorld.HideLobby.ToString().ToUpper());
            this.Game.BroadcastMessage("hideLobby", this.Game.BaseWorld.HideLobby);
        }
    }
}