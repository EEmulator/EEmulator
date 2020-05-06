using System;

namespace EverybodyEdits.Game
{
    public class Ban
    {
        public Ban(string userid, DateTime timestamp)
        {
            this.userid = userid;
            this.timestamp = timestamp;
        }

        public string userid;

        public DateTime timestamp;
    }
}
