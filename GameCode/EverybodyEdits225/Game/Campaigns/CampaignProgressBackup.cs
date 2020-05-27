using System;
using System.Collections.Generic;
using System.Linq;
using EverybodyEdits.Common;
using EverybodyEdits.Game.Chat;
using EverybodyEdits.Game.CountWorld;
using PlayerIO.GameLibrary;

namespace EverybodyEdits.Game.Campaigns
{
    internal class CampaignProgressBackup
    {
        private const string ObjectName = "ProgressBackup";

        private readonly DatabaseObject campaignPlayer;
        private readonly Player player;

        public CampaignProgressBackup(DatabaseObject campPlayer, Player player)
        {
            this.campaignPlayer = campPlayer;
            this.player = player;
        }

        public void Save(string roomId, World world)
        {
            var dbo = this.campaignPlayer;

            var effects = new DatabaseArray();

            foreach (var effect in this.player.GetEffects())
            {
                // If it's timed effect we can assume it expired and simply reset player's position to checkpoint
                if (effect.CanExpire)
                {
                    this.player.Checkpoint =
                        world.GetBrickType(0, this.player.Checkpoint.X, this.player.Checkpoint.Y) ==
                        (uint)ItemTypes.Checkpoint
                            ? this.player.Checkpoint
                            : world.GetSpawn();

                    this.player.X = this.player.Checkpoint.X * 16;
                    this.player.Y = this.player.Checkpoint.Y * 16;
                    this.player.SpeedX = 0;
                    this.player.SpeedY = 0;
                    this.player.Deaths += 1;
                }
                else
                {
                    var effectObject = new DatabaseObject().Set("Id", (int)effect.Id);
                    if (effect.Id == EffectId.Multijump)
                    {
                        effectObject.Set("Arg", effect.Duration);
                    }
                    else if (effect.Id == EffectId.Gravity)
                    {
                        effectObject.Set("Arg", effect.Duration);
                    }
                    effects.Add(effectObject);
                }
            }

            var goldCoins = this.GetPositionsArray(this.player.Coinmap.Where(it => it.Block.Type == 100).ToList());
            var blueCoins = this.GetPositionsArray(this.player.Coinmap.Where(it => it.Block.Type == 101).ToList());

            var coinsObject = new DatabaseObject()
                .Set("Gold", new DatabaseObject()
                    .Set("Count", this.player.Coins)
                    .Set("PosX", goldCoins.Item1)
                    .Set("PosY", goldCoins.Item2))
                .Set("Blue", new DatabaseObject()
                    .Set("Count", this.player.BlueCoins)
                    .Set("PosX", blueCoins.Item1)
                    .Set("PosY", blueCoins.Item2));

            var backupObject = new DatabaseObject()
                .Set("RoomId", roomId)
                .Set("SaveTime", DateTime.UtcNow)
                .Set("PosX", this.player.X)
                .Set("PosY", this.player.Y)
                .Set("Coins", coinsObject)
                .Set("Deaths", this.player.Deaths)
                .Set("CheckpointX", this.player.Checkpoint.X)
                .Set("CheckpointY", this.player.Checkpoint.Y)
                .Set("TotalMovements", this.player.TotalMovements)
                .Set("IsCheater", this.player.IsCheater)
                .Set("AntiFlightHeat", this.player.AntiFlightHeat)
                .Set("AntiJumpHeat", this.player.AntiJumpHeat)
                .Set("Switches", VarintConverter.GetVarintBytes(this.player.Switches))
                .Set("Team", this.player.Team)
                .Set("Effects", effects)
                .Set("SpeedX", this.player.SpeedX)
                .Set("SpeedY", this.player.SpeedY)
                .Set("PlayTime", (DateTime.UtcNow - this.player.CampaignJoinTime).TotalMinutes);

            dbo.Set(ObjectName, backupObject);
            dbo.Save();
        }

        public void Restore(string roomId, Action<Message> broadcast)
        {
            var dbo = this.campaignPlayer;
            var backupObject = dbo.GetObject(ObjectName);

            if (backupObject == null)
            {
                return;
            }

            if (backupObject.GetString("RoomId") != roomId ||
                (DateTime.UtcNow - backupObject.GetDateTime("SaveTime")).TotalHours > 24)
            {
                // When there is backup data for different world or it expired we should remove it from database
                this.Remove();

                Console.WriteLine("Cleaned old campaign progress backup data.");
                return;
            }

            var coinsObject = backupObject.GetObject("Coins");
            var goldCoins = coinsObject.GetObject("Gold");
            var blueCoins = coinsObject.GetObject("Blue");

            // Reset coinmap and populate it with coins from backup
            this.player.Coinmap = new HashSet<Item>();
            this.RestoreCoins(goldCoins, false);
            this.RestoreCoins(blueCoins, true);

            this.player.X = backupObject.GetDouble("PosX");
            this.player.Y = backupObject.GetDouble("PosY");
            this.player.Deaths = backupObject.GetInt("Deaths");
            this.player.Checkpoint = new Item(new ForegroundBlock(),
                backupObject.GetUInt("CheckpointX"),
                backupObject.GetUInt("CheckpointY"));
            this.player.TotalMovements = backupObject.GetInt("TotalMovements");
            this.player.IsCheater = backupObject.GetBool("IsCheater", false);
            this.player.AntiFlightHeat = backupObject.GetInt("AntiFlightHeat", 0);
            this.player.AntiJumpHeat = backupObject.GetInt("AntiJumpHeat", 0);
            this.player.Switches = new HashSet<int>(VarintConverter.ToInt32List(backupObject.GetBytes("Switches")));
            this.player.SpeedX = backupObject.GetDouble("SpeedX");
            this.player.SpeedY = backupObject.GetDouble("SpeedY");
            this.player.CampaignJoinTime = DateTime.UtcNow - TimeSpan.FromMinutes(backupObject.GetDouble("PlayTime"));

            var m = Message.Create("restoreProgress",
                this.player.Id,
                this.player.X,
                this.player.Y,
                this.player.Coins,
                this.player.BlueCoins,
                goldCoins.GetBytes("PosX"),
                goldCoins.GetBytes("PosY"),
                blueCoins.GetBytes("PosX"),
                blueCoins.GetBytes("PosY"),
                this.player.Deaths,
                this.player.Checkpoint.X,
                this.player.Checkpoint.Y,
                VarintConverter.GetVarintBytes(this.player.Switches),
                this.player.SpeedX,
                this.player.SpeedY);

            // Restore and broadcast team
            this.player.Team = backupObject.GetInt("Team");
            broadcast(Message.Create("team", this.player.Id, this.player.Team));

            // Restore and broadcast effects
            this.player.ResetEffects();
            var effects = backupObject.GetArray("Effects");
            foreach (DatabaseObject effectObject in effects)
            {
                var id = effectObject.GetInt("Id");
                if (id == (int)EffectId.Multijump)
                {
                    var arg = effectObject.GetInt("Arg");
                    var effect = new Effect(id, arg);
                    effect.CanExpire = false;
                    this.player.AddEffect(effect);
                    broadcast(Message.Create("effect", this.player.Id, id, true, arg));
                }
                else if (id == (int)EffectId.Gravity)
                {
                    var arg = effectObject.GetInt("Arg");
                    var effect = new Effect(id, arg);
                    effect.CanExpire = false;
                    this.player.AddEffect(effect);
                    broadcast(Message.Create("effect", this.player.Id, id, true, arg));
                }
                else
                {
                    this.player.AddEffect(new Effect(id));
                    broadcast(Message.Create("effect", this.player.Id, id, true));
                }
            }

            broadcast(m);

            this.player.SendMessage("write", ChatUtils.SystemName,
                "It looks like you were here few minutes ago. We have restored your previous progress so you don't have to replay everything. Good luck!");
        }

        public void Remove()
        {
            this.campaignPlayer.Remove(ObjectName).Save();
        }

        private void RestoreCoins(DatabaseObject dbo, bool isBlue)
        {
            var count = dbo.GetInt("Count");

            if (isBlue)
            {
                player.BlueCoins = count;
            }
            else
            {
                player.Coins = count;
            }

            var xs = dbo.GetBytes("PosX");
            var ys = dbo.GetBytes("PosY");

            for (var i = 0; i < xs.Length; i += 2)
            {
                var x = (uint)((xs[i] << 8) + xs[i + 1]);
                var y = (uint)((ys[i] << 8) + ys[i + 1]);

                this.player.Coinmap.Add(new Item(new ForegroundBlock((uint)(isBlue ? 101 : 100)), x, y));
            }
        }

        private Tuple<byte[], byte[]> GetPositionsArray(List<Item> items)
        {
            var count = items.Count;

            var xs = new byte[count * 2];
            var ys = new byte[count * 2];

            for (var i = 0; i < count; i++)
            {
                var item = items[i];

                xs[i * 2] = (byte)((item.X & 0x0000ff00) >> 8);
                xs[i * 2 + 1] = (byte)(item.X & 0x000000ff);

                ys[i * 2] = (byte)((item.Y & 0x0000ff00) >> 8);
                ys[i * 2 + 1] = (byte)(item.Y & 0x000000ff);
            }

            return new Tuple<byte[], byte[]>(xs, ys);
        }
    }
}