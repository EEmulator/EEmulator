namespace EverybodyEdits.Game.ChatCommands
{
    public class KillEmAllCommand : ChatCommand
    {
        public KillEmAllCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;
            foreach (var p in this._game.Players)
            {
                if (!p.canbemod && !p.CanBeGuardian && !p.isgod) /* ignore moderators and editors ("gods") */
                {
                    this._game.Broadcast("kill", p.Id);
                }
            }
        }
    }
}