namespace EverybodyEdits.Game.ChatCommands
{
    public class KillCommand : ChatCommand
    {
        public KillCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;
            foreach (var p in this._game.Players)
            {
                if (p.name.ToLower() == commandInput[1].ToLower())
                {
                    if (p.canbemod && !_player.canbemod)
                        /* If the player isn't moderator and tries to kill a moderator, the command backfires */
                    {
                        _player.Send("info", "Tsk tsk tsk",
                            "You tried to kill a moderator. Don't you know we are immortal?");
                        this._game.Broadcast("kill", _player.Id);
                        return;
                    }
                    if (!p.isgod) /* Ignore editors ("gods") */
                    {
                        this._game.Broadcast("kill", p.Id);
                    }
                    return;
                }
            }
        }
    }
}