namespace EverybodyEdits.Game.Chat.Commands
{
    internal class VisibilityCommand : ChatCommand
    {
        public VisibilityCommand(EverybodyEdits game) : base(game, CommandAccess.CrewMember, "visible")
        {
            this.LimitArguments(1, "Please specify the world's visible state. (nobody/friends/all)");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            if (this.Game.BaseWorld.IsArtContest)
            {
                this.SendSystemMessage(player, "This option cannot be changed in a contest world.");
                return;
            }

            var visibility = commandInput[1];

            this.Game.BaseWorld.FriendsOnly = false;
            this.Game.BaseWorld.Visible = true;

            switch (visibility)
            {
                case "friends":
                {
                    this.Game.BaseWorld.FriendsOnly = true;
                    this.Game.BaseWorld.Visible = true;
                }
                break;

                case "all":
                case "true":
                case "anyone":
                    this.Game.BaseWorld.Visible = true;
                    break;

                default: this.Game.BaseWorld.Visible = false; break;
            }

            this.Game.BaseWorld.Save(false);
            this.Game.SetVisibility(!this.Game.BaseWorld.HideLobby && this.Game.BaseWorld.Visible && this.Game.BaseWorld.CrewVisibleInLobby && !this.Game.BaseWorld.FriendsOnly);

            this.Game.BroadcastMessage("roomVisible", this.Game.BaseWorld.Visible, this.Game.BaseWorld.FriendsOnly);
            this.SendSystemMessage(player, this.Game.BaseWorld.FriendsOnly ? "Visible set to Friends Only." : this.Game.BaseWorld.Visible ? "Visible set to True." : "Visible set to False.");
        }
    }
}