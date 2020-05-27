using System;
using System.Linq;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Mail
{
    internal class MailHelper
    {
        private const int InboxSize = 20;

        public static void GetMail(BigDB bigDb, string connectUserId, Callback<EEMail[]> callback)
        {
            bigDb.LoadRange("Mails", "MyMails", new object[] { connectUserId }, null, null, 1000, objs =>
              {
                  callback(DeleteMails(bigDb, objs.Select(obj => new EEMail
                  {
                      Key = obj.Key,
                      From = obj.GetString("From"),
                      To = obj.GetString("To"),
                      Date = obj.GetDateTime("Date", default(DateTime)),
                      Subject = obj.GetString("Subject", ""),
                      Body = obj.GetString("Body", "")
                  }).ToArray()));
              });
        }

        public static void SendMail(BigDB bigDb, string from, string to, string subject, string body)
        {
            SendMail(bigDb, new EEMail
            {
                From = from,
                To = to,
                Subject = subject,
                Body = body,
                Date = DateTime.UtcNow
            }, o => { });
        }

        public static void DeleteMail(BigDB bigDb, string connectUserId, string mailId)
        {
            // Check if the user actually owns this mail
            GetMail(bigDb, connectUserId, mails =>
            {
                var mailToDelete = mails.SingleOrDefault(it => it.Key == mailId);
                if (mailToDelete != null)
                {
                    DeleteMail(bigDb, mailToDelete);
                }
            });
        }

        private static void SendMail(BigDB bigDb, EEMail mail, Callback<DatabaseObject> callback)
        {
            var obj = new DatabaseObject()
                .Set("From", mail.From)
                .Set("To", mail.To)
                .Set("Date", mail.Date)
                .Set("Subject", mail.Subject)
                .Set("Body", mail.Body);
            bigDb.CreateObject("Mails", null, obj, callback ?? (db => { }));

            // Delete mail if the inbox got too full
            GetMail(bigDb, mail.To, m => { });
        }

        private static void DeleteMail(BigDB bigDb, EEMail mail)
        {
            bigDb.DeleteKeys("Mails", mail.Key);
        }

        private static EEMail[] DeleteMails(BigDB bigDb, EEMail[] mail)
        {
            if (mail.Length <= InboxSize)
            {
                return mail;
            }
            bigDb.DeleteKeys("Mails", mail.Skip(InboxSize).Select(n => n.Key).ToArray());
            return mail.Take(InboxSize).ToArray();
        }
    }
}