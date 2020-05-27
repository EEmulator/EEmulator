using System.Collections.Generic;
using System.Linq;

namespace EverybodyEdits.Game
{
    internal class Keys
    {
        private readonly EverybodyEdits game;
        private readonly Dictionary<string, Key> keys = new Dictionary<string, Key>();

        public Keys(EverybodyEdits game)
        {
            this.game = game;

            this.AddKey("red", 6);
            this.AddKey("green", 7);
            this.AddKey("blue", 8);
            this.AddKey("cyan", 408);
            this.AddKey("magenta", 409);
            this.AddKey("yellow", 410);
        }

        private void AddKey(string id, int blockId)
        {
            this.keys.Add(id, new Key(id, blockId));
        }

        public void Tick()
        {
            foreach (var key in this.keys.Values.Where(key => key.Active && this.game.GetTime() - key.Time > 5000))
            {
                this.game.BroadcastMessage("show", key.Id);
                key.Active = false;
            }
        }

        public void Handle(Player player, uint x, uint y, string keyId)
        {
            if (!this.keys.ContainsKey(keyId))
            {
                return;
            }

            var key = this.keys[keyId];

            if (this.game.BaseWorld.GetBrickType(0, x, y) != key.BlockId)
            {
                return;
            }

            key.Time = this.game.GetTime();
            this.game.BroadcastMessage("hide", key.Id, player.Id);
            key.Active = true;
        }
    }

    internal class Key
    {
        public Key(string id, int blockId)
        {
            this.Id = id;
            this.BlockId = blockId;
        }

        public string Id { get; private set; }
        public int BlockId { get; private set; }
        public double Time { get; set; }
        public bool Active { get; set; }
    }
}