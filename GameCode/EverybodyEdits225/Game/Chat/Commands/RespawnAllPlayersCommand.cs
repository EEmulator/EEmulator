namespace EverybodyEdits.Game.Chat.Commands
{
    internal class RespawnAllPlayersCommand : ChatCommand
    {
        public RespawnAllPlayersCommand(EverybodyEdits game)
            : base(game, CommandAccess.CrewMember, "respawnall")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            this.Game.RespawnAllPlayers();
        }
    }
}