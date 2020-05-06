using System;
using System.Text;
using PlayerIO.GameLibrary;

namespace MyGame
{
    [RoomType("FlixelWalkerFX3")]
    public class GameCode : Game<Player>
    {
        public override void GameStarted()
        {
            if (base.RoomData.ContainsKey("editkey"))
            {
                this.lockedroom = true;
                this.editkey = base.RoomData["editkey"];
                base.RoomData.Remove("editkey");
                base.RoomData.Add("needskey", "yep");
                base.RoomData.Save();
            }
            for (var i = 0; i < 200; i++)
            {
                this.world[i, 0] = 9;
                this.world[i, 199] = 9;
                this.world[0, i] = 9;
                this.world[199, i] = 9;
            }
            base.AddTimer(delegate
            {
                if (this.red && this.getTime() - this.redtime > 5000.0)
                {
                    base.Broadcast("show", new object[]
                    {
                        "red"
                    });
                    this.red = false;
                }
                if (this.green && this.getTime() - this.greentime > 5000.0)
                {
                    base.Broadcast("show", new object[]
                    {
                        "green"
                    });
                    this.green = false;
                }
                if (this.blue && this.getTime() - this.bluetime > 5000.0)
                {
                    base.Broadcast("show", new object[]
                    {
                        "blue"
                    });
                    this.blue = false;
                }
            }, 500);
        }

        public override void UserJoined(Player player)
        {
            if (this.editkey == "" || (player.JoinData.ContainsKey("editkey") && player.JoinData["editkey"] == this.editkey))
            {
                player.canEdit = true;
            }
            foreach (var player2 in base.Players)
            {
                if (player2 != player)
                {
                    player2.Send("add", new object[]
                    {
                        player.Id,
                        player.face,
                        player.x,
                        player.y
                    });
                }
            }
        }

        public override void UserLeft(Player player)
        {
            base.Broadcast("left", new object[]
            {
                player.Id
            });
        }

        public override void GotMessage(Player player, Message m)
        {
            var type = m.Type;
            switch (type)
            {
                case "red":
                    this.redtime = this.getTime();
                    if (!this.red)
                    {
                        base.Broadcast("hide", new object[]
                        {
                        "red"
                        });
                    }
                    this.red = true;
                    break;
                case "green":
                    this.greentime = this.getTime();
                    if (!this.green)
                    {
                        base.Broadcast("hide", new object[]
                        {
                        "green"
                        });
                    }
                    this.green = true;
                    break;
                case "blue":
                    this.bluetime = this.getTime();
                    if (!this.blue)
                    {
                        base.Broadcast("hide", new object[]
                        {
                        "blue"
                        });
                    }
                    this.blue = true;
                    break;
                case "god":
                    if (this.lockedroom && player.canEdit)
                    {
                        base.Broadcast("god", new object[]
                        {
                        player.Id,
                        m.GetBoolean(0U)
                        });
                    }
                    break;
                case "time":
                    player.Send("time", new object[]
                    {
                    m.GetDouble(0U),
                    this.getTime()
                    });
                    break;
                case "access":
                    if (m.GetString(0U) == this.editkey)
                    {
                        player.canEdit = true;
                        player.Send("access", new object[0]);
                    }
                    break;
                case "init":
                {
                    var message = Message.Create("init", new object[]
                    {
                    player.Id,
                    player.canEdit
                    });
                    var stringBuilder = new StringBuilder();
                    for (var i = 0; i < 200; i++)
                    {
                        for (var j = 0; j < 200; j++)
                        {
                            message.Add(this.world[i, j]);
                        }
                    }
                    player.Send(message);
                    break;
                }
                case "init2":
                    foreach (var player2 in base.Players)
                    {
                        if (player2 != player)
                        {
                            player.Send("add", new object[]
                            {
                            player2.Id,
                            player2.face,
                            player2.x,
                            player2.y
                            });
                        }
                    }
                    player.Send("k", new object[]
                    {
                    this.crownid
                    });
                    break;
                case "c":
                    if (player.canEdit)
                    {
                        if (m.GetInt(0U) >= 1 && m.GetInt(0U) <= 198 && m.GetInt(1U) <= 198 && ((this.editkey != "") ? (m.GetInt(1U) >= 1) : (m.GetInt(1U) >= 5)))
                        {
                            if (this.world[m.GetInt(1U), m.GetInt(0U)] != m.GetInt(2U))
                            {
                                this.world[m.GetInt(1U), m.GetInt(0U)] = m.GetInt(2U);
                                base.Broadcast(m);
                            }
                        }
                    }
                    break;
                case "k":
                    this.crownid = player.Id;
                    base.Broadcast("k", new object[]
                    {
                    player.Id
                    });
                    break;
                case "face":
                    player.face = m.GetInt(0U);
                    base.Broadcast("face", new object[]
                    {
                    player.Id,
                    m.GetInt(0U)
                    });
                    break;
                case "u":
                    player.x = m.GetDouble(0U);
                    player.y = m.GetDouble(1U);
                    base.Broadcast("u", new object[]
                    {
                    player.Id,
                    m.GetDouble(0U),
                    m.GetDouble(1U),
                    m.GetDouble(2U),
                    m.GetDouble(3U),
                    m.GetDouble(4U),
                    m.GetDouble(5U),
                    m.GetDouble(6U),
                    m.GetDouble(7U)
                    });
                    break;
            }
        }

        private double getTime()
        {
            return Math.Round((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
        }

        private int[,] world = new int[200, 200];

        private string editkey = "";

        private int crownid = -1;

        private bool lockedroom = false;

        private bool red = false;

        private double redtime;

        private bool green = false;

        private double greentime;

        private bool blue = false;

        private double bluetime;
    }
}
