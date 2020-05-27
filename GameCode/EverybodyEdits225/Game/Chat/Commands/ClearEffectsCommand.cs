namespace EverybodyEdits.Game.Chat.Commands
{
    internal class ClearEffectsCommand : ChatCommand
    {
        public ClearEffectsCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "cleareffects", "ce")
        {
            this.LimitArguments(1, "You must specify a player to remove all effects from");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var username = commandInput[1];

            foreach (var pl in Game.Players)
            {
                if (pl.Name.ToLower() != username.ToLower())
                {
                    continue;
                }

                var playerEffects = pl.GetEffects();
                foreach (var ef in playerEffects)
                {
                    pl.RemoveEffect(ef.Id);
                    this.Game.BroadcastMessage("effect", pl.Id, (int)ef.Id, false);
                }

                this.SendSystemMessage(player, "All effects were removed from " + username.ToUpper());
                break;
            }
        }
    }
}