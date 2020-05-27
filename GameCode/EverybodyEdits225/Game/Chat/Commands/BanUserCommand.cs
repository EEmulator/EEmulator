using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class BanUserCommand : ChatCommand
    {
        public BanUserCommand(EverybodyEdits game)
            : base(game, CommandAccess.Moderator, "ban")
        {
            this.LimitArguments(1, "You must define an user to ban.");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            this.Game.PlayerIO.BigDB.Load("Usernames", commandInput[1].ToLower(), o =>
            {
                if (o == null || o.GetString("owner", null) == null)
                {
                    this.SendSystemMessage(player, "User {0} not found.", commandInput[1]);
                    return;
                }

                this.Game.PlayerIO.BigDB.Load("PlayerObjects", o.GetString("owner", "waggag"),
                    delegate(DatabaseObject user)
                    {
                        if (user == null)
                        {
                            this.SendSystemMessage(player, "Crap, something went horribly wrong!... tell chris!");
                            return;
                        }

                        if (user.GetBool("isModerator", false))
                        {
                            this.SendSystemMessage(player, "Dude, stop that!");
                            return;
                        }

                        user.Set("banned", true);
                        user.Set("ban_reason",
                            "Banned from chat by " + player.Name + " [" + string.Join(" ", commandInput) + "]");
                        user.Save(
                            () => this.SendSystemMessage(player, "Player {0} has been banned!", commandInput[1]));
                    });
            });
        }
    }
}