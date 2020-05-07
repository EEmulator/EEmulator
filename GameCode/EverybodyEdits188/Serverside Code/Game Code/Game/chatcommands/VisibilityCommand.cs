using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.ChatCommands
{
    public class VisibilityCommand : ChatCommand
    {
        public VisibilityCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;
            this._game.Visible = commandInput[1].ToLower() == "true";
            this._game.BaseWorld.visible = this._game.Visible;
            this._game.BroadcastMessage(Message.Create("roomVisible", this._game.Visible));
            _player.Send("write", this._game.SystemName, "World is visible: " + this._game.Visible.ToString().ToUpper());
        }
    }
}