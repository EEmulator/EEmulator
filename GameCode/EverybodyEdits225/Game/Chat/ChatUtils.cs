using System.Text.RegularExpressions;
using System;

namespace EverybodyEdits.Game.Chat
{
    public class ChatUtils
    {
        public static string SystemName
        {
            get { return "* SYSTEM"; }
        }

        public static string RemoveBadCharacters(string text)
        {
            // Limit range to 31 - 255
            text = Regex.Replace(text, @"[^\x1F-\xFF]", "").Trim();

            // Remove del char 127
            text = Regex.Replace(text, @"[\x7F]", "").Trim();

            return text;
        }

        public static bool IsSpam(Player player, string text, string channel)
        {
            if (!player.Owner)
            {
                var repeatCount = player.RepeatChatCount(text, channel);
                if (repeatCount > 12)
                {
                    player.SendMessage("write", SystemName,
                        "Final Warning. You have said the same thing " + repeatCount + " times.");
                    player.ChatCoolDown = 10000;
                    return true;
                }
            }

            if (DateTime.UtcNow.Subtract(player.LastChat).TotalMilliseconds < player.ChatCoolDown)
            {
                // If we already said something in this channel
                if (player.LastChatChannels.Contains(channel))
                {
                    player.SendMessage("write", SystemName,
                        "You are trying to chat too fast, spamming the chat room is not nice!");
                    return true;
                }
            }
            else
            {
                player.LastChat = DateTime.UtcNow;
                player.LastChatChannels.Clear();
            }

            player.LastChatChannels.Add(channel);
            return false;
        }
    }
}