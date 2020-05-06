using System;
using System.Security.Cryptography;
using System.Text;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Auth
{
    [RoomType("Auth89")]
    public class MatchMaker : Game<AuthPlayer>
    {
        public override void GameStarted()
        {
        }

        public override void UserJoined(AuthPlayer player)
        {
        }

        public override void AttemptGotMessage(BasePlayer player, Message message)
        {
            var type = message.Type;
            if (type != null)
            {
                if (type == "auth")
                {
                    var id = message.GetString(0U);
                    if (id.Substring(0, 5) == "armor")
                    {
                        player.Send("auth", new object[]
                        {
                            MatchMaker.Create(id, this.secret)
                        });
                    }
                }
            }
        }

        public override bool AllowUserJoin(AuthPlayer player)
        {
            return base.AllowUserJoin(player);
        }

        public static string Create(string userId, string sharedSecret)
        {
            var unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var hmac = new HMACSHA1(Encoding.UTF8.GetBytes(sharedSecret)).ComputeHash(Encoding.UTF8.GetBytes(unixTime + ":" + userId));
            return unixTime + ":" + MatchMaker.toHexString(hmac);
        }

        private static string toHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        private string secret = "everybodyeditsrocksyourworld$";
    }
}
