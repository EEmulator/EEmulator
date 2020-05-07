using System;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Auth
{
    public class AuthPlayer : BasePlayer
    {
    }

    [RoomType("Auth188")]
    public class MatchMaker : Game<AuthPlayer>
    {
        private string secret = "everybodyeditsrocksyourworld$"; // was QuickFix_everybodyeditsrocksyourworld$ on server

        // This method is called when an instance of your the game is created
        public override void GameStarted()
        {
        }

        public override void UserJoined(AuthPlayer player)
        {
            /*if(player.ConnectUserId.IndexOf("armor") == 0) { 
				
				player.Send("auth", Create(player.ConnectUserId, secret));

			}else player.Disconnect();*/
        }

        public override void AttemptGotMessage(BasePlayer player, Message message)
        {
            switch (message.Type)
            {
                case "auth":
                {
                    Console.WriteLine("AUTH: " + message.GetString(0) + " - " + message.GetString(1));
                    var url = "https://services.armorgames.com/services/rest/v1/authenticate/user.json?user_id=" +
                              message.GetString(0) + "&auth_token=" + message.GetString(1) +
                              "&api_key=B84276AA-5397-4B89-A8BA-0F16FFB7566F";

                    this.PlayerIO.Web.Get(url, delegate (HttpResponse response)
                    {
                        Console.WriteLine("HTTP RESPONSE: " + response.StatusCode + " = " + response.Text);

                        var id = "";

                        if (response.Text.Length > 0)
                        {
                            var json_response = (Hashtable)new JSON().JsonDecode(response.Text);
                            //Console.WriteLine("ID: " + ((Hashtable)json_response["payload"])["username"]);

                            if (json_response.ContainsKey("payload"))
                            {
                                var payload = (Hashtable)json_response["payload"];
                                if (payload != null && payload.ContainsKey("username"))
                                {
                                    id = (string)payload["username"];
                                }
                            }
                        }
                        if (id == "")
                        {
                            player.Send("auth", "", "");
                        }
                        else
                        {
                            id = "armor" + id;
                            player.Send("auth", id, Create(id, this.secret));
                        }


                        //string id = message.GetString(0);
                        //if(id.Substring(0,5) == "armor"){
                        //    player.Send("auth", id, Create(id, secret));
                        //}
                    });

                    break;
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