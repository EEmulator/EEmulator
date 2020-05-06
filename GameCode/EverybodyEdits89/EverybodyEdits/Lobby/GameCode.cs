using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Lobby
{
	[RoomType("Lobby89")]
	public class GameCode : Game<Player>
	{
		public override void GameStarted()
		{
			base.PreloadPlayerObjects = true;
			base.PreloadPayVaults = false;
			base.PlayerIO.BigDB.LoadRange("PayVaultItems", "PriceCoins", null, null, null, 1000, delegate (DatabaseObject[] ob)
			{
				for (var a = 0; a < ob.Length; a++)
				{
					if (ob[a].GetBool("Enabled", false) || ob[a].GetBool("IsClassic", false))
					{
						Console.WriteLine(ob[a]);
						this.shopitems.Add(new ShopItem(ob[a].Key, ob[a].GetInt("PriceCoins"), ob[a].GetInt("PriceEnergy", -1), ob[a].GetInt("EnergyPerClick", 5), ob[a].GetBool("Reusable", false), ob[a].GetBool("BetaOnly", false), ob[a].GetBool("IsFeatured", false), ob[a].GetBool("IsClassic", false), ob[a].GetBool("Enabled", false)));
					}
				}
				this.loadedShopConfig = true;
				this.EmptyQueue();
			});
		}

		public override bool AllowUserJoin(Player player)
		{
			foreach (var p in base.Players)
			{
				p.Disconnect();
			}
			return !(base.RoomId != player.ConnectUserId) && base.AllowUserJoin(player);
		}

		public override void UserJoined(Player player)
		{
			var haveBeta = player.PlayerObject.GetBool("haveSmileyPackage", false) || player.PayVault.Has("pro");
			if (!player.PayVault.Has("pro") && haveBeta)
			{
				player.PayVault.Give(new BuyItemInfo[]
				{
					new BuyItemInfo("pro")
				}, delegate ()
				{
				});
			}
			var oip = player.PlayerObject.GetString("lastip", "");
			var nip = player.IPAddress.ToString();
			if (oip != nip)
			{
				player.PlayerObject.Set("lastip", nip);
				player.PlayerObject.Save();
			}
		}

		public void AddToQueue(int id, string conncetUserId, string method, Message message)
		{
			this.queue.Add(new QueueItem(id, conncetUserId, method, message));
			this.EmptyQueue();
		}

		private void sendShopUpdate(QueueItem item, Player p, bool refresh)
		{
			var i = Message.Create(item.method, new object[]
			{
				refresh,
				p.Energy,
				p.GetSecoundsToNextEnergy,
				p.MaxEnergy,
				p.EnergyRechargeSecounds,
				p.PayVault.Coins
			});
			foreach (var j in this.shopitems)
			{
				if ((!j.BetaOnly || p.HasBeta) && (!p.PayVault.Has(j.key) || j.Reusable))
				{
					i.Add(j.key);
					i.Add(j.PriceEnergy);
					i.Add(j.EnergyPerClick);
					i.Add(p.GetEnergyStatus(j.key));
					i.Add(j.PriceGems);
					i.Add(p.PayVault.Count(j.key));
					i.Add(j.IsFeatured);
					i.Add(j.IsClassic);
				}
			}
			p.Send(i);
		}

		public void EmptyQueue()
		{
			if (this.loadedShopConfig)
			{
				QueueItem item;
				foreach (var item2 in this.queue)
				{
					item = item2;
					Player p;
					foreach (var p2 in base.Players)
					{
						p = p2;
						if (p.Id == item.id && p.ConnectUserId == item.connectUserId)
						{
							var method = item.method;
							if (method != null)
							{
								if (!(method == "getShop"))
								{
									if (!(method == "toggleProfile"))
									{
										if (!(method == "getProfile"))
										{
											if (method == "useEnergy")
											{
												var target = item.message.GetString(0U);
												foreach (var i in this.shopitems)
												{
													if (target == i.key && (!p.PayVault.Has(i.key) || i.Reusable) && i.PriceEnergy > 0 && i.Enabled)
													{
														if (p.UseEnergy(i.EnergyPerClick))
														{
															p.SetEnergyStatus(i.key, p.GetEnergyStatus(i.key) + i.EnergyPerClick);
															if (p.GetEnergyStatus(i.key) >= i.PriceEnergy)
															{
																p.SetEnergyStatus(i.key, 0);
																p.PayVault.Give(new BuyItemInfo[]
																{
																	new BuyItemInfo(i.key)
																}, delegate ()
																{
																	p.SaveShop(delegate ()
																	{
																		this.sendShopUpdate(item, p, true);
																	});
																}, delegate (PlayerIOError e)
																{
																	p.SaveShop(delegate ()
																	{
																		this.sendShopUpdate(item, p, false);
																	});
																});
															}
															else
															{
																p.SaveShop(delegate ()
																{
																	this.sendShopUpdate(item, p, false);
																});
															}
														}
														else
														{
															p.Send(item.method, new object[]
															{
																"error"
															});
														}
													}
												}
											}
										}
										else
										{
											var visible = true;
											if (p.PlayerObject.Contains("visible"))
											{
												visible = p.PlayerObject.GetBool("visible");
											}
											p.Send(item.method, new object[]
											{
												visible
											});
										}
									}
									else
									{
										p.PlayerObject.Set("visible", item.message.GetBoolean(0U));
										p.PlayerObject.Save(delegate ()
										{
											p.Send(item.method, new object[]
											{
												item.message.GetBoolean(0U)
											});
										});
									}
								}
								else
								{
									p.RefreshPlayerObject(delegate
									{
										p.PayVault.Refresh(delegate ()
										{
											this.sendShopUpdate(item, p, true);
										});
									});
								}
							}
						}
					}
				}
				this.queue = new List<QueueItem>();
			}
		}

		public override void GotMessage(Player player, Message m)
		{
			var haveSmileyPackage = player.PlayerObject.GetBool("haveSmileyPackage", false) || player.PayVault.Has("pro");
			var type2 = m.Type;
			switch (type2)
			{
				case "getShop":
					this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
					break;
				case "useEnergy":
					this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
					break;
				case "toggleProfile":
					this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
					break;
				case "getProfile":
					this.AddToQueue(player.Id, player.ConnectUserId, m.Type, m);
					break;
				case "useGems":
					Console.WriteLine("here baby!");
					player.PayVault.Buy(true, new BuyItemInfo[]
					{
					new BuyItemInfo(m.GetString(0U))
					}, delegate ()
					{
						player.Send(m.Type, new object[]
						{
						"refresh"
						});
					}, delegate (PlayerIOError e)
					{
						player.Send(m.Type, new object[]
						{
						"error"
						});
					});
					break;
				case "getRoom":
				{
					var myroom = player.PlayerObject.GetString("room0", "");
					if (haveSmileyPackage)
					{
						if (myroom != "")
						{
							player.Send("r", new object[]
							{
							myroom
							});
						}
						else
						{
							player.Send("creating", new object[]
							{
							player.PlayerObject.GetBool("haveSmileyPackage", false),
							player.PayVault.Has("pro")
							});
							this.createId(false, player);
						}
					}
					else
					{
						player.Send("no!", new object[]
						{
						player.PlayerObject.GetBool("haveSmileyPackage", false),
						player.PayVault.Has("pro")
						});
					}
					break;
				}
				case "getSavedLevel":
				{
					var type = m.GetInt(0U);
					var offset = m.GetInt(1U);
					var count = player.PayVault.Count("world" + type);
					if (type == 0 && haveSmileyPackage)
					{
						count++;
					}
					if (count != 0)
					{
						if (count > offset)
						{
							var roomid = player.PlayerObject.GetString(string.Concat(new object[]
							{
							"world",
							type,
							"x",
							offset
							}), "");
							if (roomid != "")
							{
								player.Send("r", new object[]
								{
								roomid
								});
							}
							else
							{
								this.getUniqueId(false, player, delegate (string newid)
								{
									var newworld = new DatabaseObject();
									newworld.Set("width", 25);
									newworld.Set("height", 25);
									newworld.Set("owner", player.ConnectUserId);
									switch (type)
									{
										case 1:
											newworld.Set("width", 50);
											newworld.Set("height", 50);
											break;
										case 2:
											newworld.Set("width", 100);
											newworld.Set("height", 100);
											break;
										case 3:
											newworld.Set("width", 200);
											newworld.Set("height", 200);
											break;
										case 4:
											newworld.Set("width", 400);
											newworld.Set("height", 50);
											break;
										case 5:
											newworld.Set("width", 400);
											newworld.Set("height", 200);
											break;
										case 6:
											newworld.Set("width", 100);
											newworld.Set("height", 400);
											break;
										case 7:
											newworld.Set("width", 636);
											newworld.Set("height", 50);
											break;
									}
									this.PlayerIO.BigDB.CreateObject("worlds", newid, newworld, delegate (DatabaseObject o)
									{
										player.PlayerObject.Set(string.Concat(new object[]
										{
										"world",
										type,
										"x",
										offset
										}), newid);
										player.PlayerObject.Save(delegate ()
										{
											player.Send("r", new object[]
											{
											newid
											});
										});
									});
								});
							}
						}
					}
					break;
				}
				case "getBetaRoom":
				{
					var myroom = player.PlayerObject.GetString("betaonlyroom", "");
					if (haveSmileyPackage)
					{
						if (myroom != "")
						{
							player.Send("r", new object[]
							{
							myroom
							});
						}
						else
						{
							player.Send("creating", new object[]
							{
							player.PlayerObject.GetBool("haveSmileyPackage", false),
							player.PayVault.Has("pro")
							});
							this.createId(true, player);
						}
					}
					else
					{
						player.Send("no!", new object[]
						{
						player.PlayerObject.GetBool("haveSmileyPackage", false),
						player.PayVault.Has("pro")
						});
					}
					break;
				}
				case "setUsername":
				{
					var username = player.PlayerObject.GetString("name", null);
					if (username == null && player.ConnectUserId != "simpleguest")
					{
						var newname = m.GetString(0U).ToLower();
						var test = new Regex("[^0-9a-z]");
						if (test.IsMatch(newname))
						{
							player.Send("error", new object[]
							{
							"Your username contains invalid charaters. Valid charaters are 0-9 and A-Z"
							});
						}
						else if (newname.Length > 20)
						{
							player.Send("error", new object[]
							{
							"Your username cannot be more than 20 characters long."
							});
						}
						else if (newname.Length < 3)
						{
							player.Send("error", new object[]
							{
							"Your username must be atleast 3 characters long."
							});
						}
						else
						{
							var obj = new DatabaseObject();
							obj.Set("owner", player.ConnectUserId);
							base.PlayerIO.BigDB.CreateObject("Usernames", newname, obj, delegate (DatabaseObject result)
							{
								player.PlayerObject.Set("name", newname);
								player.PlayerObject.Save(delegate ()
								{
									player.Send("username", new object[]
									{
									newname
									});
								});
							}, delegate (PlayerIOError e)
							{
								player.Send("error", new object[]
								{
								"The username " + newname.ToUpper() + " is already taken!"
								});
							});
						}
					}
					else
					{
						player.Send("username", new object[]
						{
						username
						});
					}
					break;
				}
			}
		}

		private void getUniqueId(bool isbetaonly, Player player, Callback<string> myCallback)
		{
			var newid = "";
			if (isbetaonly)
			{
				newid = "BW" + Convert.ToBase64String(BitConverter.GetBytes((DateTime.Now - new DateTime(1981, 3, 25)).TotalMilliseconds)).Replace("=", "").Replace("+", "_").Replace("/", "-");
			}
			else
			{
				newid = "PW" + Convert.ToBase64String(BitConverter.GetBytes((DateTime.Now - new DateTime(1981, 3, 25)).TotalMilliseconds)).Replace("=", "").Replace("+", "_").Replace("/", "-");
			}
			base.PlayerIO.BigDB.Load("Worlds", newid, delegate (DatabaseObject o)
			{
				if (o != null)
				{
					this.getUniqueId(isbetaonly, player, myCallback);
				}
				else
				{
					myCallback(newid);
				}
			});
		}

		private void createId(bool isbetaonly, Player player)
		{
			this.getUniqueId(isbetaonly, player, delegate (string newid)
			{
				player.PlayerObject.Set(isbetaonly ? "betaonlyroom" : "room0", newid);
				player.PlayerObject.Save(delegate ()
				{
					player.Send("r", new object[]
					{
						newid
					});
				});
			});
		}

		private List<QueueItem> queue = new List<QueueItem>();

		private List<ShopItem> shopitems = new List<ShopItem>();

		public bool loadedShopConfig = false;
	}
}
