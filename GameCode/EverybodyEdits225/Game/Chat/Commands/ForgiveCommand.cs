namespace EverybodyEdits.Game.Chat.Commands
{
    internal class ForgiveCommand : ChatCommand
    {
        public ForgiveCommand(EverybodyEdits game) : base(game, CommandAccess.CrewMember, "forgive", "unkick") { this.LimitArguments(1, "You must specify a player to forgive."); }
        protected override void OnExecute(Player player, string[] commandInput)
        {
            var target = commandInput[1].ToLower();

            if (target == player.Name)
            {
                this.SendSystemMessage(player, "You cannot forgive yourself.");
            }
            else
            {
                if (this.Game.ForgiveUser(target))
                {
                    this.SendSystemMessage(player, "The player has been forgiven");
                }
                else
                {
                    this.SendSystemMessage(player, "You cannot forgive a player who had not been kicked.");
                }
            }
        }
    }
}