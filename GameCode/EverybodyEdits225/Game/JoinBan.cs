using System;

namespace EverybodyEdits.Game
{
    public class JoinBan
    {
        public JoinBan(Player player, DateTime timestamp)
        {
            this.UserId = player.ConnectUserId;
            this.Username = player.Name;
            this.Timestamp = timestamp;
        }
        public JoinBan(string userId, DateTime timestamp)
        {
            this.UserId = userId;
            this.Timestamp = timestamp;
        }

        public DateTime Timestamp { get; private set; }
        public string UserId { get; private set; }
        public string Username { get; private set; }
    }
}