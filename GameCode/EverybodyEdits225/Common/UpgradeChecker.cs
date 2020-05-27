using System;
using System.Linq;
using PlayerIO.GameLibrary;
using PlayerIO.GameLibrary.Internal;

namespace EverybodyEdits.Common
{
    public class UpgradeChecker<TPlayer> where TPlayer : BasePlayer, new()
    {
        private readonly Game<TPlayer> game;

        public bool Upgrade { get; private set; }
        public bool UpgradeWarning { get; private set; }
        public bool SentWarning { get; set; }
        public bool RepeatWarning { get; private set; }
        private DateTime upgradeTime;

        public UpgradeChecker(Game<TPlayer> game)
        {
            this.game = game;
            this.game.AddTimer(this.CheckVersion, 10000);
        }

        public void CheckVersion()
        {
            this.game.PlayerIO.BigDB.Load("Config", "config", delegate(DatabaseObject o)
            {
                if (Config.Version < o.GetInt("version", Config.Version))
                {
                    this.Upgrade = true;
                }

                this.upgradeTime = o.GetDateTime("scheduled", DateTime.UtcNow);
                if (DateTime.UtcNow < this.upgradeTime)
                {
                    this.UpgradeWarning = true;
                }

                if (Config.Version < o.GetInt("nextversion", Config.Version))
                {
                    this.RepeatWarning = true;
                }

                if (!this.Upgrade) return;

                foreach (var p in this.game.Players)
                {
                    p.Send("upgrade");
                    p.Disconnect();
                }
            });
        }

        public void SendUpdateMessage(CommonPlayer player)
        {
            var time = this.upgradeTime.AddHours(-player.TimeZone);
            player.Send("write", "* WORLD",
                string.Format("Scheduled update on {0:dd/MM/yy} starting {1:HH:mm:ss}. ({2} from now)",
                    time,
                    time,
                    ToPrettyFormat(this.upgradeTime - DateTime.UtcNow)));
        }

        // http://codereview.stackexchange.com/questions/24995/convert-timespan-to-readable-text
        private static string ToPrettyFormat(TimeSpan timeSpan)
        {
            var dayParts = new[] { GetDays(timeSpan), GetHours(timeSpan), GetMinutes(timeSpan) }
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            var numberOfParts = dayParts.Length;

            return numberOfParts < 2
                ? (dayParts.FirstOrDefault() ?? string.Empty)
                : string.Join(", ", dayParts, 0, numberOfParts - 1) + " and " + dayParts[numberOfParts - 1];
        }

        private static string GetMinutes(TimeSpan timeSpan)
        {
            if (timeSpan.Minutes == 0) return string.Empty;
            if (timeSpan.Minutes == 1) return "a minute";
            return timeSpan.Minutes + " minutes";
        }

        private static string GetHours(TimeSpan timeSpan)
        {
            if (timeSpan.Hours == 0) return string.Empty;
            if (timeSpan.Hours == 1) return "an hour";
            return timeSpan.Hours + " hours";
        }

        private static string GetDays(TimeSpan timeSpan)
        {
            if (timeSpan.Days == 0) return string.Empty;
            if (timeSpan.Days == 1) return "a day";
            return timeSpan.Days + " days";
        }
    }
}
