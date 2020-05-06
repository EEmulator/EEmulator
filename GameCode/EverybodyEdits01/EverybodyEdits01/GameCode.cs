using System;
using System.Collections.Generic;
using System.Text;
using PlayerIO.GameLibrary;

namespace MyGame
{
	[RoomType("FlixelWalkerFX19")]
	public class GameCode : Game<Player>
	{
		public override void GameStarted()
		{
			base.PreloadPlayerObjects = true;
			base.PreloadPayVaults = true;
			string text = "acdefghijnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
			Random random = new Random();
			this.editchar = ".";
			for (int i = 0; i < random.Next(1, 3); i++)
			{
				this.editchar += text[random.Next(0, text.Length)].ToString();
			}
			if (base.RoomData.ContainsKey("editkey"))
			{
				this.lockedroom = true;
				this.editkey = base.RoomData["editkey"];
				base.RoomData.Remove("editkey");
				base.RoomData["needskey"] = "yep";
				base.RoomData["plays"] = "0";
				base.RoomData["rating"] = "0";
				base.RoomData.Save();
			}
			if (base.RoomData.ContainsKey("owned"))
			{
				this.owned = true;
				this.lockedroom = true;
				this.editkey = "h9f23h9g9hwqehweghwepogwepoghwpoh23tpnwbpihnwgpoj";
				base.RoomData["needskey"] = "yep";
				base.RoomData["plays"] = "0";
				base.RoomData["rating"] = "0";
				base.RoomData["name"] = "My Beta World";
				base.RoomData.Save();
				base.PlayerIO.BigDB.LoadOrCreate("Worlds", base.RoomId, delegate(DatabaseObject o)
				{
					lock (this)
					{
						this.ownedWorld = o;
						if (this.ownedWorld.Contains("world"))
						{
							this.unserializeWorld(this.ownedWorld.GetBytes("world", new byte[40000]));
							this.plays = this.ownedWorld.GetInt("plays", 1);
							base.RoomData["plays"] = this.plays.ToString();
							base.RoomData["name"] = this.ownedWorld.GetString("name", "My Beta World");
							base.RoomData.Save();
						}
						this.ready = true;
						foreach (Player player in base.Players)
						{
							if (player.ready)
							{
								this.sendInitMessage(player);
							}
						}
					}
				});
			}
			for (int i = 0; i < 200; i++)
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
				foreach (Player player in base.Players)
				{
					if (player.cheat > 0)
					{
						player.cheat--;
					}
				}
				if (this.die)
				{
					foreach (Player player in base.Players)
					{
						player.Disconnect();
					}
				}
			}, 500);
		}

		public override void UserJoined(Player player)
		{
			player.PlayerObject.Set("Online", true);
			player.PlayerObject.Save();
			if (this.editkey == "" || (player.JoinData.ContainsKey("editkey") && player.JoinData["editkey"] == this.editkey))
			{
				player.canEdit = true;
			}
			player.haveSmileyPackage = (player.PlayerObject.GetBool("haveSmileyPackage", false) || player.PayVault.Has("pro"));
			player.canbemod = player.PlayerObject.GetBool("isModerator", false);
			player.room0 = player.PlayerObject.GetString("room0", "");
			if (this.owned && base.RoomId == player.room0 && player.haveSmileyPackage)
			{
				player.canEdit = true;
				player.owner = true;
			}
			foreach (Player player2 in base.Players)
			{
				if (player2 != player)
				{
					player2.Send("add", new object[]
					{
						player.Id,
						player.face,
						player.x,
						player.y,
						player.isgod,
						player.ismod
					});
				}
			}

			//if (!this.ips.ContainsKey(player.IPAddress.Address))
			//{
			//	this.ips.Add(player.IPAddress.Address, -1);
				this.plays++;
				this.sessionplays++;
				if (this.sessionplays % 15 == 0 && this.owned && this.ownedWorld != null)
				{
					if (this.sessionplays > 10)
					{
						this.ownedWorld.Set("plays", this.plays);
					}
					this.ownedWorld.Save();
				}
				base.RoomData["plays"] = this.plays.ToString();
				base.RoomData.Save();
			//}
		}

		public override void GameClosed()
		{
			if (this.owned && this.ownedWorld != null)
			{
				if (this.sessionplays > 10)
				{
					this.ownedWorld.Set("plays", this.plays);
				}
				this.ownedWorld.Save();
			}
		}

		public override void UserLeft(Player player)
		{
			lock (this)
			{
				base.Broadcast("left", new object[]
				{
					player.Id
				});
			}
		}

		public override void GotMessage(Player player, Message m)
		{
			string type = m.Type;
			switch (type)
			{
			case "save":
				if (this.owned && player.owner && this.ownedWorld != null)
				{
					this.ownedWorld.Set("world", this.serializeWorld());
					if (this.sessionplays > 10)
					{
						this.ownedWorld.Set("plays", this.plays);
					}
					this.ownedWorld.Set("name", base.RoomData["name"]);
					this.ownedWorld.Save(delegate()
					{
						player.Send("saved", new object[0]);
					});
				}
				return;
			case "name":
				if (this.owned && player.owner && this.ownedWorld != null)
				{
					base.RoomData["name"] = m.GetString(0U);
					base.RoomData.Save();
					this.ownedWorld.Set("name", base.RoomData["name"]);
					this.ownedWorld.Save();
				}
				return;
			case "key":
				if (this.owned && player.owner && this.ownedWorld != null)
				{
					this.editkey = m.GetString(0U);
					foreach (Player player2 in base.Players)
					{
						if (player2.canEdit)
						{
							if (!player2.owner)
							{
								player2.canEdit = false;
							}
							player2.Send("lostaccess", new object[0]);
						}
					}
				}
				return;
			case "kill":
				if (player.ismod)
				{
					this.die = true;
				}
				return;
			case "mod":
				if (player.canbemod)
				{
					player.canEdit = true;
					player.Send("access", new object[0]);
					base.Broadcast("mod", new object[]
					{
						player.Id
					});
					player.ismod = true;
				}
				return;
			case "rate":
			{
				int @int = m.GetInt(0U);
				if (@int >= 0 && @int <= 5)
				{
					this.ips[player.IPAddress.Address] = @int;
					decimal d = 0m;
					foreach (long key in this.ips.Keys)
					{
						d += this.ips[key];
					}
					d /= this.ips.Keys.Count;
					base.RoomData["rating"] = Math.Round(d * 2m).ToString();
					base.RoomData.Save();
				}
				return;
			}
			case "god":
				if (this.lockedroom && player.canEdit)
				{
					base.Broadcast("god", new object[]
					{
						player.Id,
						m.GetBoolean(0U)
					});
					player.isgod = m.GetBoolean(0U);
				}
				return;
			case "time":
				player.Send("time", new object[]
				{
					m.GetDouble(0U),
					this.getTime()
				});
				return;
			case "access":
				if (m.Count != 0U && m.GetString(0U) == this.editkey)
				{
					player.canEdit = true;
					player.Send("access", new object[0]);
				}
				return;
			case "init":
				if (this.owned && !this.ready)
				{
					player.ready = true;
				}
				else
				{
					this.sendInitMessage(player);
				}
				return;
			case "init2":
				foreach (Player player2 in base.Players)
				{
					if (player2 != player)
					{
						player.Send("add", new object[]
						{
							player2.Id,
							player2.face,
							player2.x,
							player2.y,
							player2.isgod,
							player2.ismod
						});
					}
				}
				player.Send("k", new object[]
				{
					this.crownid
				});
				return;
			case "m":
			{
				player.x = m.GetDouble(0U);
				player.y = m.GetDouble(1U);
				int num2 = (int)(player.x + 8.0) >> 4;
				int num3 = (int)(player.y + 8.0) >> 4;
				if (num2 < 0 || num3 < 0 || num2 > 199 || num3 > 199)
				{
					player.Disconnect();
					return;
				}
				if (!player.canEdit && !player.ismod)
				{
					int num4 = this.world[num3, num2];
					if (num4 > 8 && num4 < 100)
					{
						if (num4 != 23 && num4 != 24 && num4 != 25 && num4 != 26 && num4 != 27 && num4 != 28)
						{
							player.cheat++;
						}
					}
					if (num4 == 0 && m.GetDouble(5U) < 1.0)
					{
						player.cheat++;
					}
					if (player.cheat > 4)
					{
						player.Disconnect();
						return;
					}
				}
				try
				{
					base.Broadcast("m", new object[]
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
				catch (Exception exception)
				{
					base.PlayerIO.ErrorLog.WriteError("Disconnected user for sending in garbage data resulting in error", exception);
					player.Disconnect();
				}
				return;
			}
			}
			if (m.Type == this.editchar)
			{
				if ((DateTime.Now - player.lastEdit).TotalMilliseconds < 15.0)
				{
					player.threshold -= 10;
				}
				else if (player.threshold < 250)
				{
					player.threshold += 30;
				}
				if (player.threshold < 0)
				{
					player.threshold = 0;
				}
				else
				{
					player.lastEdit = DateTime.Now;
					int int2 = m.GetInt(2U);
					if (int2 >= 0 && int2 <= 255)
					{
						if (int2 < 37 || int2 > 42 || player.haveSmileyPackage)
						{
							if (player.canEdit)
							{
								if (m.GetInt(0U) >= 1 && m.GetInt(0U) <= 198 && m.GetInt(1U) <= 198 && ((this.editkey != "") ? (m.GetInt(1U) >= 1) : (m.GetInt(1U) >= 5)))
								{
									if (this.world[m.GetInt(1U), m.GetInt(0U)] != int2)
									{
										this.world[m.GetInt(1U), m.GetInt(0U)] = int2;
										base.Broadcast(Message.Create("b", new object[]
										{
											m.GetInt(0U),
											m.GetInt(1U),
											int2
										}));
									}
								}
							}
						}
					}
				}
			}
			else if (m.Type == this.editchar + "k")
			{
				this.crownid = player.Id;
				base.Broadcast("k", new object[]
				{
					player.Id
				});
			}
			else if (m.Type == this.editchar + "r")
			{
				this.redtime = this.getTime();
				if (!this.red)
				{
					base.Broadcast("hide", new object[]
					{
						"red"
					});
				}
				this.red = true;
			}
			else if (m.Type == this.editchar + "g")
			{
				this.greentime = this.getTime();
				if (!this.red)
				{
					base.Broadcast("hide", new object[]
					{
						"green"
					});
				}
				this.green = true;
			}
			else if (m.Type == this.editchar + "b")
			{
				this.bluetime = this.getTime();
				if (!this.red)
				{
					base.Broadcast("hide", new object[]
					{
						"blue"
					});
				}
				this.blue = true;
			}
			else if (m.Type == this.editchar + "f")
			{
				int int3 = m.GetInt(0U);
				if (int3 >= 0 && (int3 <= 5 || player.haveSmileyPackage) && int3 <= 11)
				{
					player.face = int3;
					base.Broadcast("face", new object[]
					{
						player.Id,
						m.GetInt(0U)
					});
				}
			}
		}

		private void sendInitMessage(Player player)
		{
			Message message = Message.Create("init", new object[]
			{
				this.Rot13(this.editchar),
				player.Id,
				player.canEdit,
				player.owner
			});
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < 200; i++)
			{
				for (int j = 0; j < 200; j++)
				{
					message.Add(this.world[i, j]);
				}
			}
			player.Send(message);
		}

		private byte[] serializeWorld()
		{
			byte[] array = new byte[40000];
			for (int i = 0; i < 200; i++)
			{
				for (int j = 0; j < 200; j++)
				{
					array[i * 200 + j] = (byte)this.world[i, j];
				}
			}
			return array;
		}

		private void unserializeWorld(byte[] wd)
		{
			for (int i = 0; i < 200; i++)
			{
				for (int j = 0; j < 200; j++)
				{
					this.world[i, j] = (int)wd[i * 200 + j];
				}
			}
		}

		private string Rot13(string value)
		{
			char[] array = value.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				int num = (int)array[i];
				if (num >= 97 && num <= 122)
				{
					if (num > 109)
					{
						num -= 13;
					}
					else
					{
						num += 13;
					}
				}
				else if (num >= 65 && num <= 90)
				{
					if (num > 77)
					{
						num -= 13;
					}
					else
					{
						num += 13;
					}
				}
				array[i] = (char)num;
			}
			return new string(array);
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

		private Dictionary<long, int> ips = new Dictionary<long, int>();

		private int plays = 0;

		private int sessionplays = 0;

		private string editchar = "m";

		private bool die = false;

		private bool owned = false;

		private DatabaseObject ownedWorld;

		private bool ready = false;
	}
}
