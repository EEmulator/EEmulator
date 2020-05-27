using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using EverybodyEdits.Game.Chat.Commands;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Chat
{
    public class ChatHandler
    {
        private readonly string[] autoTexts =
        {
            "Left.",
            "Hi.",
            "Goodbye.",
            "Help me!",
            "Thank you.",
            "Follow me.",
            "Stop!",
            "Yes.",
            "No.",
            "Right."
        };

        private readonly Queue<ChatMessage> chatMessages = new Queue<ChatMessage>();
        private readonly EverybodyEdits game;
        private List<ChatCommand> chatCommands;

        public ChatHandler(EverybodyEdits game)
        {
            this.game = game;
            this.CreateChatCommands();
        }

        public List<ChatMessage> LastChatMessages
        {
            get { return this.chatMessages.ToList(); }
        }

        private void CreateChatCommands()
        {
            this.chatCommands = new List<ChatCommand>
            {
                new BanIpCommand(game),
                new BanUserCommand(game),
                new BgColorCommand(game),
                new CheckIpCommand(game),
                new ClearCommand(game),
                new ClearEffectsCommand(game),
                new CopyWorldCommand(game),
                new EditToggleCommand(game),
                new EffectCommand(game),
                new ForceFlyCommand(game),
                new ForgiveCommand(game),
                new GenCouponCommand(game),
                new HelpCommand(game),
                new HideLobbyCommand(game),
                new KickCommand(game),
                new KillAllCommand(game),
                new KillCommand(game),
                new ListPortalsCommand(game),
                new LoadLevelCommand(game),
                new MuteCommand(game),
                new MuteKickCommand(game),
                new NameCommand(game),
                new NotifyCommand(game),
                new PlayerInfoCommand(game),
                new PrivateMessageCommand(game),
                new ReportCommand(game),
                new ResetAllCommand(game),
                new ResetCommand(game),
                new ResetProgressCommand(game),
                new ResetGlobalSwitchesCommand(game),
                new RespawnPlayerCommand(game),
                new RespawnAllPlayersCommand(game),
                new SetCrownCommand(game),
                new SetTeamCommand(game),
                new StealthCommand(game),
                new TeleportCommand(game),
                new TempBanCommand(game),
                new ToggleGodModeCommand(game),
                new UnMuteCommand(game),
                new VisibilityCommand(game),
                new ArtContestCommand(game)
            };
        }

        public void HandleSay(Player player, Message m)
        {
            if (m.Count == 0) return;

            var text = ChatUtils.RemoveBadCharacters(m.GetString(0));

            if (text.Length > 13 && Regex.Matches(text, @"\p{Lu}").Count > text.Length / 2 + 7)
            {
                text = text.Substring(0, 1).ToUpper() + text.Substring(1).ToLower();
            }

            if (text.Trim() == "" || text.Length > 140 && !player.IsAdmin)
            {
                return;
            }

            if (text.StartsWith("/"))
            {
                if (!this.HandleCommand(player, text))
                {
                    player.SendMessage("info2", "System Message", "Unknown command or you don't have command access.");
                }

                return;
            }

            if (player.Stealthy)
            {
                return;
            }

            if (player.IsGuest)
            {
                player.SendMessage("info", "Info", "Sorry, you need to be a registered user to chat.");
                return;
            }

            if (ChatUtils.IsSpam(player, text, string.Empty))
            {
                return;
            }

            var friendsonline = false;
            var adminOnline = false;
            var modOnline = false;

            // Handle all other players chat roles
            foreach (var p in this.game.Players)
            {
                var isFriend = p.HasFriend(player.ConnectUserId);
                friendsonline = friendsonline || isFriend;
                adminOnline = adminOnline || p.IsAdmin;
                modOnline = modOnline || p.IsModerator;

                if (p.Id == player.Id)
                {
                    continue;
                }

                if ((player.CanChat && p.CanChat) || isFriend || p.IsAdmin || player.IsAdmin || p.IsModerator ||
                    player.IsModerator)
                {
                    // Check if player is in muted list of p, if he is we dont want to send the message
                    if (!p.MutedUsers.Contains(player.ConnectUserId))
                    {
                        p.SendMessage("say", player.Id, text);
                    }
                }
                else if (!p.CanChat)
                {
                    // The empty string is sent so that the UI can display a small bubble indicating that the user says something, but not what he is saying
                    p.SendMessage("say", player.Id, string.Empty);
                }
            }

            // Handle current players chat roles
            if (!player.CanChat && !friendsonline && !adminOnline && !modOnline)
            {
                player.SendMessage("info", "Sorry, this account is not verified for chatting.");
            }
            else
            {
                player.SendMessage("say", player.Id, text);

                if (this.chatMessages.Count >= 40)
                {
                    this.chatMessages.Dequeue();
                }

                this.chatMessages.Enqueue(new ChatMessage(player.Name, text, player.ConnectUserId, player.CanChat,
                    player.ChatColor));
            }
        }

        public void HandleAutoSay(Player player, Message m)
        {
            var offset = 1;
            try
            {
                offset = m.GetInt(0);
            }
            catch (PlayerIOError error)
            {
                this.game.PlayerIO.ErrorLog.WriteError(error.ToString(),
                    "HandleAutoSay did not recieve int. Got: " + m.GetString(0), "", new Dictionary<string, string>());
            }

            if (offset < 0 || offset >= autoTexts.Length)
            {
                return;
            }

            if (DateTime.UtcNow.Subtract(player.LastChat).TotalMilliseconds < 500)
            {
                player.SendMessage("write", ChatUtils.SystemName,
                    "You are trying to chat too fast, spamming the chat is not nice!");
                return;
            }
            player.LastChat = DateTime.UtcNow;

            this.game.BroadcastMessage("autotext", player.Id, autoTexts[offset]);
        }

        public void SendOldChat(Player player)
        {
            var messages = chatMessages.Where(message =>
                message != null &&
                (player.ConnectUserId == message.SenderConnectUserId ||
                player.HasFriend(message.SenderConnectUserId) ||
                (player.CanChat && message.SenderCanChat))).Skip(Math.Max(0, chatMessages.Count - 10));

            foreach (var message in messages)
            {
                player.SendMessage("say_old", message.SenderName,
                    message.Text, player.HasFriend(message.SenderConnectUserId),
                    message.SenderChatColor);
            }
        }

        private bool HandleCommand(Player player, string text)
        {
            var args = text.Substring(1).Split(' ');

            return chatCommands.Any(command => command.TryExecute(player, args));
        }
    }
}