using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby
{
    public class QueueItem
    {
        public QueueItem(int id, string connectUserId, string method, Message m)
        {
            this.Id = id;
            this.ConnectUserId = connectUserId;
            this.Method = method;
            this.Message = m;
        }

        public string ConnectUserId { get; private set; }
        public int Id { get; private set; }
        public Message Message { get; private set; }
        public string Method { get; private set; }
    }
}