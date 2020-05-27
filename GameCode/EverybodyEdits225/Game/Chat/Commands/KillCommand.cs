namespace EverybodyEdits.Game.Chat.Commands
{
    internal class KillCommand : ChatCommand
    {
        public KillCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "kill")
        {
            this.LimitArguments(1, "You must specify a player to kill.");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            foreach (var p in this.Game.Players)
            {
                if (p.Name.ToLower() == commandInput[1].ToLower())
                {
                    if (!p.IsInGodMode && !p.IsInModeratorMode && !p.IsInAdminMode)
                    {
                        this.Game.BroadcastMessage("kill", p.Id);
                    }
                }
            }
        }
    }
}