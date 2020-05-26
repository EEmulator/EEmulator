using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using FastBitmapLibrary;
using Newtonsoft.Json.Linq;
using PlayerIOClient;
using SevenZipExtractor;

namespace EEWorldArchive
{
    public class WorldArchive
    {
        public ArchiveFile ArchiveFile { get; private set; }

        /// <summary>
        /// Instantiate the archive in memory to allow extracting the worlds within.
        /// </summary>
        /// <param name="path"> The path to the world archive file. </param>
        public WorldArchive(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!File.Exists(path))
                throw new FileNotFoundException("The world archive file could not be found at the specified location.", path);

            this.ArchiveFile = new ArchiveFile(path);
        }

        public IEnumerable<World> Retrieve(string connectUserId)
        {
            foreach (var entry in this.ArchiveFile.Entries)
            {
                if (entry.IsFolder)
                    continue;

                if (!entry.FileName.StartsWith($@"worlds\{connectUserId}"))
                    continue;

                var stream = new MemoryStream();
                entry.Extract(stream);
                stream.Position = 0;

                var world_id = entry.FileName.Substring($@"worlds\{connectUserId}".Length, entry.FileName.Length - $@"worlds\{connectUserId}".Length - ".tson".Length);

                yield return new World(world_id, new StreamReader(stream).ReadToEnd());
            }
        }

        public class World
        {
            public string WorldId { get; private set; }
            public string Tson { get; private set; }
            public DatabaseObject Object { get; private set; }
            public Bitmap Minimap { get; private set; }

            internal World(string world_id, string tson)
            {
                this.WorldId = world_id;
                this.Tson = tson;
                this.Object = DatabaseObject.LoadFromString(this.Tson);
                this.Minimap = this.Object.GenerateMinimap();
            }
        }
    }

    public static class MinimapUtility
    {
        public static Bitmap GenerateMinimap(this DatabaseObject world)
        {
            if (world == null)
                throw new ArgumentNullException(nameof(world));

            var colors = JObject.Parse(File.ReadAllText("includes//colors.json"));
            var width = world.GetInt("width", 200);
            var height = world.GetInt("height", 200);

            Color GetColor(int bid) => Color.FromArgb(
                colors[bid.ToString()].SelectToken("a").Value<int>(),
                colors[bid.ToString()].SelectToken("r").Value<int>(),
                colors[bid.ToString()].SelectToken("g").Value<int>(),
                colors[bid.ToString()].SelectToken("b").Value<int>());

            Color GetColorFromARGB(uint color) =>
                Color.FromArgb((byte)(color >> 24),
                    (byte)((color >> 16) & 0b11111111),
                    (byte)((color >> 8) & 0b11111111),
                    (byte)(color & 0b11111111));

            var bitmap = new Bitmap(width, height);

            using (var fb = bitmap.FastLock())
            {
                if (world.ContainsKey("backgroundColor"))
                    fb.Clear(GetColorFromARGB(world.GetUInt("backgroundColor")));
                else fb.Clear(Color.Black);
            }

            var world_data = world.GetArray("worlddata", null);

            // complex object
            if (world_data != null)
            {
                using var fb = bitmap.FastLock();
                var background = new List<(int x, int y, int type)>();
                var foreground = new List<(int x, int y, int type)>();

                for (var a = 0u; a < world_data.Count; a++)
                {
                    if (world_data.GetObject(a).Count == 0)
                        continue;

                    var ct = world_data.GetObject(a, null);
                    var type = (uint)ct["type"];
                    var layerNum = ct.GetInt("layer", 0);
                    var xs = ct.GetBytes("x", new byte[0]);
                    var ys = ct.GetBytes("y", new byte[0]);
                    var x1S = ct.GetBytes("x1", new byte[0]);
                    var y1S = ct.GetBytes("y1", new byte[0]);

                    if (type == 0)
                        continue;

                    for (var b = 0; b < x1S.Length; b++)
                    {
                        var nx = x1S[b];
                        var ny = y1S[b];

                        if (layerNum == 0)
                            foreground.Add((nx, ny, (int)type));
                        if (layerNum == 1)
                            background.Add((nx, ny, (int)type));
                    }

                    for (var b = 0; b < xs.Length; b += 2)
                    {
                        var nx = (uint)((xs[b] << 8) + xs[b + 1]);
                        var ny = (uint)((ys[b] << 8) + ys[b + 1]);

                        if (layerNum == 0)
                            foreground.Add(((int)nx, (int)ny, (int)type));
                        if (layerNum == 1)
                            background.Add(((int)nx, (int)ny, (int)type));
                    }
                }

                foreach (var (x, y, type) in background)
                {
                    var color = GetColor(type);

                    if (color.A != 255)
                        continue;

                    fb.SetPixel(x, y, color);
                }

                foreach (var (x, y, type) in foreground)
                {
                    var color = GetColor(type);

                    if (color.A != 255)
                        continue;

                    fb.SetPixel(x, y, color);
                }
            }
            else // world bytes (legacy)
            {
                using var fb = bitmap.FastLock();
                {
                    // draw border
                    var maxX = (uint)(width - 1);
                    var maxY = (uint)(height - 1);
                    var color = GetColor(world.GetInt("BorderType", 9));
                    for (uint y = 0; y <= maxY; y++)
                    {
                        fb.SetPixel(0, (int)y, color);
                        fb.SetPixel((int)maxX, (int)y, color);
                    }
                    for (uint x = 0; x <= maxX; x++)
                    {
                        fb.SetPixel((int)x, 0, color);
                        fb.SetPixel((int)x, (int)maxY, color);
                    }
                }

                var world_bytes = world.GetBytes("world", null);

                if (world_bytes != null)
                {
                    for (var y = 0U; y < height; y++)
                    {
                        for (var x = 0U; x < height; x++)
                        {
                            var color = GetColor(world_bytes[y * width + x]);

                            if (color.A != 255)
                                continue;

                            fb.SetPixel((int)x, (int)y, color);
                        }
                    }
                }
            }

            return bitmap;
        }
    }
}
