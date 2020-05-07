namespace EverybodyEdits.Game.ChatCommands
{
    public class RespawnAllPlayersCommand : ChatCommand
    {
        public RespawnAllPlayersCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            this._game.RespawnAllPlayers();
        }
    }
}