namespace EverybodyEdits.Game.ChatCommands
{
    public class ChatCommand
    {
        protected EverybodyEdits _game;

        public ChatCommand(EverybodyEdits game)
        {
            this._game = game;
        }

        public virtual void resolve(Player player, string[] commandInput)
        {
        }
    }
}