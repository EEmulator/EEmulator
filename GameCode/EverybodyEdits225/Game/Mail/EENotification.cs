using System;

namespace EverybodyEdits.Game.Mail
{
    public class EENotification
    {
        public EENotification()
        {
            Channel = "";
            PublishDate = DateTime.UtcNow;
            Key = "";
            Body = "";
            Title = "";
            RoomId = "";
            ImageUrl = "";
        }

        public string Channel { get; set; }
        public DateTime PublishDate { get; set; }
        public string Key { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public string RoomId { get; set; }
        public string ImageUrl { get; set; }
    }
}