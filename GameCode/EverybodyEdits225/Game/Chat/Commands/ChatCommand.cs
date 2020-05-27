using System.Linq;

namespace EverybodyEdits.Game.Chat.Commands
{
    internal abstract class ChatCommand
    {
        private readonly CommandAccess accessLevel;
        protected readonly EverybodyEdits Game;
        private readonly string[] labels;

        private int minArgs;
        private string notEnoughArgsError = "Too few arguments to execute the command.";

        protected ChatCommand(EverybodyEdits game, CommandAccess accessLevel, params string[] labels)
        {
            this.Game = game;
            this.accessLevel = accessLevel;
            this.labels = labels;
        }

        protected void LimitArguments(int limit, string errorString)
        {
            this.minArgs = limit;
            this.notEnoughArgsError = errorString;
        }

        public bool TryExecute(Player player, string[] commandInput)
        {
            if (!this.HasAccess(player))
            {
                return false;
            }

            if (labels.All(label => label != commandInput[0].ToLower()))
            {
                return false;
            }

            if (commandInput.Length <= minArgs)
            {
                this.SendSystemMessage(player, notEnoughArgsError);
            }
            else
            {
                OnExecute(player, commandInput);
            }

            return true;
        }

        protected abstract void OnExecute(Player player, string[] commandInput);

        protected void SendSystemMessage(Player player, string message, params object[] args)
        {
            this.SendSystemMessage(player, string.Format(message, args));
        }

        protected void SendSystemMessage(Player player, string message)
        {
            this.Game.SendSystemMessage(player, message);
        }

        protected void BroadcastSystemMessage(string message, params object[] args)
        {
            this.BroadcastSystemMessage(string.Format(message, args));
        }

        protected void BroadcastSystemMessage(string message)
        {
            this.Game.BroadcastMessage("write", ChatUtils.SystemName, message);
        }

        private bool HasAccess(Player player)
        {
            switch (this.accessLevel)
            {
                case CommandAccess.PlayerWithEdit:
                    return player.CanEdit || player.IsAdmin || player.IsModerator;

                case CommandAccess.CrewMember:
                    return (player.CanChangeWorldOptions && !this.Game.IsCampaign) || player.IsAdmin || player.IsModerator;

                case CommandAccess.Owner:
                    return (player.Owner && !this.Game.IsCampaign) || player.IsAdmin || player.IsModerator;

                case CommandAccess.Moderator:
                    return player.IsAdmin || player.IsModerator;

                case CommandAccess.Admin:
                    return player.IsAdmin;

                default:
                    return true;
            }
        }
    }
}