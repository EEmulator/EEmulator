using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby
{
    public class QueueItem
    {
        public QueueItem(int id, string connectUserId, string method, Message m)
        {
            this.id = id;
            this.connectUserId = connectUserId;
            this.method = method;
            this.message = m;
        }

        public int id;

        public string connectUserId;

        public string method;

        public Message message;
    }
}
