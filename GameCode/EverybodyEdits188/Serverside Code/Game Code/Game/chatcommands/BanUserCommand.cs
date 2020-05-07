using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.ChatCommands
{
    public class BanUserCommand : ChatCommand
    {
        public BanUserCommand(EverybodyEdits game)
            : base(game)
        {
        }

        public override void resolve(Player player, string[] commandInput)
        {
            var _player = player;
            this.banUser(_player, commandInput);
        }

        protected void banUser(Player player, string[] commandInput)
        {
            if (!player.canbemod) return;

            if (commandInput.Length < 2)
            {
                player.Send("write", this._game.SystemName, "You must define a user to ban");
            }


            this._game.PlayerIO.BigDB.Load("usernames", commandInput[1], delegate(DatabaseObject o)
            {
                if (o == null || o.GetString("owner", null) == null)
                {
                    player.Send("write", this._game.SystemName, "User " + commandInput[1] + " not found");
                    return;
                }

                this._game.PlayerIO.BigDB.Load("PlayerObjects", o.GetString("owner", "waggag"),
                    delegate(DatabaseObject user)
                    {
                        if (o == null)
                        {
                            player.Send("write", this._game.SystemName,
                                "Crap, something went horriably wrong!... tell chris!");
                            return;
                        }

                        if (user.GetBool("isModerator", false))
                        {
                            player.Send("write", this._game.SystemName, "Dude, stop that!");
                        }
                        user.Set("banned", true);
                        user.Set("ban_reason",
                            "Banned from chat by " + player.name + " [" + string.Join(" ", commandInput) + "]");
                        user.Save(
                            delegate
                            {
                                player.Send("write", this._game.SystemName,
                                    "Player " + commandInput[1] + " has been banned!");
                            });
                    });
            });
        }
    }
}