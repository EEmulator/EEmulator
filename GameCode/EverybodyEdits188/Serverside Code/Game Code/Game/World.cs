using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    public class World
    {
        private readonly Client client;
        private List<Item> bluecoindoors = new List<Item>();
        private List<Item> bluecoingates = new List<Item>();
        private BlockMap brickMap;
        private List<Item> cakes = new List<Item>();
        private List<Item> checkpoints = new List<Item>();
        private List<Item> coindoors = new List<Item>();
        private List<Item> coingates = new List<Item>();
        private List<Item> coins = new List<Item>();
        private List<Item> completes = new List<Item>();

        private Brick[,] data;
        private Brick[,] dataBg;

        private DatabaseObject dbo;
        private List<Item> deathdoorsgates = new List<Item>();
        private List<Item> diamonds = new List<Item>();
        private List<Item> fireHazards = new List<Item>();
        private string forcedKey;
        private List<Item> holograms = new List<Item>();
        private List<Item> invisibleportals = new List<Item>();
        //public int width;
        //public int height;

        private bool isRefreshing;
        private List<Item> portals = new List<Item>();
        private List<Item> purpledoorgates = new List<Item>();
        private bool repeatRefresh;
        private int spawnOffset;
        private List<Item> spawns = new List<Item>();
        private List<Item> spikes = new List<Item>();
        private List<Item> textsigns = new List<Item>();
        private List<Item> timedoors = new List<Item>();
        private int wootque;
        private WootWorldStatus wootstatus;
        private List<Item> worldportals = new List<Item>();
        private List<Item> zombiedoors = new List<Item>();

        public World(Client c)
        {
            this.client = c;
            //width = 200;
            //height = 200;
            this.data = new Brick[this.height, this.width];
            this.dataBg = new Brick[this.height, this.width];

            this.brickMap = new BlockMap();
            /*
            worldPresentation = new WorldPresentation(client);
            */
            this.reset();
        }

        // Used to save wootstatus about open worlds
        //private List<int> currentwoots = new List<int>();

        /*
        private WorldPresentation worldPresentation;
        */

        public string key
        {
            get
            {
                if (this.dbo != null) return this.dbo.Key;
                if (this.forcedKey != null) return this.forcedKey;
                return "";
            }
            set { this.forcedKey = value; }
        }

        public int type
        {
            get
            {
                if (this.dbo != null) return this.dbo.GetInt("type", 0);
                return 0;
            }
            set { if (this.dbo != null) this.dbo.Set("type", value); }
        }

        public int width
        {
            get
            {
                // default width is also the width of an open world
                if (this.dbo != null) return this.dbo.GetInt("width", 200);
                return 200;
            }
            set { if (this.dbo != null) this.dbo.Set("width", value); }
        }

        public int height
        {
            get
            {
                // default height is also the height of an open world
                if (this.dbo != null) return this.dbo.GetInt("height", 200);
                return 200;
            }
            set { if (this.dbo != null) this.dbo.Set("height", value); }
        }

        public int plays
        {
            get
            {
                if (this.dbo != null) return this.dbo.GetInt("plays", 0);
                return 0;
            }
            set { if (this.dbo != null) this.dbo.Set("plays", value); }
        }

        public int woots
        {
            get
            {
                if (this.wootstatus != null) return this.wootstatus.getWoots();
                if (this.dbo != null) return this.dbo.GetInt("woots", 0);
                return 0;
            }
        }

        public int totalwoots
        {
            get
            {
                if (this.wootstatus != null) return this.wootstatus.getTotalWoots();
                if (this.dbo != null) return this.dbo.GetInt("totalwoots", 0);
                return 0;
            }
        }

        public string ownerid
        {
            get
            {
                if (this.dbo != null) return this.dbo.GetString("owner", "");
                return "";
            }
            //set {
            //    if (dbo != null) dbo.Set("owner", value);
            //}
        }

        public string name
        {
            get
            {
                if (this.dbo != null) return this.dbo.GetString("name", "Untitled World");
                return "Untitled World";
            }
            set { if (this.dbo != null) this.dbo.Set("name", value); }
        }

        public bool coinbanned
        {
            get
            {
                if (this.dbo != null) return this.dbo.GetBool("coinbanned", false);
                return false;
            }
            set { if (this.dbo != null) this.dbo.Set("coinbanned", value); }
        }

        public bool wootupbanned
        {
            get
            {
                if (this.dbo != null) return this.dbo.GetBool("wootupbanned", false);
                return false;
            }
            set { if (this.dbo != null) this.dbo.Set("wootupbanned", value); }
        }

        public bool visible
        {
            get
            {
                if (this.dbo != null) return this.dbo.GetBool("visible", true);
                return false;
            }
            set { if (this.dbo != null) this.dbo.Set("visible", value); }
        }

        public bool IsFeatured
        {
            get
            {
                if (this.dbo != null)
                {
                    return this.dbo.GetBool("IsFeatured", false);
                }
                return false;
            }
        }

        public double gravityMultiplier
        {
            get
            {
                if (this.type == (int)WorldTypes.MoonLarge) return 0.16;
                return 1;
            }
        }

        // As default, all potions are allowed.

        public bool allowPotions
        {
            get
            {
                if (this.dbo == null) return true;
                return this.dbo.GetBool("allowpotions", false);
            }
            set { if (this.dbo != null) this.dbo.Set("allowpotions", value); }
        }

        public uint backgroundColor
        {
            get
            {
                if (this.dbo == null) return 0x00000000;
                return this.dbo.GetUInt("backgroundColor", 0x00000000);
            }
            set { if (this.dbo != null) this.dbo.Set("backgroundColor", value); }
        }

        public uint borderType
        {
            get
            {
                switch (this.type)
                {
                    case (int)WorldTypes.MoonLarge:
                    {
                        return 182;
                    }
                    default:
                    {
                        return 9;
                    }
                }
            }
        }

        public uint fillType
        {
            get
            {
                switch (this.type)
                {
                    default:
                    {
                        return 0;
                    }
                }
            }
        }

        public bool isPotionEnabled(int potionid)
        {
            return isPotionEnabled(potionid.ToString());
        }

        public bool isPotionEnabled(string potionid)
        {
            if (!this.allowPotions) return false;
            if (this.dbo == null) return true;
            if (!this.dbo.Contains("enabledpotions")) return true;
            var enabledpotions = this.dbo.GetObject("enabledpotions");
            return enabledpotions.GetBool(potionid, true);
        }

        public void setPotionsEnabled(List<string> potionids, bool enabled)
        {
            if (this.dbo == null) return;
            if (!this.dbo.Contains("enabledpotions")) this.dbo.Set("enabledpotions", new DatabaseObject());
            var enabledpotions = this.dbo.GetObject("enabledpotions");

            for (var i = 0; i < potionids.Count; i++)
            {
                enabledpotions.Set(potionids[i], enabled);
            }
        }

        public List<string> getPotionsEnabled(bool enabled)
        {
            var enabledids = new List<string>();
            if (this.dbo == null || !this.dbo.Contains("enabledpotions")) return enabledids;

            var enabledpotions = this.dbo.GetObject("enabledpotions");

            foreach (var potionid in enabledpotions.Properties)
            {
                if (enabled == enabledpotions.GetBool(potionid, true))
                {
                    enabledids.Add(potionid);
                }
            }
            return enabledids;
        }

        public DatabaseObject getDatabaseObject()
        {
            return this.dbo;
        }

        //protected int getTime() {
        //    return Convert.ToInt32((DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds);
        //}

        public void fromDatabaseObject(DatabaseObject dbo)
        {
            this.dbo = dbo;

            //200 is the default size if it's not known
            //width = dbo.GetInt("width", 200);
            //height = dbo.GetInt("height", 200);

            // If world is created before 2012-08-30, it does not contain type. 
            if (!dbo.Contains("type"))
            {
                if (this.width == 25 && this.height == 25) this.type = (int)WorldTypes.Small;
                else if (this.width == 50 && this.height == 50) this.type = (int)WorldTypes.Medium;
                else if (this.width == 100 && this.height == 100) this.type = (int)WorldTypes.Large;
                else if (this.width == 200 && this.height == 200) this.type = (int)WorldTypes.Massive;
                else if (this.width == 400 && this.height == 50) this.type = (int)WorldTypes.Wide;
                else if (this.width == 400 && this.height == 200) this.type = (int)WorldTypes.Great;
                else if (this.width == 100 && this.height == 400) this.type = (int)WorldTypes.Tall;
                else if (this.width == 636 && this.height == 50) this.type = (int)WorldTypes.UltraWide;
                else if (this.width == 150 && this.height == 25) this.type = (int)WorldTypes.Tutorial;
                else if (this.width == 110 && this.height == 110) this.type = (int)WorldTypes.MoonLarge;
            }

            if (!dbo.Contains("allowpotions"))
            {
                this.allowPotions = true;
            }

            //Create new uninitalized world object
            this.data = new Brick[this.height, this.width];
            this.dataBg = new Brick[this.height, this.width];

            //Load bytes from db. (legacy format, obsolete but maybe still used on some levels)
            var worlddata = dbo.GetArray("worlddata");
            this.reset();
            if (worlddata != null)
            {
                this.unserializeFromComplexObject(worlddata);
            }
            else
            {
                var worldbytes = dbo.GetBytes("world", null);
                if (worldbytes != null)
                {
                    this.unserializeWorldFromBytes(worldbytes);
                }
            }

            this.refresh();
            /*
            loadWorldPresentation();
            */
        }

        public void addWoot()
        {
            //if (this.wootstatus == null)
            //{
            //    this.wootque++;
            //    return;
            //}
            //this.wootstatus.addWoot();
        }

        public void refreshWoots(Callback callback)
        {
            //if (this.wootstatus != null)
            //{
            //    this.wootstatus.getWoots(true);
            //    callback();
            //}
            //else if (this.key.Length > 0)
            //{
            //    WootWorldStatus.getWorldWootStatus(this.client, this.key, delegate(WootWorldStatus status)
            //    {
            //        this.wootstatus = status;
            //        for (var q = 0; q < this.wootque; q++)
            //        {
            //            this.addWoot();
            //        }
            //        this.wootstatus.save();
            //        callback();
            //    });
            //}
        }

        public void unserializeFromComplexObject(DatabaseArray worlddata)
        {
            DatabaseObject ct;
            for (var a = 0; a < worlddata.Count; a++)
            {
                if (!worlddata.Contains(a) || worlddata.GetObject(a).Count == 0) continue;
                ct = worlddata.GetObject(a);
                var type = (uint)ct.GetValue("type");
                var layerNum = ct.GetInt("layer", 0);
                var xs = ct.GetBytes("x", new byte[0]);
                var ys = ct.GetBytes("y", new byte[0]);

                for (var b = 0; b < xs.Length; b += 2)
                {
                    var nx = (uint)((xs[b] << 8) + xs[b + 1]);
                    var ny = (uint)((ys[b] << 8) + ys[b + 1]);

                    switch (type)
                    {
                        case (uint)ItemTypes.CoinDoor:
                        {
                            this.setBrickCoindoor(
                                nx,
                                ny,
                                (uint)ct.GetValue("goal"),
                                false
                                );
                            break;
                        }

                        case (uint)ItemTypes.CoinGate:
                        {
                            this.setBrickCoingate(
                                nx,
                                ny,
                                (uint)ct.GetValue("goal"),
                                false
                                );
                            break;
                        }

                        case (uint)ItemTypes.BlueCoinDoor:
                        {
                            this.setBrickBlueCoindoor(
                                nx,
                                ny,
                                (uint)ct.GetValue("goal"),
                                false
                                );
                            break;
                        }

                        case (uint)ItemTypes.BlueCoinGate:
                        {
                            this.setBrickBlueCoingate(
                                nx,
                                ny,
                                (uint)ct.GetValue("goal"),
                                false
                                );
                            break;
                        }

                        case (uint)ItemTypes.DeathDoor:
                        {
                            this.setBrickDeathDoor(
                                nx,
                                ny,
                                (uint)ct.GetValue("goal"),
                                false
                                );
                            break;
                        }

                        case (uint)ItemTypes.DeathGate:
                        {
                            this.setBrickDeathGate(
                                nx,
                                ny,
                                (uint)ct.GetValue("goal"),
                                false
                                );
                            break;
                        }

                        case (uint)ItemTypes.DoorPurple:
                        {
                            this.setBrickDoorPurple(
                                nx,
                                ny,
                                ct.GetUInt("goal", 0),
                                false
                                );
                            break;
                        }
                        case (uint)ItemTypes.GatePurple:
                        {
                            this.setBrickGatePurple(
                                nx,
                                ny,
                                ct.GetUInt("goal", 0),
                                false
                                );
                            break;
                        }
                        case (uint)ItemTypes.SwitchPurple:
                        {
                            this.setBrickSwitchPurple(
                                nx,
                                ny,
                                ct.GetUInt("goal", 0),
                                false
                                );
                            break;
                        }

                        case (uint)ItemTypes.PortalInvisible:
                        case (uint)ItemTypes.Portal:
                        {
                            this.setBrickPortal(
                                type,
                                nx,
                                ny,
                                ct.GetUInt("rotation", 0),
                                ct.GetUInt("id", 0),
                                ct.GetUInt("target", 0),
                                false
                                );
                            break;
                        }

                        case (uint)ItemTypes.WorldPortal:
                        {
                            this.setBrickWorldPortal(
                                nx,
                                ny,
                                ct.GetString("target", ""),
                                false
                                );
                            break;
                        }

                        case (uint)ItemTypes.GlowyLineBlueStraight:
                        case (uint)ItemTypes.GlowyLineBlueSlope:
                        case (uint)ItemTypes.GlowyLineGreenSlope:
                        case (uint)ItemTypes.GlowyLineGreenStraight:
                        case (uint)ItemTypes.GlowyLineYellowSlope:
                        case (uint)ItemTypes.GlowyLineYellowStraight:
                        case (uint)ItemTypes.OnewayCyan:
                        case (uint)ItemTypes.OnewayRed:
                        case (uint)ItemTypes.OnewayYellow:
                        case (uint)ItemTypes.OnewayPink:
                        case (uint)ItemTypes.Spike:
                        {
                            this.setBrickRotateable(
                                layerNum,
                                nx,
                                ny,
                                type,
                                ct.GetUInt("rotation", 0),
                                false
                                );
                            break;
                        }

                        case 1000:
                        {
                            this.setBrickLabel(
                                nx,
                                ny,
                                ct.GetString("text", "no text found"),
                                ct.GetString("text_color", "#FFFFFF")
                                );
                            break;
                        }

                        case (int)ItemTypes.Piano:
                        {
                            this.setBrickSound(
                                ItemTypes.Piano,
                                nx,
                                ny,
                                ct.GetUInt("id", 0)
                                );
                            break;
                        }

                        case (int)ItemTypes.Drums:
                        {
                            this.setBrickSound(
                                ItemTypes.Drums,
                                nx,
                                ny,
                                ct.GetUInt("id", 0)
                                );
                            break;
                        }

                        case (int)ItemTypes.Complete:
                        {
                            this.setBrickComplete(nx,
                                ny);
                            break;
                        }
                        case (int)ItemTypes.TextSign:
                        {
                            this.setBrickTextSign(nx, ny, ct.GetString("text", "no text found"), false);
                            break;
                        }

                        default:
                        {
                            this.setBrick(layerNum, nx, ny, type, true);
                            break;
                        }
                    }
                }
            }
        }

        public void save(bool saveworlddata, Callback callback = null)
        {
            if (saveworlddata) this.saveWorldData();

            if (this.dbo != null)
            {
                //saving a "cached" woots/totalwoots count
                if (this.wootstatus != null)
                {
                    this.dbo.Set("woots", this.wootstatus.getWoots(true));
                    this.dbo.Set("totalwoots", this.wootstatus.getTotalWoots());
                    this.wootstatus.save();
                }
                this.dbo.Save(callback);
            }
        }

        public void saveWorldData()
        {
            if (this.dbo == null) return;

            var bricks = this.getBrickList(0); // Get a list og foreground bricks...
            bricks.AddRange(this.getBrickList(1)); // ...add the list of background bricks

            var worlddata = new DatabaseArray();

            foreach (var b in bricks)
            {
                var cb = new DatabaseObject();

                var l = b.xs.Count;

                var xs = new byte[l * 2];
                var ys = new byte[l * 2];

                for (var a = 0; a < l; a++)
                {
                    xs[a * 2] = (byte)((b.xs[a] & 0x0000ff00) >> 8);
                    xs[a * 2 + 1] = (byte)(b.xs[a] & 0x000000ff);

                    ys[a * 2] = (byte)((b.ys[a] & 0x0000ff00) >> 8);
                    ys[a * 2 + 1] = (byte)(b.ys[a] & 0x000000ff);
                    /*
                    // Check if block is within camera viewport
                    if (b.xs[a] > worldPresentation.ViewportX && b.xs[a] < (worldPresentation.ViewportX + worldPresentation.ViewportWidth))
                    {

                    }
                    */
                }

                cb.Set("type", b.type);
                cb.Set("layer", b.layer);
                cb.Set("x", xs);
                cb.Set("y", ys);

                switch (b.type)
                {
                    // Coin door
                    case (uint)ItemTypes.CoinDoor:
                    case (uint)ItemTypes.CoinGate:
                    case (uint)ItemTypes.BlueCoinDoor:
                    case (uint)ItemTypes.BlueCoinGate:
                    case (uint)ItemTypes.DeathDoor:
                    case (uint)ItemTypes.DeathGate:
                    case (uint)ItemTypes.SwitchPurple:
                    case (uint)ItemTypes.GatePurple:
                    case (uint)ItemTypes.DoorPurple:
                    {
                        cb.Set("goal", b.goal);
                        break;
                    }

                    // Portals
                    case (uint)ItemTypes.PortalInvisible:
                    case (uint)ItemTypes.Portal:
                    {
                        cb.Set("rotation", b.rotation);
                        cb.Set("id", b.id);
                        cb.Set("target", b.target);
                        break;
                    }

                    // World Portals
                    case (uint)ItemTypes.WorldPortal:
                    {
                        cb.Set("target", b.targetworld);
                        break;
                    }


                    // Spikes
                    case (uint)ItemTypes.Spike:
                    {
                        cb.Set("rotation", b.rotation);
                        break;
                    }
                    case (uint)ItemTypes.GlowyLineBlueStraight:
                    case (uint)ItemTypes.GlowyLineBlueSlope:
                    case (uint)ItemTypes.GlowyLineGreenSlope:
                    case (uint)ItemTypes.GlowyLineGreenStraight:
                    case (uint)ItemTypes.GlowyLineYellowSlope:
                    case (uint)ItemTypes.GlowyLineYellowStraight:
                    case (uint)ItemTypes.OnewayCyan:
                    case (uint)ItemTypes.OnewayRed:
                    case (uint)ItemTypes.OnewayYellow:
                    case (uint)ItemTypes.OnewayPink:
                    {
                        cb.Set("rotation", b.rotation);
                        break;
                    }

                    case (uint)ItemTypes.Label:
                    {
                        cb.Set("text", b.text);
                        cb.Set("text_color", b.text_color);
                        break;
                    }

                    case (uint)ItemTypes.TextSign:
                    {
                        cb.Set("text", b.text);
                        break;
                    }

                    case (uint)ItemTypes.Piano:
                    {
                        cb.Set("id", b.id);
                        break;
                    }

                    case (uint)ItemTypes.Drums:
                    {
                        cb.Set("id", b.id);
                        break;
                    }
                }
                worlddata.Add(cb);
            }

            this.dbo.Set("worlddata", worlddata);


            //Remove old database definition;
            if (this.dbo.Contains("world"))
            {
                this.dbo.Remove("world");
            }

            //dbo.Save(callback); 
        }


        private List<DBBrick> getBrickList(int layer)
        {
            var dbbricks = new List<DBBrick>();
            var layerData = (Brick[,])(layer == 0 ? this.data : this.dataBg).Clone();

            for (uint y = 0; y < this.height; y++)
            {
                for (uint x = 0; x < this.width; x++)
                {
                    var cur = layerData[y, x];

                    // Set the presentation camera properties (using the spike tile as a camera for now)
                    /*
                    if (cur.type == (uint)ItemTypes.Spike)
                    {
                        worldPresentation.StartX = x;
                        worldPresentation.StartY = y;
                    }
                    */
                    if (cur.type == 0) continue;

                    // Do we have an existing instance of this type (with exact same attributes)?
                    // (Attributes are only stored pr. object list in the DB, so treat i.e. coin doors with different goals as different types)
                    DBBrick dbb = null;

                    foreach (var b in dbbricks)
                    {
                        if (b.equalBrickValues(cur))
                        {
                            dbb = b;
                            break;
                        }
                    }

                    if (dbb == null)
                    {
                        dbb = new DBBrick(cur);
                        dbb.layer = layer;
                        dbbricks.Add(dbb);
                    }

                    dbb.xs.Add(x);
                    dbb.ys.Add(y);
                }
            }

            return dbbricks;
        }

        // gather all text signs and return them - this is used by the report command to document the current text of a sign
        // This is only invoked when the command is executed
        public List<DBBrick> GetBrickTextSignList()
        {
            var signs = new List<DBBrick>();
            var bricks = this.getBrickList(0); // signs are always in layer 0
            foreach (var b in bricks)
            {
                if (b.type == (int)ItemTypes.TextSign)
                {
                    signs.Add(b);
                }
            }
            return signs;
        }


        // This is a overridden version of getBrickList to parameterize width and height
        /*
        private List<DBBrick> getBrickList(int layer, uint startx, uint starty, uint pwidth, uint pheight)
        {
            List<DBBrick> dbbricks = new List<DBBrick>();
            Brick[,] layerData = (layer == 0 ? data : dataBg);
            for (uint y = starty; y < pheight; y++)
            {
                for (uint x = startx; x < pwidth; x++)
                {
                    Brick cur = layerData[y, x];
                    if (cur.type == 0) continue;

                    // Do we have an existing instance of this type (with exact same attributes)?
                    // (Attributes are only stored pr. object list in the DB, so treat i.e. coin doors with different goals as different types)
                    DBBrick dbb = null;

                    foreach (DBBrick b in dbbricks)
                    {
                        if (b.equalBrickValues(cur))
                        { dbb = b; break; }
                    }

                    if (dbb == null)
                    {
                        dbb = new DBBrick(cur);
                        dbb.layer = layer;
                        dbbricks.Add(dbb);
                    }

                    dbb.xs.Add(x);
                    dbb.ys.Add(y);


                }
            }

            return dbbricks;
        }
        */

        private void refresh()
        {
            if (this.isRefreshing)
            {
                this.repeatRefresh = true;
                return;
            }
            this.isRefreshing = true;

            this.coins = new List<Item>();
            this.spawns = new List<Item>();
            this.checkpoints = new List<Item>();
            this.coindoors = new List<Item>();
            this.coingates = new List<Item>();
            this.bluecoindoors = new List<Item>();
            this.bluecoingates = new List<Item>();
            this.deathdoorsgates = new List<Item>();
            this.timedoors = new List<Item>();
            this.purpledoorgates = new List<Item>();
            this.portals = new List<Item>();
            this.diamonds = new List<Item>();
            this.cakes = new List<Item>();
            this.holograms = new List<Item>();
            this.completes = new List<Item>();
            this.spikes = new List<Item>();
            this.fireHazards = new List<Item>();
            this.zombiedoors = new List<Item>();
            this.worldportals = new List<Item>();
            this.invisibleportals = new List<Item>();
            this.textsigns = new List<Item>();

            for (var y = 0; y < this.height; y++)
            {
                for (var x = 0; x < this.width; x++)
                {
                    if (this.data[y, x] == null) continue;

                    // Build "Coins" list
                    if (this.data[y, x].type == (int)ItemTypes.Coin)
                    {
                        this.coins.Add(new Item((int)ItemTypes.Coin, x, y));
                    }

                    // Build "Checkpoint points" list
                    if (this.data[y, x].type == (int)ItemTypes.Checkpoint)
                    {
                        this.checkpoints.Add(new Item((int)ItemTypes.Checkpoint, x, y));
                    }

                    // Build "Spawn points" list
                    if (this.data[y, x].type == (int)ItemTypes.SpawnPoint)
                    {
                        this.spawns.Add(new Item((int)ItemTypes.SpawnPoint, x, y));
                    }

                    // Build "Coin doors" list
                    if (this.data[y, x].type == (int)ItemTypes.CoinDoor)
                    {
                        this.coindoors.Add(new Item((int)ItemTypes.CoinDoor, x, y));
                    }

                    // Build "Coin Gates" list
                    if (this.data[y, x].type == (int)ItemTypes.CoinGate)
                    {
                        this.coingates.Add(new Item((int)ItemTypes.CoinGate, x, y));
                    }

                    // Build "Coin doors" list
                    if (this.data[y, x].type == (int)ItemTypes.BlueCoinDoor)
                    {
                        this.bluecoindoors.Add(new Item((int)ItemTypes.BlueCoinDoor, x, y));
                    }

                    // Build "Coin Gates" list
                    if (this.data[y, x].type == (int)ItemTypes.BlueCoinGate)
                    {
                        this.bluecoingates.Add(new Item((int)ItemTypes.BlueCoinGate, x, y));
                    }

                    // Build "portals" list
                    if (this.data[y, x].type == (int)ItemTypes.Portal)
                    {
                        this.portals.Add(new Item((int)ItemTypes.Portal, x, y));
                    }

                    // Build "Diamonds" list
                    if (this.data[y, x].type == (int)ItemTypes.Diamond)
                    {
                        this.diamonds.Add(new Item((int)ItemTypes.Diamond, x, y));
                    }

                    // Build "Cake" list
                    if (this.data[y, x].type == (int)ItemTypes.Cake)
                    {
                        this.cakes.Add(new Item((int)ItemTypes.Cake, x, y));
                    }

                    // Build "Holograms" list
                    if (this.data[y, x].type == (int)ItemTypes.Hologram)
                    {
                        this.holograms.Add(new Item((int)ItemTypes.Hologram, x, y));
                    }

                    // Build "Completes" list
                    if (this.data[y, x].type == (int)ItemTypes.Complete)
                    {
                        this.completes.Add(new Item((int)ItemTypes.Complete, x, y));
                    }

                    // Build "timedoors" list
                    if (this.data[y, x].type == (int)ItemTypes.TimeDoor ||
                        this.data[y, x].type == (int)ItemTypes.TimeGate)
                    {
                        this.timedoors.Add(new Item((int)this.data[y, x].type, x, y));
                    }

                    // Build "deathdoorsgates" list
                    if (this.data[y, x].type == (int)ItemTypes.DeathDoor ||
                        this.data[y, x].type == (int)ItemTypes.DeathGate)
                    {
                        this.deathdoorsgates.Add(new Item((int)this.data[y, x].type, x, y));
                    }

                    // Build "purple gates and doors" list
                    if (this.data[y, x].type == (int)ItemTypes.DoorPurple ||
                        this.data[y, x].type == (int)ItemTypes.GatePurple)
                    {
                        this.purpledoorgates.Add(new Item((int)this.data[y, x].type, x, y));
                    }

                    // Build "Spikes" list
                    if (this.data[y, x].type == (int)ItemTypes.Spike)
                    {
                        this.spikes.Add(new Item((int)this.data[y, x].type, x, y));
                    }

                    if (this.data[y, x].type == (int)ItemTypes.Fire)
                    {
                        this.fireHazards.Add(new Item((int)this.data[y, x].type, x, y));
                    }

                    // Build "zombiedoors" list
                    if (this.data[y, x].type == (int)ItemTypes.ZombieDoor ||
                        this.data[y, x].type == (int)ItemTypes.ZombieGate)
                    {
                        this.zombiedoors.Add(new Item((int)this.data[y, x].type, x, y));
                    }

                    // Build "world portals" list
                    if (this.data[y, x].type == (int)ItemTypes.WorldPortal)
                    {
                        this.worldportals.Add(new Item((int)this.data[y, x].type, x, y));
                    }

                    // Build "invisible portals" list
                    if (this.data[y, x].type == (int)ItemTypes.PortalInvisible)
                    {
                        this.invisibleportals.Add(new Item((int)ItemTypes.PortalInvisible, x, y));
                    }

                    // Build "text signs" list
                    if (this.data[y, x].type == (int)ItemTypes.TextSign)
                    {
                        this.textsigns.Add(new Item((int)ItemTypes.TextSign, x, y));
                    }
                }
            }

            this.isRefreshing = false;
            if (this.repeatRefresh)
            {
                this.repeatRefresh = false;
                this.refresh();
            }
        }

        public Item getSpawn()
        {
            if (this.spawns.Count > 0)
            {
                return this.spawns[++this.spawnOffset % this.spawns.Count];
            }
            return new Item(255, 1, 1);
        }

        public void reset()
        {
            this.coins = new List<Item>();
            this.spawns = new List<Item>();
            this.checkpoints = new List<Item>();
            this.coindoors = new List<Item>();
            this.coingates = new List<Item>();
            this.bluecoindoors = new List<Item>();
            this.bluecoingates = new List<Item>();
            this.timedoors = new List<Item>();
            this.purpledoorgates = new List<Item>();
            this.portals = new List<Item>();
            this.diamonds = new List<Item>();
            this.cakes = new List<Item>();
            this.holograms = new List<Item>();
            this.completes = new List<Item>();
            this.spikes = new List<Item>();
            this.fireHazards = new List<Item>();
            this.zombiedoors = new List<Item>();
            this.worldportals = new List<Item>();
            this.invisibleportals = new List<Item>();
            this.textsigns = new List<Item>();
            this.deathdoorsgates = new List<Item>();

            var fill_id = this.fillType;
            var border_id = this.borderType;

            //Reset entire field
            for (var y = 0; y < this.height; y++)
            {
                for (var x = 0; x < this.width; x++)
                {
                    this.data[y, x] = new Brick(fill_id);
                    this.dataBg[y, x] = new Brick(fill_id);
                }
            }

            //Set borders;
            for (var y = 0; y < this.height; y++)
            {
                this.data[y, 0].type = border_id;
                this.data[y, this.width - 1].type = border_id;
            }

            for (var x = 0; x < this.width; x++)
            {
                this.data[0, x].type = border_id;
                this.data[this.height - 1, x].type = border_id;
            }
        }

        public void reload()
        {
            this.fromDatabaseObject(this.dbo);
        }

        public bool setBrick(int layerNum, uint x, uint y, uint type)
        {
            return this.setBrick(layerNum, x, y, type, false);
        }

        public bool setBrick(int layerNum, uint x, uint y, uint type, bool skiprefresh)
        {
            var layerData = (layerNum == 0 ? this.data : this.dataBg);

            if (layerData[y, x] != null && layerData[y, x].type != type)
            {
                var past = layerData[y, x].type;

                // Do we need a recount of the restricted shop items (portal, coindoor, etc.)?
                if (
                    // Todo: måske bare check klassen?
                    past == (int)ItemTypes.SpawnPoint ||
                    past == (int)ItemTypes.Coin ||
                    past == (int)ItemTypes.CoinDoor ||
                    past == (int)ItemTypes.CoinGate ||
                    past == (int)ItemTypes.BlueCoinDoor ||
                    past == (int)ItemTypes.BlueCoinGate ||
                    past == (int)ItemTypes.Portal ||
                    past == (int)ItemTypes.Diamond ||
                    past == (int)ItemTypes.Label ||
                    past == (int)ItemTypes.Complete ||
                    past == (int)ItemTypes.TimeDoor ||
                    past == (int)ItemTypes.TimeGate ||
                    past == (int)ItemTypes.Cake ||
                    past == (int)ItemTypes.SwitchPurple ||
                    past == (int)ItemTypes.DoorPurple ||
                    past == (int)ItemTypes.GatePurple ||
                    past == (int)ItemTypes.Checkpoint ||
                    past == (int)ItemTypes.Spike ||
                    past == (int)ItemTypes.Fire ||
                    past == (int)ItemTypes.ZombieDoor ||
                    past == (int)ItemTypes.ZombieGate ||
                    past == (int)ItemTypes.WorldPortal ||
                    past == (int)ItemTypes.PortalInvisible ||
                    past == (int)ItemTypes.TextSign ||
                    past == (int)ItemTypes.Hologram ||
                    past == (int)ItemTypes.DeathDoor ||
                    past == (int)ItemTypes.DeathGate ||
                    type == (int)ItemTypes.SpawnPoint ||
                    type == (int)ItemTypes.Coin ||
                    type == (int)ItemTypes.CoinDoor ||
                    type == (int)ItemTypes.CoinGate ||
                    type == (int)ItemTypes.BlueCoinDoor ||
                    type == (int)ItemTypes.BlueCoinGate ||
                    type == (int)ItemTypes.Portal ||
                    type == (int)ItemTypes.Diamond ||
                    type == (int)ItemTypes.Label ||
                    type == (int)ItemTypes.Complete ||
                    type == (int)ItemTypes.TimeDoor ||
                    type == (int)ItemTypes.TimeGate ||
                    type == (int)ItemTypes.Cake ||
                    type == (int)ItemTypes.SwitchPurple ||
                    type == (int)ItemTypes.DoorPurple ||
                    type == (int)ItemTypes.GatePurple ||
                    type == (int)ItemTypes.Checkpoint ||
                    type == (int)ItemTypes.Spike ||
                    type == (int)ItemTypes.Fire ||
                    type == (int)ItemTypes.ZombieGate ||
                    type == (int)ItemTypes.ZombieDoor ||
                    type == (int)ItemTypes.WorldPortal ||
                    type == (int)ItemTypes.PortalInvisible ||
                    type == (int)ItemTypes.TextSign ||
                    type == (int)ItemTypes.Hologram ||
                    type == (int)ItemTypes.DeathDoor ||
                    type == (int)ItemTypes.DeathGate
                    )
                {
                    // Only NEW when it was another class before..
                    layerData[y, x] = new Brick(type);

                    //Added new property skiprefresh for the first time we initalize the world
                    if (!skiprefresh) this.refresh();
                }
                else
                {
                    // ..otherwise just change the type
                    layerData[y, x].type = type;
                }

                return true;
            }
            return false;
        }

        // ********************************************************************************************
        // Count functions are used to ensure that the payvault limit for a given item is not exceeded
        // ********************************************************************************************

        public uint getBrickType(int layerNum, int x, int y)
        {
            try
            {
                if (layerNum == 0)
                    return this.data[y, x].type;
                if (this.dataBg != null)
                    return this.dataBg[y, x].type;
                Console.Error.WriteLine("error in getBrickType, asking for a layer that doesn't exist");
                return 0;
            }
            catch (Exception e)
            {
                this.client.ErrorLog.WriteError("Unable to read value from world " + x + " " + y, e);
                return 0;
            }
        }

        public Brick getBrick(int layer, int x, int y)
        {
            if (layer == 0)
                return this.data[y, x];
            if (this.dataBg != null)
                return this.dataBg[y, x];
            Console.Error.WriteLine("error in getBrickType, asking for a layer that doesn't exist");
            return null;
        }

        private void unserializeWorldFromBytes(byte[] wd)
        {
            for (var y = 0; y < this.height; y++)
            {
                for (var x = 0; x < this.width; x++)
                {
                    this.data[y, x] = new Brick(wd[y * this.width + x]);
                }
            }
        }

        // Serializa world data, and add it to a message. 
        // Used for sending the entire world to a new player when connecting

        public void addToMessageAsComplexList(Message m)
        {
            m.Add("ws");

            var bricks = this.getBrickList(0);
            var bricksBg = this.getBrickList(1);

            bricks.AddRange(bricksBg);

            foreach (var b in bricks)
            {
                m.Add(b.type);
                m.Add(b.layer);

                var l = b.xs.Count;

                var xs = new byte[l * 2];
                var ys = new byte[l * 2];

                for (var a = 0; a < l; a++)
                {
                    xs[a * 2] = (byte)((b.xs[a] & 0x0000ff00) >> 8);
                    xs[a * 2 + 1] = (byte)(b.xs[a] & 0x000000ff);

                    ys[a * 2] = (byte)((b.ys[a] & 0x0000ff00) >> 8);
                    ys[a * 2 + 1] = (byte)(b.ys[a] & 0x000000ff);

                    //ys[a] = b.ys[a];
                }

                m.Add(xs);
                m.Add(ys);

                switch (b.type)
                {
                    //Coin doors and other stuffs
                    case (uint)ItemTypes.BlueCoinDoor:
                    case (uint)ItemTypes.BlueCoinGate:
                    case (uint)ItemTypes.CoinGate:
                    case (uint)ItemTypes.CoinDoor:
                    case (uint)ItemTypes.DeathDoor:
                    case (uint)ItemTypes.DeathGate:
                    case (uint)ItemTypes.DoorPurple:
                    case (uint)ItemTypes.GatePurple:
                    case (uint)ItemTypes.SwitchPurple:
                    {
                        m.Add(b.goal);
                        break;
                    }

                    // Portals
                    case (int)ItemTypes.PortalInvisible:
                    case (uint)ItemTypes.Portal:
                    {
                        m.Add(b.rotation);
                        m.Add(b.id);
                        m.Add(b.target);
                        break;
                    }
                    // World Portals
                    case (uint)ItemTypes.WorldPortal:
                    {
                        m.Add(b.targetworld);
                        break;
                    }

                    case (uint)ItemTypes.TextSign:
                    {
                        m.Add(b.text);
                        break;
                    }

                    case (uint)ItemTypes.GlowyLineBlueStraight:
                    case (uint)ItemTypes.GlowyLineBlueSlope:
                    case (int)ItemTypes.GlowyLineGreenSlope:
                    case (int)ItemTypes.GlowyLineGreenStraight:
                    case (int)ItemTypes.GlowyLineYellowSlope:
                    case (int)ItemTypes.GlowyLineYellowStraight:
                    case (uint)ItemTypes.OnewayCyan:
                    case (uint)ItemTypes.OnewayRed:
                    case (uint)ItemTypes.OnewayYellow:
                    case (uint)ItemTypes.OnewayPink:
                    case (uint)ItemTypes.Spike:
                    {
                        m.Add(b.rotation);
                        break;
                    }

                    // Labels
                    case (uint)ItemTypes.Label:
                    {
                        m.Add(b.text);
                        m.Add(b.text_color);
                        break;
                    }

                    //Piano
                    case (uint)ItemTypes.Piano:
                    {
                        m.Add(b.id);
                        break;
                    }

                    //Piano
                    case (uint)ItemTypes.Drums:
                    {
                        m.Add(b.id);
                        break;
                    }
                }
            }
            m.Add("we");
        }

        /*
        private void loadWorldPresentation()
        {
            worldPresentation.load(this.key, delegate() {
                Console.WriteLine("World presentation loaded/created");
            });
        }
        */

        public List<Item> getPortals()
        {
            return this.portals;
        }

        public List<Item> getInvisiblePortals()
        {
            return this.invisibleportals;
        }

        #region setBrick

        public bool setBrickCoindoor(uint x, uint y, uint goal, Boolean doRefresh)
        {
            // Check if any changes is necesary?

            if (this.data[y, x].type == (uint)ItemTypes.CoinDoor)
                if (((BrickCoindoor)this.data[y, x]).goal == goal)
                    return false;

            // Bounds check

            if (goal < 0 || goal > 99)
                return false;

            // Set brick

            this.data[y, x] = new BrickCoindoor(goal);


            if (doRefresh) this.refresh();

            return true;
        }

        public bool setBrickBlueCoindoor(uint x, uint y, uint goal, Boolean doRefresh)
        {
            // Check if any changes is necesary?

            if (this.data[y, x].type == (uint)ItemTypes.BlueCoinDoor)
            {
                if (((BrickBlueCoinDoor)this.data[y, x]).goal == goal)
                {
                    return false;
                }
            }

            // Bounds check

            if (goal < 0 || goal > 99)
                return false;

            // Set brick

            this.data[y, x] = new BrickBlueCoinDoor(goal);


            if (doRefresh) this.refresh();

            return true;
        }

        public bool setBrickCoingate(uint x, uint y, uint goal, Boolean doRefresh)
        {
            // Check if any changes is necesary?

            if (this.data[y, x].type == (uint)ItemTypes.CoinGate)
                if (((BrickCoingate)this.data[y, x]).goal == goal)
                    return false;

            // Bounds check

            if (goal < 0 || goal > 99)
                return false;

            // Set brick

            this.data[y, x] = new BrickCoingate(goal);


            if (doRefresh) this.refresh();

            return true;
        }

        public bool setBrickBlueCoingate(uint x, uint y, uint goal, Boolean doRefresh)
        {
            // Check if any changes is necesary?

            if (this.data[y, x].type == (uint)ItemTypes.BlueCoinGate)
                if (((BrickBlueCoingate)this.data[y, x]).goal == goal)
                    return false;

            // Bounds check

            if (goal < 0 || goal > 99)
                return false;

            // Set brick

            this.data[y, x] = new BrickBlueCoingate(goal);


            if (doRefresh) this.refresh();

            return true;
        }

        public bool setBrickDeathDoor(uint x, uint y, uint goal, Boolean doRefresh)
        {
            // Check if any changes is necesary?

            if (this.data[y, x].type == (uint)ItemTypes.DeathDoor)
            {
                if (((BrickDeathDoor)this.data[y, x]).goal == goal)
                {
                    return false;
                }
            }

            // Bounds check

            if (goal < 1 || goal > 99)
                return false;

            // Set brick

            this.data[y, x] = new BrickDeathDoor(goal);


            if (doRefresh) this.refresh();

            return true;
        }

        public bool setBrickDeathGate(uint x, uint y, uint goal, Boolean doRefresh)
        {
            // Check if any changes is necesary?

            if (this.data[y, x].type == (uint)ItemTypes.DeathGate)
            {
                if (((BrickDeathGate)this.data[y, x]).goal == goal)
                {
                    return false;
                }
            }

            // Bounds check

            if (goal < 1 || goal > 99)
                return false;

            // Set brick

            this.data[y, x] = new BrickDeathGate(goal);


            if (doRefresh) this.refresh();

            return true;
        }

        public bool setBrickDoorPurple(uint x, uint y, uint goal, Boolean doRefresh)
        {
            // Check if any changes is necesary?

            if (this.data[y, x].type == (uint)ItemTypes.DoorPurple)
            {
                if (((BrickDoorPurple)this.data[y, x]).goal == goal)
                {
                    return false;
                }
            }

            // Bounds check

            if (goal < 0 || goal > 99)
                return false;

            // Set brick

            this.data[y, x] = new BrickDoorPurple(goal);


            if (doRefresh) this.refresh();

            return true;
        }

        public bool setBrickGatePurple(uint x, uint y, uint goal, Boolean doRefresh)
        {
            // Check if any changes is necesary?

            if (this.data[y, x].type == (uint)ItemTypes.GatePurple)
            {
                if (((BrickGatePurple)this.data[y, x]).goal == goal)
                {
                    return false;
                }
            }

            // Bounds check

            if (goal < 0 || goal > 99)
                return false;

            // Set brick

            this.data[y, x] = new BrickGatePurple(goal);


            if (doRefresh) this.refresh();

            return true;
        }

        public bool setBrickSwitchPurple(uint x, uint y, uint goal, Boolean doRefresh)
        {
            // Check if any changes is necesary?

            if (this.data[y, x].type == (uint)ItemTypes.SwitchPurple)
            {
                if (((BrickSwitchPurple)this.data[y, x]).goal == goal)
                {
                    return false;
                }
            }

            // Bounds check

            if (goal < 0 || goal > 99)
                return false;

            // Set brick

            this.data[y, x] = new BrickSwitchPurple(goal);


            if (doRefresh) this.refresh();

            return true;
        }

        //
        //
        public bool setBrickRotateable(int layernum, uint x, uint y, uint type, uint rotation, Boolean doRefresh)
        {
            //Console.WriteLine("World.cs > setBrickRotateable, layernum " + layernum + ", rotation " + rotation + ", type " + type);
            var layerData = (layernum == 0 ? this.data : this.dataBg);

            var changed = false;

            if (layerData[y, x].type != type)
            {
                layerData[y, x] = new BrickRotateable(type, rotation);
                if (doRefresh) this.refresh();

                return true;
            }

            // ...Otherwise just check and change the attributes

            var rotateable = (BrickRotateable)layerData[y, x];

            if (rotateable.rotation != rotation && rotation <= 4)
            {
                rotateable.rotation = rotation;
                changed = true;
            }

            return changed;
        }


        public bool setBrickPortal(uint brickType, uint x, uint y, uint rotation, uint id, uint target, bool doRefresh)
        {
            var changed = false;
            // If tile is not already a portal, create a new...

            if (this.data[y, x].type != (uint)ItemTypes.Portal || this.type != (uint)ItemTypes.PortalInvisible)
            {
                this.data[y, x] = new BrickPortal(brickType, rotation, id, target);
                //We added a portal, do refresh
                if (doRefresh) this.refresh();

                return true;
            }

            // ...Otherwise just check and change the attributes

            var portal = (BrickPortal)this.data[y, x];

            if (portal.rotation != rotation && rotation <= 4)
            {
                portal.rotation = rotation;
                changed = true;
            }

            if (portal.id != id)
            {
                portal.id = id;
                changed = true;
            }

            if (portal.target != target)
            {
                portal.target = target;
                changed = true;
            }

            return changed;
        }

        public bool setBrickWorldPortal(uint x, uint y, string target, bool doRefresh)
        {
            var changed = false;

            // If tile is not already a portal, create a new...

            if (this.data[y, x].type != (uint)ItemTypes.WorldPortal)
            {
                this.data[y, x] = new BrickWorldPortal(target);

                //We added a portal, do refresh
                if (doRefresh) this.refresh();

                return true;
            }

            // ...Otherwise just check and change the attributes

            var portal = (BrickWorldPortal)this.data[y, x];

            if (portal.targetworld != target)
            {
                portal.targetworld = target;
                changed = true;
            }

            return changed;
        }

        public bool setBrickLabel(uint x, uint y, string text, string text_color)
        {
            if (this.data[y, x].type == 1000)
                if (text.Equals(((BrickLabel)this.data[y, x]).text) &&
                    text_color.Equals(((BrickLabel)this.data[y, x]).text_color))
                    return false;

            this.data[y, x] = new BrickLabel(text, text_color);

            return true;
        }

        public bool setBrickTextSign(uint x, uint y, string text, bool doRefresh)
        {
            if (this.data[y, x].type == (uint)ItemTypes.TextSign)
                if (text.Equals(((BrickTextSign)this.data[y, x]).text))
                    return false;

            this.data[y, x] = new BrickTextSign(text);

            //We added a textsign, do refresh
            if (doRefresh) this.refresh();

            return true;
        }

        public bool setBrickSound(ItemTypes type, uint x, uint y, uint sound)
        {
            if (type == ItemTypes.Piano)
            {
                //Do nothing if current value is already a piano with same value
                if (this.data[y, x].type == (int)ItemTypes.Piano && sound.Equals(((BrickPiano)this.data[y, x]).sound))
                {
                    return false;
                }

                if (sound > 26) return false;

                this.data[y, x] = new BrickPiano(sound);
                return true;
            }

            if (type == ItemTypes.Drums)
            {
                //Do nothing if current value is already a drum with same value
                if (this.data[y, x].type == (int)ItemTypes.Drums && sound.Equals(((BrickDrums)this.data[y, x]).sound))
                {
                    return false;
                }

                if (sound > 9) return false;

                this.data[y, x] = new BrickDrums(sound);
                return true;
            }


            return true;
        }

        public bool setBrickComplete(uint x, uint y)
        {
            //if (data[y, x].type == (int)ItemTypes.Exit)

            this.data[y, x] = new BrickComplete();
            this.refresh();

            return true;
        }

        #endregion

        #region List count getters

        public int coinCount()
        {
            return this.coins.Count;
        }

        public double coinPercentage()
        {
            return this.coins.Count / ((this.width - 2) * (this.height - 2));
        }

        public int spawnCount()
        {
            return this.spawns.Count;
        }

        public int checkpointCount()
        {
            return this.checkpoints.Count;
        }

        public int deathDoorsGatesCount()
        {
            return this.deathdoorsgates.Count;
        }

        public int coindoorCount()
        {
            return this.coindoors.Count;
        }

        public int coingateCount()
        {
            return this.coingates.Count;
        }

        public int bluecoindoorCount()
        {
            return this.bluecoindoors.Count;
        }

        public int bluecoingateCount()
        {
            return this.bluecoingates.Count;
        }

        public int timedoorCount()
        {
            return this.timedoors.Count;
        }


        public int zombiedoorCount()
        {
            return this.zombiedoors.Count;
        }

        public int purpleDoorGateCount()
        {
            return this.purpledoorgates.Count;
        }

        public int portalCount()
        {
            return this.portals.Count;
        }

        public int diamondCount()
        {
            return this.diamonds.Count;
        }

        public int cakesCount()
        {
            return this.cakes.Count;
        }

        public int hologramsCount()
        {
            return this.holograms.Count;
        }

        public int completeCount()
        {
            return this.completes.Count;
        }

        public int spikesCount()
        {
            return this.spikes.Count;
        }

        public int fireHazardCount()
        {
            return this.fireHazards.Count;
        }

        public int worldportalsCount()
        {
            return this.worldportals.Count;
        }

        public int invisibleportalsCount()
        {
            return this.invisibleportals.Count;
        }

        public int textsignsCount()
        {
            return this.textsigns.Count;
        }

        #endregion
    }

    public class Brick
    {
        public uint type;

        public Brick(uint type)
        {
            this.type = type;
        }
    }

    public class BrickRotateable : Brick
    {
        public uint rotation = 0;

        public BrickRotateable(uint type, uint rotation)
            : base(type)
        {
            this.rotation = Math.Max(0, Math.Min(rotation, 3));

            // Reintroduced but to add invisible teleports
            //if (rotation == 42) this.rotation = rotation;
        }
    }

    public class BrickPortal : BrickRotateable
    {
        public uint id = 0;
        public uint target = 0;

        public BrickPortal(uint brickType, uint rotation, uint id, uint target)
            : base( /*(uint)ItemTypes.Portal*/brickType, rotation)
        {
            this.id = id;
            this.target = target;
            // remove this line to remove "invisible portal" bug
            this.rotation = rotation;
        }
    }

    public class BrickWorldPortal : Brick
    {
        public string targetworld = null;

        public BrickWorldPortal(string target)
            : base((uint)ItemTypes.WorldPortal)
        {
            this.targetworld = target;
        }
    }

    public class BrickCoindoor : Brick
    {
        public uint goal = 5;

        public BrickCoindoor(uint goal)
            : base((uint)ItemTypes.CoinDoor)
        {
            this.goal = goal;
        }
    }

    public class BrickBlueCoinDoor : Brick
    {
        public uint goal = 5;

        public BrickBlueCoinDoor(uint goal)
            : base((uint)ItemTypes.BlueCoinDoor)
        {
            this.goal = goal;
        }
    }


    public class BrickCoingate : Brick
    {
        public uint goal = 5;

        public BrickCoingate(uint goal)
            : base((uint)ItemTypes.CoinGate)
        {
            this.goal = goal;
        }
    }

    public class BrickBlueCoingate : Brick
    {
        public uint goal = 5;

        public BrickBlueCoingate(uint goal)
            : base((uint)ItemTypes.BlueCoinGate)
        {
            this.goal = goal;
        }
    }

    public class BrickDeathDoor : Brick
    {
        public uint goal = 5;

        public BrickDeathDoor(uint goal)
            : base((uint)ItemTypes.DeathDoor)
        {
            this.goal = goal;
        }
    }

    public class BrickDeathGate : Brick
    {
        public uint goal = 5;

        public BrickDeathGate(uint goal)
            : base((uint)ItemTypes.DeathGate)
        {
            this.goal = goal;
        }
    }

    public class BrickDoorPurple : Brick
    {
        public uint goal = 5;

        public BrickDoorPurple(uint goal)
            : base((uint)ItemTypes.DoorPurple)
        {
            this.goal = goal;
        }
    }

    public class BrickGatePurple : Brick
    {
        public uint goal = 5;

        public BrickGatePurple(uint goal)
            : base((uint)ItemTypes.GatePurple)
        {
            this.goal = goal;
        }
    }

    public class BrickSwitchPurple : Brick
    {
        public uint goal = 5;

        public BrickSwitchPurple(uint goal)
            : base((uint)ItemTypes.SwitchPurple)
        {
            this.goal = goal;
        }
    }

    internal class BrickLabel : Brick
    {
        public string text = null;
        public string text_color = null;

        public BrickLabel(string txt, string txt_col)
            : base((uint)ItemTypes.Label)
        {
            this.text = txt;
            this.text_color = txt_col;
        }
    }

    internal class BrickTextSign : Brick
    {
        public string text = null;

        public BrickTextSign(string txt)
            : base((uint)ItemTypes.TextSign)
        {
            this.text = txt;
        }
    }

    internal class BrickPiano : Brick
    {
        public uint sound = 0;

        public BrickPiano(uint offset)
            : base((uint)ItemTypes.Piano)
        {
            if (offset > 26) offset = 26;
            this.sound = offset;
        }
    }

    internal class BrickDrums : Brick
    {
        public uint sound = 0;

        public BrickDrums(uint offset)
            : base((uint)ItemTypes.Drums)
        {
            if (offset > 9) offset = 9;
            this.sound = offset;
        }
    }

    public class BrickComplete : Brick
    {
        public BrickComplete()
            : base((uint)ItemTypes.Complete)
        {
        }
    }


    public class DBBrick
    {
        public uint goal;

        public uint id = 0;
        public int layer;
        public uint rotation = 0;
        public uint target = 0;
        public string targetworld = null;

        public string text = null;
        public string text_color = null;
        public uint type;

        public List<uint> xs = new List<uint>();
        public List<uint> ys = new List<uint>();

        public DBBrick(Brick b)
        {
            this.type = b.type;

            if (this.type == (uint)ItemTypes.CoinDoor)
            {
                this.goal = ((BrickCoindoor)b).goal;
            }
            else if (this.type == (uint)ItemTypes.CoinGate)
            {
                this.goal = ((BrickCoingate)b).goal;
            }
            else if (this.type == (uint)ItemTypes.BlueCoinDoor)
            {
                this.goal = ((BrickBlueCoinDoor)b).goal;
            }
            else if (this.type == (uint)ItemTypes.BlueCoinGate)
            {
                this.goal = ((BrickBlueCoingate)b).goal;
            }
            else if (this.type == (uint)ItemTypes.DeathDoor)
            {
                this.goal = ((BrickDeathDoor)b).goal;
            }
            else if (this.type == (uint)ItemTypes.DeathGate)
            {
                this.goal = ((BrickDeathGate)b).goal;
            }
            else if (this.type == (uint)ItemTypes.SwitchPurple)
            {
                this.goal = ((BrickSwitchPurple)b).goal;
            }
            else if (this.type == (uint)ItemTypes.DoorPurple)
            {
                this.goal = ((BrickDoorPurple)b).goal;
            }
            else if (this.type == (uint)ItemTypes.GatePurple)
            {
                this.goal = ((BrickGatePurple)b).goal;
            }
            else if (this.type == (uint)ItemTypes.Portal)
            {
                this.id = ((BrickPortal)b).id;
                this.target = ((BrickPortal)b).target;
                this.rotation = ((BrickPortal)b).rotation;
            }
            else if (this.type == (uint)ItemTypes.WorldPortal)
            {
                this.targetworld = ((BrickWorldPortal)b).targetworld;
            }
            else if (this.type == (uint)ItemTypes.Label)
            {
                this.text = ((BrickLabel)b).text;
                this.text_color = ((BrickLabel)b).text_color;
            }
            else if (this.type == (uint)ItemTypes.Piano)
            {
                this.id = ((BrickPiano)b).sound;
            }
            else if (this.type == (uint)ItemTypes.Drums)
            {
                this.id = ((BrickDrums)b).sound;

                // TODO: This should be refactored to some generic isRotateable
            }
            else if (this.type == (uint)ItemTypes.Spike)
            {
                this.rotation = ((BrickRotateable)b).rotation;
            }
            else if (this.type == (uint)ItemTypes.GlowyLineBlueSlope ||
                     this.type == (uint)ItemTypes.GlowyLineBlueStraight ||
                     this.type == (uint)ItemTypes.GlowyLineGreenSlope ||
                     this.type == (uint)ItemTypes.GlowyLineGreenStraight ||
                     this.type == (uint)ItemTypes.GlowyLineYellowSlope ||
                     this.type == (uint)ItemTypes.GlowyLineYellowStraight ||
                     this.type == (uint)ItemTypes.OnewayCyan || this.type == (uint)ItemTypes.OnewayRed ||
                     this.type == (uint)ItemTypes.OnewayYellow ||
                     this.type == (uint)ItemTypes.OnewayPink)
            {
                this.rotation = ((BrickRotateable)b).rotation;
            }
            else if (this.type == (uint)ItemTypes.PortalInvisible)
            {
                this.id = ((BrickPortal)b).id;
                this.target = ((BrickPortal)b).target;
                this.rotation = ((BrickPortal)b).rotation;
            }
            else if (this.type == (uint)ItemTypes.TextSign)
            {
                this.text = ((BrickTextSign)b).text;
            }
        }

        public bool equalBrickValues(Brick b)
        {
            if (b.type != this.type)
                return false;

            if (b.type == (uint)ItemTypes.CoinDoor)
            {
                var door = (BrickCoindoor)b;
                return (this.goal == door.goal);
            }

            if (b.type == (uint)ItemTypes.CoinGate)
            {
                var door = (BrickCoingate)b;
                return (this.goal == door.goal);
            }

            if (b.type == (uint)ItemTypes.BlueCoinDoor)
            {
                var door = (BrickBlueCoinDoor)b;
                return (this.goal == door.goal);
            }

            if (b.type == (uint)ItemTypes.BlueCoinGate)
            {
                var door = (BrickBlueCoingate)b;
                return (this.goal == door.goal);
            }

            if (b.type == (uint)ItemTypes.DeathDoor)
            {
                var door = (BrickDeathDoor)b;
                return (this.goal == door.goal);
            }

            if (b.type == (uint)ItemTypes.DeathGate)
            {
                var gate = (BrickDeathGate)b;
                return (this.goal == gate.goal);
            }

            if (b.type == (uint)ItemTypes.Portal)
            {
                var portal = (BrickPortal)b;
                return ((portal.id == this.id) && (portal.rotation == this.rotation) && (portal.target == this.target));
            }

            if (b.type == (uint)ItemTypes.PortalInvisible)
            {
                var portal = (BrickPortal)b;
                return ((portal.id == this.id) && (portal.rotation == this.rotation) && (portal.target == this.target));
            }


            // Purple Switches
            if (b.type == (uint)ItemTypes.SwitchPurple)
            {
                var switchy = (BrickSwitchPurple)b;
                return (switchy.goal == this.goal);
            }

            if (b.type == (uint)ItemTypes.DoorPurple)
            {
                var switchy = (BrickDoorPurple)b;
                return (switchy.goal == this.goal);
            }
            if (b.type == (uint)ItemTypes.GatePurple)
            {
                var switchy = (BrickGatePurple)b;
                return (switchy.goal == this.goal);
            }


            if (b.type == (uint)ItemTypes.WorldPortal)
            {
                var portal = (BrickWorldPortal)b;
                return portal.targetworld.Equals(this.targetworld);
            }

            if (b.type == (uint)ItemTypes.Spike)
            {
                var rot = (BrickRotateable)b;
                return (rot.rotation == this.rotation);
            }

            if (b.type == (uint)ItemTypes.GlowyLineBlueSlope || b.type == (uint)ItemTypes.GlowyLineBlueStraight ||
                b.type == (uint)ItemTypes.GlowyLineGreenSlope || b.type == (uint)ItemTypes.GlowyLineGreenStraight ||
                b.type == (uint)ItemTypes.GlowyLineYellowSlope || b.type == (uint)ItemTypes.GlowyLineYellowStraight ||
                this.type == (uint)ItemTypes.OnewayCyan || this.type == (uint)ItemTypes.OnewayRed ||
                this.type == (uint)ItemTypes.OnewayYellow ||
                this.type == (uint)ItemTypes.OnewayPink)
            {
                var rot = (BrickRotateable)b;
                return (rot.rotation == this.rotation);
            }

            if (b.type == (uint)ItemTypes.Label)
            {
                var l = (BrickLabel)b;
                return l.text.Equals(this.text) && l.text.Equals(this.text_color);
            }

            if (b.type == (uint)ItemTypes.Piano)
            {
                var l = (BrickPiano)b;
                return l.sound.Equals(this.id);
            }

            if (b.type == (uint)ItemTypes.Drums)
            {
                var l = (BrickDrums)b;
                return l.sound.Equals(this.id);
            }

            if (b.type == (uint)ItemTypes.TextSign)
            {
                return ((BrickTextSign)b).text.Equals(this.text);
            }

            return true;
        }
    }
}