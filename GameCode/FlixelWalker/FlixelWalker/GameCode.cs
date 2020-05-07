using System;
using System.Text;
using PlayerIO.GameLibrary;

namespace MyGame
{
    [RoomType("FlixelWalker")]
    public class GameCode : Game<Player>
    {
        public override void GameStarted()
        {
            Console.WriteLine("Game is started: " + base.RoomId);
            for (int i = 0; i < 200; i++)
            {
                this.world[i, 0] = 5;
                this.world[i, 199] = 5;
                this.world[0, i] = 5;
                this.world[199, i] = 5;
            }
        }

        public override void GameClosed()
        {
            Console.WriteLine("RoomId: " + base.RoomId);
        }

        public override void UserJoined(Player player)
        {
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
            string type = m.Type;
            if (type != null)
            {
                if (!(type == "time"))
                {
                    if (!(type == "init"))
                    {
                        if (!(type == "change"))
                        {
                            if (!(type == "face"))
                            {
                                if (type == "update")
                                {
                                    player.x = m.GetDouble(0U);
                                    player.y = m.GetDouble(1U);
                                    base.Broadcast("update", new object[]
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
                                }
                            }
                            else
                            {
                                player.face = m.GetInt(0U);
                                base.Broadcast("face", new object[]
                                {
                                    player.Id,
                                    m.GetInt(0U)
                                });
                            }
                        }
                        else if (m.GetInt(0U) >= 1 && m.GetInt(0U) <= 198 && m.GetInt(1U) >= 5 && m.GetInt(1U) <= 198)
                        {
                            this.world[m.GetInt(1U), m.GetInt(0U)] = m.GetInt(2U);
                            base.Broadcast(m);
                        }
                    }
                    else
                    {
                        StringBuilder stringBuilder = new StringBuilder();
                        for (int i = 0; i < 200; i++)
                        {
                            if (i != 0)
                            {
                                stringBuilder.Append("\n");
                            }
                            for (int j = 0; j < 200; j++)
                            {
                                if (j != 0)
                                {
                                    stringBuilder.Append(",");
                                }
                                stringBuilder.Append(this.world[i, j]);
                            }
                        }
                        player.Send("init", new object[]
                        {
                            stringBuilder.ToString(),
                            player.Id
                        });
                        foreach (Player player2 in base.Players)
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
                    }
                }
                else
                {
                    player.Send("time", new object[]
                    {
                        m.GetDouble(0U),
                        this.getTime()
                    });
                }
            }
        }

        private double getTime()
        {
            return Math.Round((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
        }

        private int[,] world = new int[200, 200];
    }
}
