namespace EverybodyEdits.Game.ChatCommands
{
    internal class PrivateMessageCommand : ChatCommand
    {
        public PrivateMessageCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;

            var message = "";
            if (commandInput.Length > 2)
            {
                for (var a = 2; a < commandInput.Length; a++)
                {
                    message += commandInput[a] + " ";
                }
            }

            var sent = false;

            foreach (var p in this._game.Players)
            {
                if (p.name.ToLower() == commandInput[1].ToLower() && p.name != _player.name)
                {
                    p.Send("write", "* " + _player.name + " > you", message);
                    player.Send("write", "* you > " + p.name, message);
                    sent = true;
                }
            }

            if (!sent)
            {
                player.Send("write", this._game.SystemName, "Unknown user.");
            }
        }
    }
}