using EverybodyEdits.Common;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Crews
{
    public class QueueItem
    {
        public readonly Message Message;

        public readonly CommonPlayer Player;

        public QueueItem(CommonPlayer player, Message message)
        {
            this.Player = player;
            this.Message = message;
        }
    }
}