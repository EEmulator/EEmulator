using System;

namespace EverybodyEdits.Game.Mail
{
    internal class EEMail
    {
        public string Key { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime Date { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}