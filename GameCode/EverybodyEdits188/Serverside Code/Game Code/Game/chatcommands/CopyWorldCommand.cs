namespace EverybodyEdits.Game.ChatCommands
{
    public class CopyWorldCommand : ChatCommand
    {
        public CopyWorldCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;
            this._game.copyLevel(_player);
        }
    }
}