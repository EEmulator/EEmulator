namespace EverybodyEdits.Game.ChatCommands
{
    public class EditToggleCommand : ChatCommand
    {
        public EditToggleCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;

            var edit = (commandInput[0] == "/giveedit");

            if (commandInput[1].ToLower() == this._game.LevelOwnerName.ToLower())
            {
                _player.Send("write", this._game.SystemName, "You can not alter your own edit rights");
                return;
            }

            foreach (var p in this._game.Players)
            {
                if (p.name.ToLower() == commandInput[1].ToLower())
                {
                    this._game.setEditRigths(p, edit);
                    var write = (edit ? " can now" : " can no longer") + " edit this world.";
                    _player.Send("write", this._game.SystemName, p.name.ToUpper() + write);
                    p.Send("write", this._game.SystemName, "You " + write);
                    return;
                }
            }
        }
    }
}