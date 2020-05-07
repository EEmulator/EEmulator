namespace EverybodyEdits.Game.ChatCommands
{
    public class KickCommand : ChatCommand
    {
        public KickCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;

            var wherekicked = false;

            var reason = "";
            if (commandInput.Length > 2)
            {
                for (var a = 2; a < commandInput.Length; a++)
                {
                    reason += commandInput[a] + " ";
                }
            }

            foreach (var p in this._game.Players)
            {
                if (p.name.ToLower() == commandInput[1].ToLower())
                {
                    if (p.CanBeGuardian && (!_player.canbemod && !_player.CanBeGuardian))
                    {
                        _player.Send("write", this._game.SystemName, "You can not kick a Guardian.");
                    }
                    else if (p.canbemod && !_player.canbemod)
                    {
                        var pRole = p.canbemod ? "moderator" : "Guardian";
                        this._game.Broadcast("write", this._game.SystemName,
                            _player.name + " tried to kick a " + pRole + ", sadly it backfired and " + _player.name +
                            " was kicked.");
                        _player.Send("info", "Tsk tsk tsk",
                            "You tried to kick a " + pRole + ". Sadly it backfired and you were kicked.");
                        _player.Disconnect();
                    }
                    else
                    {
                        if (!wherekicked)
                        {
                            if (p.name == _player.name)
                            {
                                this._game.Broadcast("write", this._game.SystemName,
                                    _player.name + " is no more" + (reason.Trim() != "" ? " - " + reason : ""));
                            }
                            else
                            {
                                this._game.Broadcast("write", this._game.SystemName,
                                    _player.name + " kicked " + p.name + (reason.Trim() != "" ? " - " + reason : ""));
                            }
                        }
                        this._game.KickUser(p.ConnectUserId, 5);

                        p.Send("info", "You were kicked",
                            "You were kicked by " + _player.name.ToUpper() + ".\n" +
                            (reason.Trim() != "" ? "Reason: " + reason : ""));
                        p.Disconnect();
                    }
                    wherekicked = true;
                }
            }

            if (!wherekicked)
            {
                _player.Send("write", this._game.SystemName, "Unknown user " + commandInput[1]);
            }
        }
    }
}