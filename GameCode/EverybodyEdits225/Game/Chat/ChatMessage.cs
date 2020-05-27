namespace EverybodyEdits.Game.Chat
{
    public class ChatMessage
    {
        public ChatMessage(string senderName, string text, string senderConnectUserId, bool senderCanChat,
            uint senderChatColor)
        {
            this.SenderName = senderName;
            this.Text = text;
            this.SenderConnectUserId = senderConnectUserId;
            this.SenderCanChat = senderCanChat;
            this.SenderChatColor = senderChatColor;
        }

        public string Text { get; private set; }

        public string SenderName { get; private set; }
        public string SenderConnectUserId { get; private set; }
        public bool SenderCanChat { get; private set; }
        public uint SenderChatColor { get; private set; }
    }
}