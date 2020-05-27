using System;
using System.Linq;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Mail
{
    internal class NotificationHelper
    {
        private const int MaxNotifications = 10;
        public const int MaxTotalNotifications = 50;

        public static void GetSubscriptions(BigDB bigDb, string connectUserId, Callback<string[]> callback)
        {
            bigDb.Load("Subscriptions", connectUserId, obj =>
            {
                var defaultChannels = new[] {"news", "creweverybodyeditsstaff"};
                if (obj == null)
                {
                    callback(defaultChannels);
                }
                else
                {
                    var subs = (from sub in obj where (bool) sub.Value select sub.Key).Concat(defaultChannels).ToArray();
                    callback(subs);
                }
            });
        }

        public static void AddSubscription(BigDB bigDb, string connectUserId, string channel,
            Callback<bool> callback = null)
        {
            bigDb.LoadOrCreate("Subscriptions", connectUserId, obj =>
            {
                if (!obj.GetBool(channel, false))
                {
                    obj.Set(channel, true);
                    obj.Save();

                    if (callback != null)
                    {
                        callback.Invoke(true);
                    }
                }
                else
                {
                    if (callback != null)
                    {
                        callback.Invoke(false);
                    }
                }
            });
        }

        public static void RemoveSubscription(BigDB bigDb, string connectUserId, string channel, Callback<bool> callback)
        {
            bigDb.Load("Subscriptions", connectUserId, obj =>
            {
                if (obj != null)
                {
                    if (obj.GetBool(channel, false))
                    {
                        obj.Set(channel, false);
                        obj.Save();
                        callback(true);
                    }
                    else
                    {
                        callback(false);
                    }
                }
                else
                {
                    callback(false);
                }
            });
        }

        public static void GetDismissedNotifications(BigDB bigDb, string connectUserId, Action<string[]> callback)
        {
            bigDb.Load("Dismisses", connectUserId, obj =>
            {
                if (obj == null)
                {
                    callback(new string[] {});
                }
                else
                {
                    if (!obj.Contains("Notifications"))
                    {
                        obj.Set("Notifications", new DatabaseArray());
                    }
                    var notifications = obj.GetArray("Notifications");

                    callback(notifications.Select(i => i.ToString()).ToArray());
                }
            });
        }

        public static void DismissNotification(BigDB bigDb, string connectUserId, string notificationId,
            EENotification[] ignoreList)
        {
            var ignoreIds = ignoreList.Select(i => i.Key).ToArray();

            bigDb.LoadOrCreate("Dismisses", connectUserId, obj =>
            {
                if (!obj.Contains("Notifications"))
                {
                    obj.Set("Notifications", new DatabaseArray());
                }

                var notifications = obj.GetArray("Notifications");
                notifications.Add(notificationId);

                // remove all unused notifications
                for (var i = 0; i < notifications.Count; i++)
                {
                    if (!ignoreIds.Contains(notifications[0]))
                    {
                        notifications.RemoveAt(i);
                    }
                }

                obj.Save();
            });
        }

        private static void GetNotifications(BigDB bigDb, string channel, Callback<EENotification[]> callback)
        {
            bigDb.LoadRange("Notifications", "ByChannel", new object[] {channel}, null, null, 1000, items =>
            {
                callback(DeleteNotifications(bigDb, items.Select(i => new EENotification
                {
                    Key = i.Key,
                    Channel = i.GetString("Channel"),
                    PublishDate = i.GetDateTime("PublishDate"),
                    Title = i.GetString("Title", ""),
                    RoomId = i.GetString("RoomId", ""),
                    ImageUrl = i.GetString("ImageUrl", ""),
                    Body = i.GetString("Body")
                }).ToArray()));
            });
        }

        public static void GetNotifications(BigDB bigDb, string[] channels, Callback<EENotification[]> callback)
        {
            if (channels.Length == 0)
            {
                callback(new EENotification[0]);
                return;
            }

            GetNotifications(bigDb, channels[0],
                nots =>
                {
                    GetNotifications(bigDb, channels.Skip(1).ToArray(),
                        nots2 => { callback(nots.Concat(nots2).ToArray()); });
                });
        }

        private static EENotification[] DeleteNotifications(BigDB bigDb, EENotification[] eeNotifications)
        {
            if (eeNotifications.Length <= MaxNotifications)
            {
                return eeNotifications;
            }
            bigDb.DeleteKeys("Notifications", eeNotifications.Skip(MaxNotifications).Select(n => n.Key).ToArray());
            return eeNotifications.Take(MaxNotifications).ToArray();
        }

        public static void PublishNotification(BigDB bigDb, EENotification eeNotification,
            Callback<DatabaseObject> callback)
        {
            var obj = new DatabaseObject()
                .Set("Channel", eeNotification.Channel)
                .Set("PublishDate", eeNotification.PublishDate)
                .Set("Title", eeNotification.Title)
                .Set("RoomId", eeNotification.RoomId)
                .Set("ImageUrl", eeNotification.ImageUrl)
                .Set("Body", eeNotification.Body);
            bigDb.CreateObject("Notifications", null, obj, callback ?? (db => { }));
        }
    }
}