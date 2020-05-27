namespace EverybodyEdits.Game.Chat.Commands
{
    internal class ResetProgressCommand : ChatCommand
    {
        public ResetProgressCommand(EverybodyEdits game) : base(game, CommandAccess.Public, "resetprogress", "resetp") { }
        protected override void OnExecute(Player player, string[] commandInput)
        {
            if (!this.Game.IsCampaign) {
                this.SendSystemMessage(player, "You cannot use this command outside campaign worlds.");
                return;
            }

            player.BackupCampaign = !player.BackupCampaign;
            var message = !player.BackupCampaign ? "now be reset" : "no longer reset";
            this.SendSystemMessage(player, "Your campaign progress will " + message + " when you leave this world.");
        }
    }
}