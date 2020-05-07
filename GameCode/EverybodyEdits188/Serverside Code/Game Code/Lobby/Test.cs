using System;
using System.Security.Cryptography;
using System.Text;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Auth
{
    [RoomType("Test")]
    public class Test : Game<AuthPlayer>
    {
        // This method is called when an instance of your the game is created

        public Random rnd = new Random();

        public override void GameStarted()
        {
            this.PreloadPlayerObjects = true;
        }

        public override void UserJoined(AuthPlayer player)
        {
            //	compareObjects(player.PlayerObject, player.PlayerObject);
            var ready = true;
            this.AddTimer(delegate
            {
                if (ready)
                {
                    ready = false;


                    this.setRandom(player);

                    this.setRandom(player);

                    this.setRandom(player);
                    player.PlayerObject.Save(
                        delegate
                        {
                            this.PlayerIO.BigDB.Load("PlayerObjects", player.ConnectUserId,
                                delegate(DatabaseObject clone)
                                {
                                    ready = this.compareObjects(player.PlayerObject, clone);
                                });
                        });
                }
            }, 100);
        }

        private void setRandom(AuthPlayer player)
        {
            switch (this.rnd.Next(5))
            {
                default:
                {
                    player.PlayerObject.Set("crap" + this.rnd.Next(5), this.rnd.Next(100000));
                    return;
                }
                case 1:
                {
                    player.PlayerObject.Set("crap" + this.rnd.Next(5), "String " + this.rnd.Next(100000));
                    return;
                }
                case 2:
                {
                    player.PlayerObject.Set("crap" + this.rnd.Next(5), this.rnd.Next(1) == 0);
                    return;
                }
                case 3:
                {
                    player.PlayerObject.Set("crap" + this.rnd.Next(5), this.rnd.NextDouble());
                    return;
                }
                case 4:
                {
                    var tst = new DatabaseArray();
                    for (var i = 0; i < this.rnd.Next(50); i++)
                    {
                        tst.Add(this.rnd.Next());
                    }
                    player.PlayerObject.Set("crap" + this.rnd.Next(5), tst);
                    return;
                }
            }
        }

        public override void UserLeft(AuthPlayer player)
        {
        }

        public override void AttemptGotMessage(BasePlayer player, Message message)
        {
            //	player.PlayerObject.Set("test", "ewfwefwefwef");
        }

        private bool compareObjects(DatabaseObject a, DatabaseObject b)
        {
            if (a.Count != b.Count)
            {
                Console.WriteLine("Error occoured\n" + a + "\n" + b);
                return false;
            }

            if (a.ToString() != b.ToString())
            {
                Console.WriteLine("Error occoured\n" + a + "\n" + b);
                return false;
            }

            Console.WriteLine("Everything is fine and dandy!");
            return true;
        }

        public override bool AllowUserJoin(AuthPlayer player)
        {
            return base.AllowUserJoin(player);
        }

        public static string Create(string userId, string sharedSecret)
        {
            var unixTime = (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
            var hmac =
                new HMACSHA1(Encoding.UTF8.GetBytes(sharedSecret)).ComputeHash(
                    Encoding.UTF8.GetBytes(unixTime + ":" + userId));
            return unixTime + ":" + toHexString(hmac);
        }

        private static string toHexString(byte[] bytes)
        {
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
    }
}