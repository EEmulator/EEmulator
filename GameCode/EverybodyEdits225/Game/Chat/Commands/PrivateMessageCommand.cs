using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal class PrivateMessageCommand : ChatCommand
    {
        public PrivateMessageCommand(EverybodyEdits game)
            : base(game, CommandAccess.Public, "pm")
        {
        }

        protected override void OnExecute(Player player, string[] commandInput)
        {
            var message = this.GetMessage(commandInput, 2);
            if (message.Trim() == "")
            {
                return;
            }
            var target = commandInput[1].ToLower();
            this.SendMessage(player, target, message);
        }

        private string GetMessage(string[] commandInput, int start)
        {
            var message = "";
            if (commandInput.Length <= start)
            {
                return message;
            }

            for (var i = start; i < commandInput.Length; i++)
            {
                message += commandInput[i] + " ";
            }
            return message;
        }

        private void SendMessage(Player sender, string target, string message)
        {
            if (ChatUtils.IsSpam(sender, message, target))
            {
                return;
            }

            var sent = false;
            var triedToSendToSelf = false;

            foreach (var receiver in this.Game.FilteredPlayers.Where(receiver => receiver.Name.ToLower() == target))
            {
                if (sender.Id == receiver.Id)
                {
                    triedToSendToSelf = true;
                    continue;
                }

                if (sender.MutedUsers.Contains(receiver.ConnectUserId))
                {
                    this.SendSystemMessage(sender, "You can't send private messages to muted FilteredPlayers.");
                    return;
                }
                if (receiver.MutedUsers.Contains(sender.ConnectUserId))
                {
                    this.SendSystemMessage(sender, "Message couldn't be sent. Player has muted you.");
                    return;
                }

                if (sender.IsGuest && !(receiver.IsAdmin || receiver.IsModerator))
                {
                    this.SendSystemMessage(sender,
                        "Register to be able to chat or send private messages to regular users.");
                    return;
                }

                if (receiver.IsGuest && !(sender.IsAdmin || sender.IsModerator))
                {
                    this.SendSystemMessage(sender, "You can't send private messages to guests.");
                    return;
                }

                if (!sender.CanChat && !(receiver.IsAdmin || receiver.IsModerator))
                {
                    this.SendSystemMessage(sender,
                        "Sorry, you are not allowed to send private messages to regular users.");
                    return;
                }

                if (!receiver.CanChat && !receiver.IsGuest && (!sender.IsAdmin || !sender.IsModerator))
                {
                    this.SendSystemMessage(sender, "Sorry, this player is not allowed to chat.");
                    return;
                }

                receiver.SendMessage("write", "* " + sender.Name + " > you", message);
                if (!sent)
                {
                    sender.SendMessage("write", "* you > " + receiver.Name, message);
                }
                sent = true;
            }

            if (sent)
            {
                return;
            }

            this.SendSystemMessage(sender,
                triedToSendToSelf ? "You can't send private messages to yourself." : "No player found.");
        }
    }
}