namespace EverybodyEdits.Game.ChatCommands
{
    internal class KickGuestsCommand : ChatCommand
    {
        public KickGuestsCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            foreach (var p in this._game.Players)
            {
                if (p.isguest)
                {
                    p.Disconnect();
                }
            }
        }
    }
}