using System.Linq;
using System;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class NotifyCommand : ChatCommand
    {
        public NotifyCommand(EverybodyEdits game) : base(game, CommandAccess.Admin, "notify") {
            this.LimitArguments(1, "You must enter a message.");
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var message = string.Join(" ", commandInput.Skip(1));
            if (string.IsNullOrWhiteSpace(message)) {
                this.SendSystemMessage(player, "You cannot create an empty notifiication.");
                return;
            }

            this.Game.PlayerIO.BigDB.LoadOrCreate("Config", "Notification", value => {
                value.Set("Header", "Notification from " + player.Name.ToUpper());
                value.Set("Body", message);

                value.Set("EndDate", DateTime.UtcNow.AddMinutes(5));
                value.Save(() => {
                    this.SendSystemMessage(player, "Message created! Players will now start to see it.");

                    this.Game.NotifyWorld(true);
                }, error => this.SendSystemMessage(player, "Something went wrong! " + error.Message));
            }, error => this.SendSystemMessage(player, "Something went wrong! " + error.Message));
        }
    }
}