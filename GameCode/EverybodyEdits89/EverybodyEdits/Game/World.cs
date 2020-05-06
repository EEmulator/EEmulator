using System;
using System.Collections.Generic;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game
{
    public class World
    {
        public World()
        {
            this.width = 200;
            this.height = 200;
            this.data = new Brick[this.height, this.width];
            this.dataBg = new Brick[this.height, this.width];
            this.reset();
        }

        public void fromDatabaseObject(DatabaseObject dbo)
        {
            this.dbo = dbo;
            this.width = dbo.GetInt("width", 200);
            this.height = dbo.GetInt("height", 200);
            this.data = new Brick[this.height, this.width];
            this.dataBg = new Brick[this.height, this.width];
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
        }

        public void unserializeFromComplexObject(DatabaseArray worlddata)
        {
            for (var a = 0; a < worlddata.Count; a++)
            {
                var ct = worlddata.GetObject(a);
                var type = (uint)ct.GetValue("type");
                var layerNum = ct.GetInt("layer", 0);
                var xs = ct.GetBytes("x", new byte[0]);
                var ys = ct.GetBytes("y", new byte[0]);
                var b = 0;
                while (b < xs.Length)
                {
                    var nx = (uint)(((int)xs[b] << 8) + (int)xs[b + 1]);
                    var ny = (uint)(((int)ys[b] << 8) + (int)ys[b + 1]);
                    var num = type;
                    if (num <= 77U)
                    {
                        if (num != 43U)
                        {
                            if (num != 77U)
                            {
                                goto IL_168;
                            }
                            this.setBrickSound(ItemTypes.Piano, nx, ny, ct.GetUInt("id", 0U));
                        }
                        else
                        {
                            this.setBrickCoindoor(nx, ny, (uint)ct.GetValue("goal"), false);
                        }
                    }
                    else if (num != 83U)
                    {
                        if (num != 242U)
                        {
                            if (num != 1000U)
                            {
                                goto IL_168;
                            }
                            this.setBrickLabel(nx, ny, ct.GetString("text", "no text found"));
                        }
                        else
                        {
                            this.setBrickPortal(nx, ny, ct.GetUInt("rotation", 0U), ct.GetUInt("id", 0U), ct.GetUInt("target", 0U), false);
                        }
                    }
                    else
                    {
                        this.setBrickSound(ItemTypes.Drums, nx, ny, ct.GetUInt("id", 0U));
                    }
                    IL_179:
                    b += 2;
                    continue;
                    IL_168:
                    this.setBrick(layerNum, nx, ny, type, true);
                    goto IL_179;
                }
            }
        }

        public void save(Callback callback)
        {
            if (this.dbo != null)
            {
                var bricks = this.getBrickList(0);
                bricks.AddRange(this.getBrickList(1));
                var worlddata = new DatabaseArray();
                foreach (var b in bricks)
                {
                    var cb = new DatabaseObject();
                    var i = b.xs.Count;
                    var xs = new byte[i * 2];
                    var ys = new byte[i * 2];
                    for (var a = 0; a < i; a++)
                    {
                        xs[a * 2] = (byte)((b.xs[a] & 65280U) >> 8);
                        xs[a * 2 + 1] = (byte)(b.xs[a] & 255U);
                        ys[a * 2] = (byte)((b.ys[a] & 65280U) >> 8);
                        ys[a * 2 + 1] = (byte)(b.ys[a] & 255U);
                    }
                    cb.Set("type", b.type);
                    cb.Set("layer", b.layer);
                    cb.Set("x", xs);
                    cb.Set("y", ys);
                    var type = b.type;
                    if (type <= 77U)
                    {
                        if (type != 43U)
                        {
                            if (type == 77U)
                            {
                                cb.Set("id", b.id);
                            }
                        }
                        else
                        {
                            cb.Set("goal", b.goal);
                        }
                    }
                    else if (type != 83U)
                    {
                        if (type != 242U)
                        {
                            if (type == 1000U)
                            {
                                cb.Set("text", b.text);
                            }
                        }
                        else
                        {
                            cb.Set("rotation", b.rotation);
                            cb.Set("id", b.id);
                            cb.Set("target", b.target);
                        }
                    }
                    else
                    {
                        cb.Set("id", b.id);
                    }
                    worlddata.Add(cb);
                }
                this.dbo.Set("worlddata", worlddata);
                if (this.dbo.Contains("world"))
                {
                    this.dbo.Remove("world");
                }
                this.dbo.Save(callback);
            }
        }

        private List<DBBrick> getBrickList(int layer)
        {
            var dbbricks = new List<DBBrick>();
            var layerData = (layer == 0) ? this.data : this.dataBg;
            var y = 0U;
            while ((ulong)y < (ulong)((long)this.height))
            {
                var x = 0U;
                while ((ulong)x < (ulong)((long)this.width))
                {
                    var cur = layerData[(int)((uint)y), (int)((uint)x)];
                    if (cur.type != 0U)
                    {
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
                    x += 1U;
                }
                y += 1U;
            }
            return dbbricks;
        }

        private void refresh()
        {
            Console.WriteLine("refresh called");
            this.spawns = new List<Item>();
            this.coindoors = new List<Item>();
            this.portals = new List<Item>();
            this.diamonds = new List<Item>();
            for (var y = 0; y < this.height; y++)
            {
                for (var x = 0; x < this.width; x++)
                {
                    if (this.data[y, x].type == 255U)
                    {
                        this.spawns.Add(new Item(255, x, y));
                    }
                    if (this.data[y, x].type == 43U)
                    {
                        this.coindoors.Add(new Item(43, x, y));
                    }
                    if (this.data[y, x].type == 242U)
                    {
                        this.portals.Add(new Item(242, x, y));
                    }
                    if (this.data[y, x].type == 241U)
                    {
                        this.diamonds.Add(new Item(141, x, y));
                    }
                }
            }
        }

        public Item getSpawn()
        {
            Item result;
            if (this.spawns.Count > 0)
            {
                result = this.spawns[++this.spawnOffset % this.spawns.Count];
            }
            else
            {
                result = new Item(255, 1, 1);
            }
            return result;
        }

        public void reset()
        {
            this.spawns = new List<Item>();
            this.coindoors = new List<Item>();
            this.portals = new List<Item>();
            this.diamonds = new List<Item>();
            for (var y = 0; y < this.height; y++)
            {
                for (var x = 0; x < this.width; x++)
                {
                    this.data[y, x] = new Brick(0U);
                    this.dataBg[y, x] = new Brick(0U);
                }
            }
            for (var y = 0; y < this.height; y++)
            {
                this.data[y, 0].type = 9U;
                this.data[y, this.width - 1].type = 9U;
            }
            for (var x = 0; x < this.width; x++)
            {
                this.data[0, x].type = 9U;
                this.data[this.height - 1, x].type = 9U;
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
            var layerData = (layerNum == 0) ? this.data : this.dataBg;
            var past = layerData[(int)((uint)y), (int)((uint)x)].type;
            bool result;
            if (past != type)
            {
                if (past == 255U || past == 43U || past == 242U || past == 241U || past == 1000U || type == 255U || type == 43U || type == 242U || type == 241U || type == 1000U)
                {
                    layerData[(int)((uint)y), (int)((uint)x)] = new Brick(type);
                    if (!skiprefresh)
                    {
                        this.refresh();
                    }
                }
                else
                {
                    layerData[(int)((uint)y), (int)((uint)x)].type = type;
                }
                result = true;
            }
            else
            {
                result = false;
            }
            return result;
        }

        public bool setBrickCoindoor(uint x, uint y, uint goal, bool doRefresh)
        {
            bool result;
            if (this.data[(int)((uint)y), (int)((uint)x)].type == 43U && ((BrickCoindoor)this.data[(int)((uint)y), (int)((uint)x)]).goal == goal)
            {
                result = false;
            }
            else if (goal < 0U || goal > 99U)
            {
                result = false;
            }
            else
            {
                this.data[(int)((uint)y), (int)((uint)x)] = new BrickCoindoor(goal);
                if (doRefresh)
                {
                    this.refresh();
                }
                result = true;
            }
            return result;
        }

        public bool setBrickPortal(uint x, uint y, uint rotation, uint id, uint target, bool doRefresh)
        {
            var changed = false;
            bool result;
            if (this.data[(int)((uint)y), (int)((uint)x)].type != 242U)
            {
                this.data[(int)((uint)y), (int)((uint)x)] = new BrickPortal(rotation, id, target);
                if (doRefresh)
                {
                    this.refresh();
                }
                result = true;
            }
            else
            {
                var portal = (BrickPortal)this.data[(int)((uint)y), (int)((uint)x)];
                if (portal.rotation != rotation && rotation <= 4U)
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
                result = changed;
            }
            return result;
        }

        public bool setBrickLabel(uint x, uint y, string text)
        {
            bool result;
            if (this.data[(int)((uint)y), (int)((uint)x)].type == 1000U && text.Equals(((BrickLabel)this.data[(int)((uint)y), (int)((uint)x)]).text))
            {
                result = false;
            }
            else
            {
                this.data[(int)((uint)y), (int)((uint)x)] = new BrickLabel(text);
                result = true;
            }
            return result;
        }

        public bool setBrickSound(ItemTypes type, uint x, uint y, uint sound)
        {
            bool result;
            if (type == ItemTypes.Piano)
            {
                if (this.data[(int)((uint)y), (int)((uint)x)].type == 77U && sound.Equals(((BrickPiano)this.data[(int)((uint)y), (int)((uint)x)]).sound))
                {
                    result = false;
                }
                else if (sound > 26U)
                {
                    result = false;
                }
                else
                {
                    this.data[(int)((uint)y), (int)((uint)x)] = new BrickPiano(sound);
                    result = true;
                }
            }
            else if (type == ItemTypes.Drums)
            {
                if (this.data[(int)((uint)y), (int)((uint)x)].type == 83U && sound.Equals(((BrickDrums)this.data[(int)((uint)y), (int)((uint)x)]).sound))
                {
                    result = false;
                }
                else if (sound > 9U)
                {
                    result = false;
                }
                else
                {
                    this.data[(int)((uint)y), (int)((uint)x)] = new BrickDrums(sound);
                    result = true;
                }
            }
            else
            {
                result = true;
            }
            return result;
        }

        public int spawnCount()
        {
            return this.spawns.Count;
        }

        public int coindoorCount()
        {
            return this.coindoors.Count;
        }

        public int portalCoint()
        {
            return this.portals.Count;
        }

        public int diamondCoint()
        {
            return this.diamonds.Count;
        }

        public uint getBrickType(int layerNum, int x, int y)
        {
            uint result;
            if (layerNum == 0)
            {
                result = this.data[y, x].type;
            }
            else if (this.dataBg != null)
            {
                result = this.dataBg[y, x].type;
            }
            else
            {
                Console.Error.WriteLine("error in getBrickType, asking for a layer that doesn't exist");
                result = 0U;
            }
            return result;
        }

        public Brick getBrick(int layer, int x, int y)
        {
            Brick result;
            if (layer == 0)
            {
                result = this.data[y, x];
            }
            else if (this.dataBg != null)
            {
                result = this.dataBg[y, x];
            }
            else
            {
                Console.Error.WriteLine("error in getBrickType, asking for a layer that doesn't exist");
                result = null;
            }
            return result;
        }

        private void unserializeWorldFromBytes(byte[] wd)
        {
            for (var y = 0; y < this.height; y++)
            {
                for (var x = 0; x < this.width; x++)
                {
                    this.data[y, x] = new Brick((uint)wd[y * this.width + x]);
                }
            }
        }

        public void addToMessageAsComplexList(Message m)
        {
            var bricks = this.getBrickList(0);
            var bricksBg = this.getBrickList(1);
            bricks.AddRange(bricksBg);
            foreach (var b in bricks)
            {
                m.Add(b.type);
                m.Add(b.layer);
                var i = b.xs.Count;
                var xs = new byte[i * 2];
                var ys = new byte[i * 2];
                for (var a = 0; a < i; a++)
                {
                    xs[a * 2] = (byte)((b.xs[a] & 65280U) >> 8);
                    xs[a * 2 + 1] = (byte)(b.xs[a] & 255U);
                    ys[a * 2] = (byte)((b.ys[a] & 65280U) >> 8);
                    ys[a * 2 + 1] = (byte)(b.ys[a] & 255U);
                }
                m.Add(xs);
                m.Add(ys);
                var type = b.type;
                if (type <= 77U)
                {
                    if (type != 43U)
                    {
                        if (type == 77U)
                        {
                            m.Add(b.id);
                        }
                    }
                    else
                    {
                        m.Add(b.goal);
                    }
                }
                else if (type != 83U)
                {
                    if (type != 242U)
                    {
                        if (type == 1000U)
                        {
                            m.Add(b.text);
                        }
                    }
                    else
                    {
                        m.Add(b.rotation);
                        m.Add(b.id);
                        m.Add(b.target);
                    }
                }
                else
                {
                    m.Add(b.id);
                }
            }
        }

        private Brick[,] data;

        private Brick[,] dataBg;

        private DatabaseObject dbo;

        public int width;

        public int height;

        private List<Item> spawns = new List<Item>();

        private List<Item> coindoors = new List<Item>();

        private List<Item> portals = new List<Item>();

        private List<Item> diamonds = new List<Item>();

        private int spawnOffset = 0;
    }
}
