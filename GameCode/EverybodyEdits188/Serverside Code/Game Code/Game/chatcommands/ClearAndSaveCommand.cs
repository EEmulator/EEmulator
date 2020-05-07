namespace EverybodyEdits.Game.ChatCommands
{
    public class ClearAndSaveCommand : ChatCommand
    {
        public ClearAndSaveCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;
            if (_player.canbemod) // only moderators can do this
            {
                this._game.BaseWorld.reset();
                this._game.Broadcast("clear", this._game.BaseWorld.width, this._game.BaseWorld.height);

                this._game.BaseWorld.save(true,
                    delegate { _player.Send("write", this._game.SystemName, "Room has been reset and saved"); });
            }
        }
    }
}