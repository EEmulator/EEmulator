using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    [RoomType("Everybodyedits89")]
    public class EverybodyEdits : Game<Player>
    {
        public override void GameStarted()
        {
            base.PreloadPlayerObjects = true;
            base.PreloadPayVaults = true;
            var allowedChars = "acdefghijnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ";
            var rd = new Random();
            this.editchar = ".";
            for (var a = 0; a < rd.Next(1, 3); a++)
            {
                this.editchar += allowedChars[rd.Next(0, allowedChars.Length)].ToString();
            }
            this.isbetalevel = (base.RoomId.Substring(0, 2) == "BW" || base.RoomId == "ChrisWorld");
            var isowned = this.isbetalevel || base.RoomId.Substring(0, 2) == "PW" || this.isTutorialRoom || base.RoomId == "toturialWorld";
            if (this.isbetalevel)
            {
                base.RoomData["beta"] = "true";
            }
            if (isowned || base.RoomData.ContainsKey("owned"))
            {
                this.owned = true;
                this.lockedroom = true;
                this.editkey = "23f23fdswvsdv24t24wrgwerg23t5h35h35h35h3x2f'23f5";
                base.RoomData["needskey"] = "yep";
                base.RoomData["plays"] = "0";
                base.RoomData["rating"] = "0";
                base.RoomData["name"] = "My World";
                this.loadWorld(base.RoomId);
            }
            else if (base.RoomData.ContainsKey("editkey"))
            {
                this.lockedroom = true;
                this.editkey = base.RoomData["editkey"];
                base.RoomData.Remove("editkey");
                base.RoomData["needskey"] = "yep";
                base.RoomData["plays"] = "0";
                base.RoomData["rating"] = "0";
            }
            base.RoomData.Save();
            this.startTimers();
        }

        protected void startTimers()
        {
            base.AddTimer(new Action(this.checkVersion), 10000);
            this.checkVersion();
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
                foreach (var p in base.Players)
                {
                    if (p.cheat > 0)
                    {
                        p.cheat--;
                    }
                }
                if (this.die)
                {
                    foreach (var p in base.Players)
                    {
                        p.Disconnect();
                    }
                }
                if (this.upgrade)
                {
                    foreach (var p in base.Players)
                    {
                        p.Send("upgrade", new object[0]);
                        p.Disconnect();
                    }
                }
                var nl = new List<Ban>();
                foreach (var b in this.bans)
                {
                    if ((b.timestamp - DateTime.Now).TotalMilliseconds > 0.0)
                    {
                        nl.Add(b);
                    }
                }
                this.bans = nl;
                if (this.coinscollected > 1)
                {
                    this.coinscollected--;
                }
                else
                {
                    this.coinscollected = 0;
                }
            }, 250);
        }

        protected virtual void loadWorld(string roomid)
        {
            base.PlayerIO.BigDB.LoadOrCreate("Worlds", roomid, delegate (DatabaseObject o)
            {
                var d = DateTime.Now;
                this.ownedWorld = o;
                this.baseworld.fromDatabaseObject(o);
                Console.WriteLine("Object unserialized " + DateTime.Now.Subtract(d).TotalMilliseconds);
                this.width = this.ownedWorld.GetInt("width", this.width);
                this.height = this.ownedWorld.GetInt("height", this.height);
                this.levelOwner = this.ownedWorld.GetString("owner", "");
                this.coinbanned = this.ownedWorld.GetBool("coinbanned", false);
                this.plays = this.ownedWorld.GetInt("plays", 1);
                base.RoomData["plays"] = this.plays.ToString();
                base.RoomData["name"] = this.ownedWorld.GetString("name", "Untitled World");
                base.RoomData.Save();
                Console.WriteLine("Roomdata saved " + DateTime.Now.Subtract(d).TotalMilliseconds);
                if (this.levelOwner != "")
                {
                    base.PlayerIO.BigDB.Load("PlayerObjects", this.levelOwner, delegate (DatabaseObject oo)
                    {
                        this.levelOwnerName = oo.GetString("name", "");
                        this.ready = true;
                        foreach (var p2 in base.Players)
                        {
                            if (p2.ready)
                            {
                                this.sendInitMessage(p2);
                            }
                        }
                    });
                }
                else
                {
                    this.ready = true;
                    foreach (var p in base.Players)
                    {
                        if (p.ready)
                        {
                            this.sendInitMessage(p);
                        }
                    }
                }
                Console.WriteLine("Total work time " + DateTime.Now.Subtract(d).TotalMilliseconds);
            });
        }

        protected void addBan(string userid, int minutes)
        {
            this.bans.Add(new Ban(userid, DateTime.Now.AddMinutes((double)minutes)));
        }

        protected void checkVersion()
        {
            base.PlayerIO.BigDB.Load("Config", "config", delegate (DatabaseObject o)
            {
                if (this.version < o.GetInt("version", this.version) || (this.isbetalevel && this.version < o.GetInt("betaversion", this.version)))
                {
                    this.upgrade = true;
                }
            });
        }

        public override bool AllowUserJoin(Player player)
        {
            try
            {
                player.PayVault.Refresh(() => { });

                player.haveSmileyPackage = (player.PlayerObject.GetBool("haveSmileyPackage", false) || player.PayVault.Has("pro"));
                player.canbemod = player.PlayerObject.GetBool("isModerator", false);
                player.room0 = player.PlayerObject.GetString("room0", "");
                player.betaonlyroom = player.PlayerObject.GetString("betaonlyroom", "");
                player.name = player.PlayerObject.GetString("name", "Guest-" + player.Id.ToString());
                player.canchat = true; //(player.PayVault.Has("canchat") && !player.PlayerObject.GetBool("chatbanned", false));
                player.lastCoin = player.PlayerObject.GetDateTime("lastcoin", DateTime.Now.AddHours(-24.0));
            }
            catch (Exception e)
            {
                base.PlayerIO.ErrorLog.WriteError("In block #1", e);
                return false;
            }

            player.canEdit = true;
            player.owner = true;

            return true;

            //bool result;
            //if (this.PlayerCount > 50 && !player.canbemod)
            //{
            //    player.Send("Info", new object[]
            //    {
            //        "Room is full",
            //        "Sorry this room is full, please try again later :)"
            //    });
            //    result = false;
            //}
            //else
            //{
            //    try
            //    {
            //        if (player.PlayerObject.GetBool("banned", false))
            //        {
            //            player.Send("info", new object[]
            //            {
            //                "You are banned",
            //                "This account is banned due to abuse or fraud."
            //            });
            //            return false;
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        base.PlayerIO.ErrorLog.WriteError("In block #2", e);
            //        return false;
            //    }
            //    try
            //    {
            //        if (player.isguest)
            //        {
            //            player.canWinEnergy = false;
            //        }
            //        if (this.editkey == "" || (player.JoinData.ContainsKey("editkey") && player.JoinData["editkey"] == this.editkey))
            //        {
            //            player.canEdit = true;
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        base.PlayerIO.ErrorLog.WriteError("In block #3", e);
            //        return false;
            //    }
            //    try
            //    {
            //        if (this.owned && (base.RoomId == player.room0 || player.betaonlyroom == base.RoomId) && player.haveSmileyPackage)
            //        {
            //            player.canEdit = true;
            //            player.owner = true;
            //            try
            //            {
            //                if (this.ownedWorld != null)
            //                {
            //                    this.ownedWorld.Set("owner", player.ConnectUserId);
            //                }
            //            }
            //            catch (Exception e)
            //            {
            //                base.PlayerIO.ErrorLog.WriteError("In block #4", e);
            //                return false;
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        base.PlayerIO.ErrorLog.WriteError("In block #5", e);
            //        return false;
            //    }
            //    try
            //    {
            //        foreach (var b in this.bans)
            //        {
            //            if (b.userid == player.ConnectUserId && !player.owner && !player.canbemod)
            //            {
            //                player.Send("info", new object[]
            //                {
            //                    "You are banned",
            //                    "You have been banned from this world"
            //                });
            //                return false;
            //            }
            //        }
            //    }
            //    catch (Exception e)
            //    {
            //        base.PlayerIO.ErrorLog.WriteError("In block #6", e);
            //        return false;
            //    }
            //    result = true;
            //}
            //return result;
        }

        public override void UserJoined(Player player)
        {
            if (!this.ips.ContainsKey(player.IPAddress.ToString()))
            {
                this.ips.Add(player.IPAddress.ToString(), -1);
                this.plays++;
                this.sessionplays++;
                if (this.sessionplays % 15 == 0)
                {
                    if (this.owned && this.ownedWorld != null)
                    {
                        if (this.sessionplays > 10)
                        {
                            this.ownedWorld.Set("plays", this.plays);
                        }
                        this.ownedWorld.Save();
                    }
                    base.Broadcast("updatemeta", new object[]
                    {
                        this.levelOwnerName,
                        base.RoomData["name"],
                        this.plays
                    });
                }
                base.RoomData["plays"] = this.plays.ToString();
                base.RoomData.Save();
            }
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

        private void ResetPlayers()
        {
            var tele = Message.Create("tele", new object[]
            {
                true
            });
            foreach (var p in base.Players)
            {
                if (!p.isgod && !p.ismod)
                {
                    p.x = 16.0;
                    p.y = 16.0;
                    var d = this.baseworld.getSpawn();
                    p.x = (double)(d.x * 16);
                    p.y = (double)(d.y * 16);
                    tele.Add(new object[]
                    {
                        p.Id,
                        p.x,
                        p.y
                    });
                }
            }
            base.Broadcast(tele);
        }

        public override void GotMessage(Player player, Message m)
        {
            var type = m.Type;
            switch (type)
            {
                case "clear":
                    if (this.owned && (player.owner || player.canbemod) && this.ownedWorld != null)
                    {
                        this.baseworld.reset();
                        base.Broadcast("clear", new object[]
                        {
                        this.width,
                        this.height
                        });
                    }
                    return;
                case "say":
                    this.handleSay(player, m);
                    return;
                case "autosay":
                    this.handleAutoSay(player, m);
                    return;
                case "diamondtouch":
                {
                    var xp = m.GetInt(0U);
                    var yp = m.GetInt(1U);
                    if (this.baseworld.getBrickType(0, xp, yp) == 241U)
                    {
                        player.face = 31;
                        base.Broadcast("face", new object[]
                        {
                        player.Id,
                        31
                        });
                    }
                    return;
                }
                case "save":
                    if (this.owned && (player.owner || player.canbemod))
                    {
                        if (this.sessionplays > 10)
                        {
                            this.ownedWorld.Set("plays", this.plays);
                        }
                        this.ownedWorld.Set("name", base.RoomData["name"]);
                        this.baseworld.save(delegate
                        {
                            player.Send("saved", new object[0]);
                        });
                    }
                    return;
                case "name":
                    if (this.owned && (player.owner || player.canbemod) && this.ownedWorld != null)
                    {
                        base.RoomData["name"] = m.GetString(0U);
                        base.RoomData.Save();
                        base.Broadcast("updatemeta", new object[]
                        {
                        this.levelOwnerName,
                        base.RoomData["name"],
                        this.plays
                        });
                        this.ownedWorld.Set("name", base.RoomData["name"]);
                    }
                    return;
                case "key":
                    if (player.owner)
                    {
                        this.editkey = m.GetString(0U);
                        foreach (var p in base.Players)
                        {
                            if (!p.owner)
                            {
                                if (p.isgod)
                                {
                                    base.Broadcast("god", new object[]
                                    {
                                    p.Id,
                                    false
                                    });
                                }
                                p.isgod = false;
                                p.canEdit = false;
                                p.Send("lostaccess", new object[0]);
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
                        player.ismod = true;
                        player.Send("access", new object[0]);
                        base.Broadcast("mod", new object[]
                        {
                        player.Id
                        });
                    }
                    return;
                case "rate":
                {
                    var score = m.GetInt(0U);
                    if (score >= 0 && score <= 5)
                    {
                        this.ips[player.IPAddress.ToString()] = score;
                        var total = 0m;
                        foreach (var key in this.ips.Keys)
                        {
                            total += this.ips[key];
                        }
                        total /= this.ips.Keys.Count;
                        base.RoomData["rating"] = Math.Round(total * 2m).ToString();
                        base.RoomData.Save();
                    }
                    return;
                }
                case "god":
                    if (player.canEdit)
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
                    if (!player.haveSmileyPackage && this.isbetalevel)
                    {
                        player.Send("betaonly", new object[0]);
                        player.Disconnect();
                    }
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
                    foreach (var p in base.Players)
                    {
                        if (p != player)
                        {
                            player.Send("add", new object[]
                            {
                            p.Id,
                            p.name,
                            p.face,
                            p.x,
                            p.y,
                            p.isgod,
                            p.ismod,
                            p.canchat,
                            p.coins
                            });
                        }
                    }
                    if (player.canchat)
                    {
                        for (var a = 0; a < this.chatmessages.Length; a++)
                        {
                            if (this.chatmessages[a] != null)
                            {
                                player.Send("write", new object[]
                                {
                                this.chatmessages[a].name,
                                this.chatmessages[a].text
                                });
                            }
                        }
                    }
                    player.Send("k", new object[]
                    {
                    this.crownid
                    });
                    return;
                case "c":
                    player.coins = m.GetInt(0U);
                    this.coinscollected++;
                    if (player.canWinEnergy && (DateTime.Now - player.lastCoin).TotalMinutes > 20.0 && DateTime.Now > player.coinTimer && this.coinscollected < 4 * this.PlayerCount && this.coinscollected < 40 && (DateTime.Now - this.coinanticheat).TotalMilliseconds > 100.0 && (DateTime.Now - player.lastmove).TotalSeconds < 15.0 && !this.coinbanned)
                    {
                        if (this.rd.Next(0, 75) == 1)
                        {
                            if (this.rd.Next(0, 50) == 1)
                            {
                                if (!player.PayVault.Has("smileywizard"))
                                {
                                    player.Send("info", new object[]
                                    {
                                    "Congratulations you just found the wizard smiley!",
                                    "You found an extra magical coin! Upon touching the coin you found that you had a wizard hat and beard..."
                                    });
                                    base.Broadcast("write", new object[]
                                    {
                                    "* MAGIC",
                                    player.name.ToUpper() + " just became a wizard!"
                                    });
                                    player.PayVault.Give(new BuyItemInfo[]
                                    {
                                    new BuyItemInfo("smileywizard")
                                    }, delegate ()
                                    {
                                    });
                                    player.PlayerObject.Set("lastcoin", DateTime.Now);
                                    player.PlayerObject.Save();
                                    player.lastCoin = DateTime.Now;
                                    player.canWinEnergy = false;
                                    base.Broadcast("face", new object[]
                                    {
                                    player.Id,
                                    22
                                    });
                                    player.Send("givewizard", new object[0]);
                                }
                                else if (!player.PayVault.Has("smileywizard2"))
                                {
                                    player.Send("info", new object[]
                                    {
                                    "Congratulations you just found the rare wizard smiley!",
                                    "You found an extra magical coin! Upon touching the coin you found that you had a red wizard hat and beard..."
                                    });
                                    base.Broadcast("write", new object[]
                                    {
                                    "* MAGIC",
                                    player.name.ToUpper() + " just became a fire wizard!"
                                    });
                                    player.PayVault.Give(new BuyItemInfo[]
                                    {
                                    new BuyItemInfo("smileywizard2")
                                    }, delegate ()
                                    {
                                    });
                                    player.PlayerObject.Set("lastcoin", DateTime.Now);
                                    player.PlayerObject.Save();
                                    player.lastCoin = DateTime.Now;
                                    player.canWinEnergy = false;
                                    base.Broadcast("face", new object[]
                                    {
                                    player.Id,
                                    32
                                    });
                                    player.Send("givewizard2", new object[0]);
                                }
                                else if (!player.PayVault.Has("smileywitch"))
                                {
                                    player.Send("info", new object[]
                                    {
                                    "Congratulations you just found the rare witch smiley!",
                                    "You found an ultra magical coin! Upon touching the coin you found that you had become a witch..."
                                    });
                                    base.Broadcast("write", new object[]
                                    {
                                    "* MAGIC",
                                    player.name.ToUpper() + " just became a witch!"
                                    });
                                    player.PayVault.Give(new BuyItemInfo[]
                                    {
                                    new BuyItemInfo("smileywitch")
                                    }, delegate ()
                                    {
                                    });
                                    player.PlayerObject.Set("lastcoin", DateTime.Now);
                                    player.PlayerObject.Save();
                                    player.lastCoin = DateTime.Now;
                                    player.canWinEnergy = false;
                                    base.Broadcast("face", new object[]
                                    {
                                    player.Id,
                                    32
                                    });
                                    player.Send("givewitch", new object[0]);
                                }
                            }
                            else if (this.rd.Next(0, 100) == 1)
                            {
                                var maxe = player.PlayerObject.GetInt("maxEnergy", 200);
                                maxe += 15;
                                player.Send("info", new object[]
                                {
                                "Congratulations you just found a rare magical coin!",
                                "Magical coins are very rare and sought after as they increase your energy!\n\nYour new maximum energy is now " + maxe + "!\nYour energy has also been fully recharged!"
                                });
                                base.Broadcast("write", new object[]
                                {
                                "* MAGIC",
                                player.name.ToUpper() + " just found a rare magical coin and was awarded 15 extra total energy and a full energy bar!"
                                });
                                player.PlayerObject.Remove("shopDate");
                                player.PlayerObject.Set("maxEnergy", maxe);
                                player.PlayerObject.Set("lastcoin", DateTime.Now);
                                player.PlayerObject.Save();
                                player.lastCoin = DateTime.Now;
                                player.canWinEnergy = false;
                                player.Send("refreshshop", new object[0]);
                            }
                            else if (this.rd.Next(0, 75) == 1)
                            {
                                var maxe = player.PlayerObject.GetInt("maxEnergy", 200);
                                maxe += 10;
                                player.Send("info", new object[]
                                {
                                "Congratulations you just found an extra magical coin!",
                                "Magical coins are very rare and sought after as they increase your energy!\n\nYour new maximum energy is now " + maxe + "!\nYour energy has also been fully recharged!"
                                });
                                base.Broadcast("write", new object[]
                                {
                                "* MAGIC",
                                player.name.ToUpper() + " just found an extra magical coin and was awarded 10 extra total energy and a full energy bar!"
                                });
                                player.PlayerObject.Remove("shopDate");
                                player.PlayerObject.Set("maxEnergy", maxe);
                                player.PlayerObject.Set("lastcoin", DateTime.Now);
                                player.PlayerObject.Save();
                                player.lastCoin = DateTime.Now;
                                player.canWinEnergy = false;
                                player.Send("refreshshop", new object[0]);
                            }
                            else if (this.rd.Next(0, 2) == 1)
                            {
                                var maxe = player.PlayerObject.GetInt("maxEnergy", 200);
                                maxe += 2;
                                player.Send("info", new object[]
                                {
                                "Congratulations you just found an magical coin!",
                                "Magical coins are very rare and sought after as they increase your energy!\n\nYour new maximum energy is now " + maxe + "!"
                                });
                                base.Broadcast("write", new object[]
                                {
                                "* MAGIC",
                                player.name.ToUpper() + " just found a magical coin and was awarded 2 extra total energy!"
                                });
                                player.PlayerObject.Set("maxEnergy", maxe);
                                player.PlayerObject.Set("lastcoin", DateTime.Now);
                                player.PlayerObject.Save();
                                player.lastCoin = DateTime.Now;
                                player.canWinEnergy = false;
                                player.Send("refreshshop", new object[0]);
                            }
                            else
                            {
                                var maxe = player.PlayerObject.GetInt("maxEnergy", 200);
                                maxe++;
                                player.Send("info", new object[]
                                {
                                "Congratulations you just found an minor magical coin!",
                                "Magical coins are very rare and sought after as they increase your energy!\n\nYour new maximum energy is now " + maxe + "!"
                                });
                                base.Broadcast("write", new object[]
                                {
                                "* MAGIC",
                                player.name.ToUpper() + " just found a minor magical coin and was awarded 1 extra total energy!"
                                });
                                player.PlayerObject.Set("maxEnergy", maxe);
                                player.PlayerObject.Set("lastcoin", DateTime.Now);
                                player.PlayerObject.Save();
                                player.lastCoin = DateTime.Now;
                                player.canWinEnergy = false;
                                player.Send("refreshshop", new object[0]);
                            }
                        }
                        else if (this.rd.Next(0, 15) == 1)
                        {
                            if (!player.PayVault.Has("smileyxmasgrinch"))
                            {
                                player.Send("info", new object[]
                                {
                                "Congratulations you just found the grinch smiley!",
                                "You found an magical Christmas coin! Upon touching the coin you found that you where the feared Grinch who stole Christmas."
                                });
                                base.Broadcast("write", new object[]
                                {
                                "* MAGIC",
                                player.name.ToUpper() + " just became the feared Christmas Grinch!"
                                });
                                player.PayVault.Give(new BuyItemInfo[]
                                {
                                new BuyItemInfo("smileyxmasgrinch")
                                }, delegate ()
                                {
                                });
                                player.PlayerObject.Set("lastcoin", DateTime.Now);
                                player.PlayerObject.Save();
                                player.lastCoin = DateTime.Now;
                                player.canWinEnergy = false;
                                base.Broadcast("face", new object[]
                                {
                                player.Id,
                                48
                                });
                                player.Send("givegrinch", new object[0]);
                            }
                        }
                        player.coinTimer = DateTime.Now.AddMinutes(1.0);
                    }
                    this.coinanticheat = DateTime.Now;
                    base.Broadcast("c", new object[]
                    {
                    player.Id,
                    player.coins
                    });
                    return;
                case "m":
                {
                    player.x = m.GetDouble(0U);
                    player.y = m.GetDouble(1U);
                    var xo = (int)(player.x + 8.0) >> 4;
                    var yo = (int)(player.y + 8.0) >> 4;
                    if (xo < 0 || yo < 0 || xo > this.width - 1 || yo > this.height - 1)
                    {
                        player.Disconnect();
                        return;
                    }
                    if (!player.canEdit && !player.ismod)
                    {
                        if (this.baseworld.getBrickType(0, xo, yo) == 0U && m.GetDouble(5U) < 1.0)
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
                        var horizontal = m.GetInt(6U);
                        var vertical = m.GetInt(7U);
                        if (player.horizontal != horizontal || player.vertical != vertical)
                        {
                            player.lastmove = DateTime.Now;
                        }
                        player.horizontal = horizontal;
                        player.vertical = vertical;
                        base.Broadcast("m", new object[]
                        {
                        player.Id,
                        m.GetDouble(0U),
                        m.GetDouble(1U),
                        m.GetDouble(2U),
                        m.GetDouble(3U),
                        m.GetDouble(4U),
                        m.GetDouble(5U),
                        horizontal,
                        vertical,
                        player.coins
                        });
                    }
                    catch (Exception e)
                    {
                        base.PlayerIO.ErrorLog.WriteError("We got invalid data from " + player.ConnectUserId, e);
                    }
                    return;
                }
            }
            if (m.Type == this.editchar)
            {
                this.placeBrick(player, m);
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
                if (!this.green)
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
                if (!this.blue)
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
                var face = m.GetInt(0U);
                var dobreak = false;
                if (face > 11)
                {
                    dobreak = true;
                    if (face == 12 && player.PayVault.Has("smileyninja"))
                    {
                        dobreak = false;
                    }
                    if (face == 13 && player.PayVault.Has("smileysanta"))
                    {
                        dobreak = false;
                    }
                    if (face == 14 && player.PayVault.Has("smileyworker"))
                    {
                        dobreak = false;
                    }
                    if (face == 15 && player.PayVault.Has("smileybigspender"))
                    {
                        dobreak = false;
                    }
                    if (face == 16 && player.PayVault.Has("smileysuper"))
                    {
                        dobreak = false;
                    }
                    if (face == 17 && player.PayVault.Has("smileysupprice"))
                    {
                        dobreak = false;
                    }
                    if (face == 18)
                    {
                        dobreak = false;
                    }
                    if (face == 19 && player.PayVault.Has("smileygirl"))
                    {
                        dobreak = false;
                    }
                    if (face == 20 && player.PayVault.Has("mixednewyear2010"))
                    {
                        dobreak = false;
                    }
                    if (face == 21 && player.PayVault.Has("smileycoy"))
                    {
                        dobreak = false;
                    }
                    if (face == 22 && player.PayVault.Has("smileywizard"))
                    {
                        dobreak = false;
                    }
                    if (face == 23 && player.PayVault.Has("smileyfanboy"))
                    {
                        dobreak = false;
                    }
                    if (face == 24 && player.PayVault.Has("smileyterminator"))
                    {
                        dobreak = false;
                    }
                    if (face == 25 && player.PayVault.Has("smileyxd"))
                    {
                        dobreak = false;
                    }
                    if (face == 26 && player.PayVault.Has("smileybully"))
                    {
                        dobreak = false;
                    }
                    if (face == 27 && player.PayVault.Has("smileycommando"))
                    {
                        dobreak = false;
                    }
                    if (face == 28 && player.PayVault.Has("smileyvalentines2011"))
                    {
                        dobreak = false;
                    }
                    if (face == 29 && player.PayVault.Has("smileybird"))
                    {
                        dobreak = false;
                    }
                    if (face == 30 && player.PayVault.Has("smileybunni"))
                    {
                        dobreak = false;
                    }
                    if (face == 31)
                    {
                        dobreak = false;
                    }
                    if (face == 32 && player.PayVault.Has("smileywizard2"))
                    {
                        dobreak = false;
                    }
                    if (face == 33 && player.PayVault.Has("smileyxdp"))
                    {
                        dobreak = false;
                    }
                    if (face == 34 && player.PayVault.Has("smileypostman"))
                    {
                        dobreak = false;
                    }
                    if (face == 35 && player.PayVault.Has("smileytemplar"))
                    {
                        dobreak = false;
                    }
                    if (face == 36 && player.PayVault.Has("smileyangel"))
                    {
                        dobreak = false;
                    }
                    if (face == 37 && player.PayVault.Has("smileynurse"))
                    {
                        dobreak = false;
                    }
                    if (face == 38 && player.PayVault.Has("smileyhw2011vampire"))
                    {
                        dobreak = false;
                    }
                    if (face == 39 && player.PayVault.Has("smileyhw2011ghost"))
                    {
                        dobreak = false;
                    }
                    if (face == 40 && player.PayVault.Has("smileyhw2011frankenstein"))
                    {
                        dobreak = false;
                    }
                    if (face == 41 && player.PayVault.Has("smileywitch"))
                    {
                        dobreak = false;
                    }
                    if (face == 42 && player.PayVault.Has("smileytg2011indian"))
                    {
                        dobreak = false;
                    }
                    if (face == 43 && player.PayVault.Has("smileytg2011pilgrim"))
                    {
                        dobreak = false;
                    }
                    if (face == 44 && player.PayVault.Has("smileypumpkin1"))
                    {
                        dobreak = false;
                    }
                    if (face == 45 && player.PayVault.Has("smileypumpkin2"))
                    {
                        dobreak = false;
                    }
                    if (face == 46 && player.PayVault.Has("smileyxmassnowman"))
                    {
                        dobreak = false;
                    }
                    if (face == 47 && player.PayVault.Has("smileyxmasreindeer"))
                    {
                        dobreak = false;
                    }
                    if (face == 48 && player.PayVault.Has("smileyxmasgrinch"))
                    {
                        dobreak = false;
                    }
                    if (face == 49 && player.PayVault.Has("bricknode"))
                    {
                        dobreak = false;
                    }
                }
                if (face >= 0 && (face <= 5 || face >= 11 || player.haveSmileyPackage) && !dobreak)
                {
                    player.face = face;
                    base.Broadcast("face", new object[]
                    {
                        player.Id,
                        m.GetInt(0U)
                    });
                }
            }
        }

        protected void handleAutoSay(Player player, Message m)
        {
            var offset = m.GetInt(0U);
            if (offset >= 0 && offset < this.autotexts.Length)
            {
                if (player.isguest)
                {
                    player.Send("info", new object[]
                    {
                        "Not logged in",
                        "Woops, you must be signed in to use the quick chat."
                    });
                }
                else if (DateTime.Now.Subtract(player.lastChat).TotalMilliseconds < 500.0)
                {
                    player.Send("write", new object[]
                    {
                        "SYSTEM",
                        "You are trying to chat way to fast, spamming the chat is not nice!"
                    });
                }
                else
                {
                    player.lastChat = DateTime.Now;
                    base.Broadcast("autotext", new object[]
                    {
                        player.Id,
                        this.autotexts[offset]
                    });
                }
            }
        }

        protected void handleSay(Player player, Message m)
        {
            var text = Regex.Replace(m.GetString(0U), "[^\\u0000-\\u02AF]", "").Trim();
            if (text.Length <= 80)
            {
                if (text.StartsWith("/"))
                {
                    var words = text.Split(new char[]
                    {
                        ' '
                    });
                    if (player.owner || player.canbemod)
                    {
                        if (words[0] == "/loadlevel")
                        {
                            if (this.owned && this.ownedWorld != null)
                            {
                                this.baseworld.reload();
                                var mess = Message.Create("reset", new object[0]);
                                this.baseworld.addToMessageAsComplexList(mess);
                                base.Broadcast(mess);
                                this.ResetPlayers();
                                return;
                            }
                        }
                        if (words[0] == "/stats" && player.canbemod)
                        {
                            player.Send("write", new object[]
                            {
                                "SYSTEM",
                                string.Concat(new object[]
                                {
                                    "coinscollected: ",
                                    this.coinscollected,
                                    " \ncoinbanned: ",
                                    this.coinbanned
                                })
                            });
                        }
                        else if (words[0] == "/bancoins" && player.canbemod)
                        {
                            this.coinbanned = true;
                            if (this.owned)
                            {
                                this.ownedWorld.Set("coinbanned", true);
                                this.ownedWorld.Save(delegate ()
                                {
                                    player.Send("write", new object[]
                                    {
                                        "SYSTEM",
                                        "Room can no longer award coins"
                                    });
                                });
                            }
                        }
                        else
                        {
                            if (words[0] == "/reset")
                            {
                                if (this.owned)
                                {
                                    this.ResetPlayers();
                                    return;
                                }
                            }
                            if (words[0] == "/clearandsave")
                            {
                                if (this.owned && player.canbemod)
                                {
                                    this.clearAndClose(player, m);
                                    return;
                                }
                            }
                            if (words[0] == "/banuser" && player.canbemod)
                            {
                                this.banUser(player, m);
                            }
                            else if (words[0] == "/kick" && words.Length >= 2)
                            {
                                var wherekicked = false;
                                var reason = "";
                                for (var a = 2; a < words.Length; a++)
                                {
                                    reason = reason + words[a] + " ";
                                }
                                foreach (var p in base.Players)
                                {
                                    if (p.name.ToLower() == words[1].ToLower())
                                    {
                                        if (p.canbemod && !player.canbemod)
                                        {
                                            base.Broadcast("write", new object[]
                                            {
                                                this.systemName,
                                                player.name + " tried to kick a moderator, sadly it backfired and " + player.name + " is no longer with us."
                                            });
                                            player.Send("info", new object[]
                                            {
                                                "Tsk tsk tsk",
                                                "You tried to kick a moderator... Sadly it backfired and you lost the game!"
                                            });
                                            player.Disconnect();
                                        }
                                        else
                                        {
                                            if (!wherekicked)
                                            {
                                                if (p.name == player.name)
                                                {
                                                    base.Broadcast("write", new object[]
                                                    {
                                                        this.systemName,
                                                        player.name + " is no more" + ((reason.Trim() != "") ? (" - " + reason) : "")
                                                    });
                                                }
                                                else
                                                {
                                                    base.Broadcast("write", new object[]
                                                    {
                                                        this.systemName,
                                                        player.name + " kicked " + p.name + ((reason.Trim() != "") ? (" - " + reason) : "")
                                                    });
                                                }
                                            }
                                            this.addBan(p.ConnectUserId, 5);
                                            p.Send("info", new object[]
                                            {
                                                "You were kicked by " + player.name,
                                                (reason.Trim() != "") ? reason : "Tsk tsk tsk"
                                            });
                                            p.Disconnect();
                                        }
                                        wherekicked = true;
                                    }
                                }
                                if (!wherekicked)
                                {
                                    player.Send("write", new object[]
                                    {
                                        this.systemName,
                                        "Unknown user " + words[1]
                                    });
                                }
                            }
                            else
                            {
                                player.Send("info", new object[]
                                {
                                    "System Message",
                                    "Unknown command"
                                });
                            }
                        }
                    }
                    else
                    {
                        player.Send("info", new object[]
                        {
                            "System Message",
                            "You do not have command access in this room!"
                        });
                    }
                }
                else if (player.canchat)
                {
                    for (var a = 0; a < this.chatmessages.Length - 1; a++)
                    {
                        this.chatmessages[a] = this.chatmessages[a + 1];
                    }
                    this.chatmessages[this.chatmessages.Length - 1] = new ChatMessage(player.name, text);
                    if (!(text.Trim() == ""))
                    {
                        if (DateTime.Now.Subtract(player.lastChat).TotalMilliseconds < 500.0)
                        {
                            player.Send("write", new object[]
                            {
                                "SYSTEM",
                                "You are trying to chat way to fast, spamming the chat room is not nice!"
                            });
                        }
                        else
                        {
                            player.lastChat = DateTime.Now;
                            foreach (var p in base.Players)
                            {
                                if (p.canchat)
                                {
                                    p.Send("say", new object[]
                                    {
                                        player.Id,
                                        text
                                    });
                                }
                                else
                                {
                                    p.Send("say", new object[]
                                    {
                                        player.Id,
                                        ""
                                    });
                                }
                            }
                        }
                    }
                }
                else
                {
                    player.Send("info", new object[]
                    {
                        "Woops, you can not chat!",
                        "This account is not verified for chat. You can use the parental control option in the shop to verify your account.\nYou are however free to use the quickchat buttons!"
                    });
                }
            }
        }

        protected void banUser(Player player, Message m)
        {
            if (player.canbemod)
            {
                var text = m.GetString(0U);
                var words = text.ToLower().Split(new char[]
                {
                    ' '
                });
                if (words.Length < 2)
                {
                    player.Send("write", new object[]
                    {
                        "SYSTEM",
                        "You must define a user to ban"
                    });
                }
                base.PlayerIO.BigDB.Load("usernames", words[1], delegate (DatabaseObject o)
                {
                    if (o == null || o.GetString("owner", null) == null)
                    {
                        player.Send("write", new object[]
                        {
                            "SYSTEM",
                            "User " + words[1] + " not found"
                        });
                    }
                    else
                    {
                        this.PlayerIO.BigDB.Load("PlayerObjects", o.GetString("owner", "waggag"), delegate (DatabaseObject user)
                        {
                            if (o == null)
                            {
                                player.Send("write", new object[]
                                {
                                    "SYSTEM",
                                    "Crap, something went horriably wrong!... tell chris!"
                                });
                            }
                            else if (user.GetBool("isModerator", false))
                            {
                                player.Send("write", new object[]
                                {
                                    "SYSTEM",
                                    "Dude, stop that!"
                                });
                            }
                            else
                            {
                                user.Set("banned", true);
                                user.Set("ban_reason", string.Concat(new string[]
                                {
                                    "Banned from chat by ",
                                    player.name,
                                    " [",
                                    text,
                                    "]"
                                }));
                                user.Save(delegate ()
                                {
                                    player.Send("write", new object[]
                                    {
                                        "SYSTEM",
                                        "Player " + words[1] + " has been banned!"
                                    });
                                });
                            }
                        });
                    }
                });
            }
        }

        protected void clearAndClose(Player player, Message m)
        {
            if (player.canbemod)
            {
                this.baseworld.reset();
                base.Broadcast("clear", new object[]
                {
                    this.width,
                    this.height
                });
                this.baseworld.save(delegate
                {
                    player.Send("write", new object[]
                    {
                        "SYSTEM",
                        "Room has been reset and saved"
                    });
                });
            }
        }

        protected virtual bool canEdit(Player player, Message m)
        {
            return player.canEdit;
        }

        private bool isBrickAllowed(uint id, int layer)
        {
            if (layer == 0)
            {
                if ((id < 0U || id > 255U) && id != 1000U)
                {
                    return false;
                }
                if (id > 83U && id != 100U && id != 101U && id < 218U)
                {
                    return false;
                }
            }
            else if ((id < 500U || id > 544U) && id != 0U)
            {
                return false;
            }
            return true;
        }

        private void placeBrick(Player player, Message m)
        {
            if (this.canEdit(player, m))
            {
                if ((DateTime.Now - player.lastEdit).TotalMilliseconds < 15.0)
                {
                    player.threshold -= 10;
                }
                else if (player.threshold < 250)
                {
                    player.threshold += 25;
                }
                if (player.threshold < 0)
                {
                    player.threshold = 0;
                }
                else
                {
                    player.lastEdit = DateTime.Now;
                    var layerNum = m.GetInt(0U);
                    var cx = (uint)m.GetInt(1U);
                    var cy = (uint)m.GetInt(2U);
                    var brick = (uint)m.GetInt(3U);
                    if (this.isBrickAllowed(brick, layerNum))
                    {
                        var margin = 1;
                        if (player.canbemod)
                        {
                            margin = 0;
                        }
                        if (brick == 44U)
                        {
                            margin = 0;
                        }
                        if (brick > 8U && brick < 16U)
                        {
                            margin = 0;
                        }
                        if ((ulong)cx >= (ulong)((long)margin) && (ulong)cx <= (ulong)((long)(this.width - (1 + margin))) && (ulong)cy <= (ulong)((long)(this.height - (1 + margin))) && ((this.editkey != "") ? ((ulong)cy >= (ulong)((long)margin)) : (cy >= 5U)))
                        {
                            var num = brick;
                            if (num <= 243U)
                            {
                                switch (num)
                                {
                                    case 60U:
                                        this.setBrick(layerNum, cx, cy, brick);
                                        goto IL_9CE;
                                    case 61U:
                                    case 62U:
                                    case 63U:
                                    case 64U:
                                    case 65U:
                                    case 66U:
                                    case 67U:
                                        goto IL_583;
                                    case 68U:
                                    case 69U:
                                        goto IL_4FF;
                                    case 70U:
                                    case 71U:
                                    case 72U:
                                    case 73U:
                                    case 74U:
                                    case 75U:
                                    case 76U:
                                        if (player.PayVault.Has("brickminiral"))
                                        {
                                            this.setBrick(layerNum, cx, cy, brick);
                                        }
                                        goto IL_9CE;
                                    case 77U:
                                        if (this.owned && player.PayVault.Has("bricknode"))
                                        {
                                            this.setBrickSound(ItemTypes.Piano, cx, cy, (uint)m.GetInt(4U));
                                        }
                                        goto IL_9CE;
                                    case 78U:
                                    case 79U:
                                    case 80U:
                                    case 81U:
                                    case 82U:
                                        break;
                                    case 83U:
                                        if (this.owned && player.PayVault.Has("brickdrums"))
                                        {
                                            this.setBrickSound(ItemTypes.Drums, cx, cy, (uint)m.GetInt(4U));
                                        }
                                        goto IL_9CE;
                                    default:
                                        switch (num)
                                        {
                                            case 218U:
                                            case 219U:
                                            case 220U:
                                            case 221U:
                                            case 222U:
                                                break;
                                            case 223U:
                                                if (player.PayVault.Has("brickhwtrophy"))
                                                {
                                                    this.setBrick(layerNum, cx, cy, brick);
                                                }
                                                goto IL_9CE;
                                            case 224U:
                                            case 225U:
                                            case 226U:
                                                goto IL_4FF;
                                            default:
                                                switch (num)
                                                {
                                                    case 241U:
                                                        if (this.owned && (player.owner || player.canbemod))
                                                        {
                                                            this.setBrick(layerNum, cx, cy, brick);
                                                        }
                                                        goto IL_9CE;
                                                    case 242U:
                                                        if (this.owned && (player.owner || player.canbemod))
                                                        {
                                                            var rotation = (uint)m.GetInt(4U);
                                                            var id = (uint)m.GetInt(5U);
                                                            var target = (uint)m.GetInt(6U);
                                                            if (rotation >= 4U)
                                                            {
                                                                rotation -= 4U;
                                                            }
                                                            if (rotation < 0U)
                                                            {
                                                                rotation += 4U;
                                                            }
                                                            this.setBrick(cx, cy, brick, rotation, id, target);
                                                        }
                                                        goto IL_9CE;
                                                    case 243U:
                                                        if (this.owned && (player.owner || player.canbemod) && player.PayVault.Has("bricksecret"))
                                                        {
                                                            this.setBrick(layerNum, cx, cy, brick);
                                                        }
                                                        goto IL_9CE;
                                                    default:
                                                        goto IL_583;
                                                }
                                                break;
                                        }
                                        break;
                                }
                                if (player.PayVault.Has("brickxmas2011"))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                                goto IL_9CE;
                            }
                            if (num == 255U)
                            {
                                if (this.owned && (player.owner || player.canbemod))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                                goto IL_9CE;
                            }
                            switch (num)
                            {
                                case 541U:
                                case 542U:
                                    break;
                                default:
                                    if (num != 1000U)
                                    {
                                        goto IL_583;
                                    }
                                    if (this.owned && player.canbemod)
                                    {
                                        this.setBrickLabel(cx, cy, m.GetString(4U));
                                    }
                                    goto IL_9CE;
                            }
                            IL_4FF:
                            if (player.PayVault.Has("brickhw2011"))
                            {
                                this.setBrick(layerNum, cx, cy, brick);
                            }
                            goto IL_9CE;
                            IL_583:
                            if (brick == 59U || (brick >= 228U && brick <= 232U))
                            {
                                if (player.PayVault.Has("bricksummer2011"))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                            }
                            else if (brick >= 249U && brick <= 254U)
                            {
                                if (player.PayVault.Has("brickchristmas2010"))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                            }
                            else if (brick >= 244U && brick <= 248U)
                            {
                                if (player.PayVault.Has("mixednewyear2010"))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                            }
                            else if (brick == 243U)
                            {
                                if (player.PayVault.Has("bricksecret"))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                            }
                            else if (brick >= 233U && brick <= 240U)
                            {
                                if (player.PayVault.Has("brickspring2011"))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                            }
                            else if (brick >= 45U && brick <= 49U)
                            {
                                if (player.PayVault.Has("brickfactorypack"))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                            }
                            else if (brick >= 51U && brick <= 58U)
                            {
                                if (player.PayVault.Has("brickglass"))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                            }
                            else if (brick == 43U)
                            {
                                var count = (uint)m.GetInt(4U);
                                if (count > 99U)
                                {
                                    count = 99U;
                                }
                                if (this.owned && (player.owner || player.canbemod))
                                {
                                    this.setBrick(cx, cy, brick, count);
                                }
                            }
                            else if (brick == 44U)
                            {
                                if (player.PayVault.Has("brickblackblock"))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                            }
                            else if (brick >= 37U && brick <= 42U)
                            {
                                if (player.haveSmileyPackage)
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                            }
                            else if (brick == 50U)
                            {
                                if (player.PayVault.Has("bricksecret") && this.owned && (player.owner || player.canbemod))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                            }
                            else if (brick >= 513U && brick <= 519U)
                            {
                                if (player.PayVault.Has("brickbgchecker"))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                            }
                            else if (brick >= 520U && brick <= 526U)
                            {
                                if (player.PayVault.Has("brickbgdark"))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                            }
                            else if (brick >= 527U && brick <= 532U)
                            {
                                if (player.PayVault.Has("brickbgpastel"))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                            }
                            else if (brick >= 533U && brick <= 538U)
                            {
                                if (player.PayVault.Has("brickbgcanvas"))
                                {
                                    this.setBrick(layerNum, cx, cy, brick);
                                }
                            }
                            else
                            {
                                this.setBrick(layerNum, cx, cy, brick);
                            }
                            IL_9CE:;
                        }
                    }
                }
            }
        }

        private void setBrick(int layerNum, uint x, uint y, uint brick)
        {
            if (this.baseworld.setBrick(layerNum, x, y, brick))
            {
                base.Broadcast(Message.Create("b", new object[]
                {
                    layerNum,
                    x,
                    y,
                    brick
                }));
            }
        }

        private void setBrick(uint x, uint y, uint brick, uint goal)
        {
            if (this.baseworld.setBrickCoindoor(x, y, goal, true))
            {
                base.Broadcast(Message.Create("bc", new object[]
                {
                    x,
                    y,
                    brick,
                    goal
                }));
            }
        }

        private void setBrickSound(ItemTypes type, uint x, uint y, uint offset)
        {
            if (this.baseworld.setBrickSound(type, x, y, offset))
            {
                base.Broadcast(Message.Create("bs", new object[]
                {
                    x,
                    y,
                    (uint)type,
                    offset
                }));
            }
        }

        private void setBrickLabel(uint x, uint y, string text)
        {
            if (this.baseworld.setBrickLabel(x, y, text))
            {
                base.Broadcast(Message.Create("lb", new object[]
                {
                    x,
                    y,
                    1000,
                    text
                }));
            }
        }

        private void setBrick(uint x, uint y, uint brick, uint rotation, uint id, uint target)
        {
            if (this.baseworld.setBrickPortal(x, y, rotation, id, target, true))
            {
                base.Broadcast(Message.Create("pt", new object[]
                {
                    x,
                    y,
                    brick,
                    rotation,
                    id,
                    target
                }));
            }
        }

        protected void sendInitMessage(Player player)
        {
            var d = this.baseworld.getSpawn();
            player.x = (double)(d.x * 16);
            player.y = (double)(d.y * 16);
            if (player.ConnectUserId == this.levelOwner || (player.canbemod && base.RoomId == "tutorialWorld"))
            {
                player.canEdit = true;
                player.owner = true;
            }
            var count = 0;
            foreach (var p in base.Players)
            {
                if (p.ConnectUserId == player.ConnectUserId && !player.isguest)
                {
                    count++;
                }
            }
            if (count >= 2)
            {
                if (!player.owner)
                {
                    player.Send("info", new object[]
                    {
                        "Limit reached",
                        "To prevent abuse you can only be connected to the same level once."
                    });
                    player.Disconnect();
                    return;
                }
                if (count >= 3)
                {
                    player.Send("info", new object[]
                    {
                        "Limit reached",
                        "To prevent abuse you can only be connected to your own level twice."
                    });
                    player.Disconnect();
                    return;
                }
            }
            foreach (var p in base.Players)
            {
                if (p != player)
                {
                    p.Send("add", new object[]
                    {
                        player.Id,
                        player.name,
                        player.face,
                        player.x,
                        player.y,
                        player.isgod,
                        player.ismod,
                        player.canchat
                    });
                }
            }
            var roomname = "";
            var plays = "";
            base.RoomData.TryGetValue("name", out roomname);
            base.RoomData.TryGetValue("plays", out plays);
            var initmessage = Message.Create("init", new object[]
            {
                this.levelOwnerName,
                roomname ?? "Untitled Room",
                plays,
                this.Rot13(this.editchar),
                player.Id,
                player.x,
                player.y,
                player.name,
                player.canEdit,
                player.owner,
                this.width,
                this.height,
                this.isTutorialRoom
            });
            this.baseworld.addToMessageAsComplexList(initmessage);
            player.Send(initmessage);
        }

        protected string Rot13(string value)
        {
            var array = value.ToCharArray();
            for (var i = 0; i < array.Length; i++)
            {
                var number = (int)array[i];
                if (number >= 97 && number <= 122)
                {
                    if (number > 109)
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                else if (number >= 65 && number <= 90)
                {
                    if (number > 77)
                    {
                        number -= 13;
                    }
                    else
                    {
                        number += 13;
                    }
                }
                array[i] = (char)number;
            }
            return new string(array);
        }

        protected double getTime()
        {
            return Math.Round((DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds);
        }

        protected int version = 89;

        protected string editkey = "";

        protected int crownid = -1;

        protected bool lockedroom = false;

        protected bool red = false;

        protected double redtime;

        protected bool green = false;

        protected double greentime;

        protected bool blue = false;

        protected double bluetime;

        protected Dictionary<string, int> ips = new Dictionary<string, int>();

        protected int plays = 0;

        protected int sessionplays = 0;

        protected string editchar = ".";

        protected bool die = false;

        protected bool upgrade = false;

        protected bool owned = false;

        protected DatabaseObject ownedWorld;

        protected bool ready = false;

        protected bool isbetalevel = false;

        protected string systemName = "* System";

        protected List<Ban> bans = new List<Ban>();

        protected int width = 200;

        protected int height = 200;

        protected string levelOwner = "";

        protected int coinscollected = 0;

        protected World baseworld = new World();

        protected Random rd = new Random();

        protected bool coinbanned = false;

        protected ChatMessage[] chatmessages = new ChatMessage[10];

        protected string levelOwnerName = "";

        protected bool isTutorialRoom = false;

        private DateTime coinanticheat = DateTime.Now;

        protected string[] autotexts = new string[]
        {
            "Left.",
            "Hi.",
            "Goodbye.",
            "Help me!",
            "Thank you.",
            "Follow me.",
            "Stop!",
            "Yes.",
            "No.",
            "Right."
        };
    }
}
