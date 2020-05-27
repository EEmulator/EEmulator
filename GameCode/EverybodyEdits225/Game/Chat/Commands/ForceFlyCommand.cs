namespace EverybodyEdits.Game.Chat.Commands
{
    internal class ForceFlyCommand : ChatCommand
    {
        public ForceFlyCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "forcefly")
        {
            this.LimitArguments(2, "Please specify a player and whether to force fly mode or not (true/false).");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            foreach (var p in this.Game.Players)
            {
                if (p.Name.ToLower() != commandInput[1].ToLower())
                {
                    continue;
                }

                var forceFly = commandInput[2].ToLower() == "true";
                this.Game.BroadcastMessage("god", p.Id, forceFly);
                p.IsInGodMode = forceFly;
                break;
            }
        }
    }
}