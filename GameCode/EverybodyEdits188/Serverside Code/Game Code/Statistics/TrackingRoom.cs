using System;
using System.Collections.Generic;
using EverybodyEdits.Game;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Statistics
{
    public class StatsPlayer : BasePlayer
    {
    }

    [RoomType("Tracking188")]
    public class TrackingRoom : Game<StatsPlayer>
    {
        private PlayerStats playerstats;
        private List<QueueItem> queue = new List<QueueItem>();

        public override void GameStarted()
        {
            Console.WriteLine("GameStarted: TRACKING");
        }

        public override bool AllowUserJoin(StatsPlayer player)
        {
            return true;
        }

        // This method is called whenever a player joins the game
        public override void UserJoined(StatsPlayer player)
        {
            //Console.WriteLine("UserJoined: " + (player.JoinData.ContainsKey("statskey")? player.JoinData["statskey"]: "(no key)"));

            //// If pageview
            //if (player.JoinData.ContainsKey("pageview"))
            //{
            //    Console.WriteLine("UserJoined for pageview");
            //    playerstats = new PlayerStats(PlayerIO, true);
            //    return;
            //}

            //// If returning guest
            //String statskey = player.JoinData.ContainsKey("statskey") ? player.JoinData["statskey"] : null;
            //playerstats = new PlayerStats(PlayerIO, statskey != null? statskey: player.isguest ? null: player.ConnectUserId, delegate()
            //{
            //    Console.WriteLine("Stats ready: " + playerstats.Key);
            //    if (player.isguest) player.Send("statskey", playerstats.Key);
            //    EmptyQueue();                
            //});
            this.playerstats = new PlayerStats(this.PlayerIO);
        }

        // This method is called when a player leaves the game
        public override void UserLeft(StatsPlayer player)
        {
            this.playerstats.HandleMessage("sessionend");
            this.playerstats.Save();
        }


        public override void GotMessage(StatsPlayer player, Message m)
        {
            Console.WriteLine("GotMessage:  " + m.Type);
            switch (m.Type)
            {
                case "firstpageload":
                {
                    this.playerstats.isFirstVisit = true;
                    break;
                }
                case "activateUser":
                {
                    // Can be either "guest","guest_return" or "registered". 
                    var activation_type = m.GetString(0);
                    var id = m.Count > 1 ? m.GetString(1) : null;

                    this.playerstats.activate(id, delegate
                    {
                        Console.WriteLine("Stats ready: " + this.playerstats.Key);
                        if (activation_type == "guest") player.Send("statskey", this.playerstats.Key);
                        this.playerstats.HandleMessage("sessionstart");
                        this.EmptyQueue();
                    });

                    break;
                }
                case "playerStats":
                {
                    this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
                    break;
                }
            }
        }

        public void AddToQueue(int id, string conncetUserId, string method, Message message)
        {
            Console.WriteLine("add to que: " + message.GetString(0));
            this.queue.Add(new QueueItem(id, conncetUserId, method, message));
            this.EmptyQueue();
        }

        public void EmptyQueue()
        {
            //if (playerstats.isReady)
            //{
            var return_to_que = new List<QueueItem>();
            foreach (var item in this.queue)
            {
                foreach (var p in this.Players)
                {
                    if (p.Id == item.id && p.ConnectUserId == item.connectUserId)
                    {
                        switch (item.method)
                        {
                            case "playerStats":
                            {
                                var props = new object[item.message.Count];
                                for (uint i = 0; i < item.message.Count; i++)
                                {
                                    Console.Write(item.message[i] + ", ");
                                    props[i] = item.message[i];
                                }
                                if (!this.playerstats.HandleMessage(props))
                                {
                                    return_to_que.Add(item);
                                }
                                break;
                            }
                        }
                    }
                }
            }
            this.queue = return_to_que;
            //}
        }
    }

    public class QueueItem
    {
        public string connectUserId;
        public int id;
        public Message message;
        public string method;

        public QueueItem(int id, string connectUserId, string method, Message m)
        {
            this.id = id;
            this.connectUserId = connectUserId;
            this.method = method;
            this.message = m;
        }
    }
}