using System.Collections.Generic;

namespace EverybodyEdits.Game
{
    internal class DBBrick
    {
        public DBBrick(Brick b)
        {
            this.type = b.type;
            if (this.type == 43U)
            {
                this.goal = ((BrickCoindoor)b).goal;
            }
            else if (this.type == 242U)
            {
                this.id = ((BrickPortal)b).id;
                this.target = ((BrickPortal)b).target;
                this.rotation = ((BrickPortal)b).rotation;
            }
            else if (this.type == 1000U)
            {
                this.text = ((BrickLabel)b).text;
            }
            else if (this.type == 77U)
            {
                this.id = ((BrickPiano)b).sound;
            }
            else if (this.type == 83U)
            {
                this.id = ((BrickDrums)b).sound;
            }
        }

        public bool equalBrickValues(Brick b)
        {
            bool result;
            if (b.type != this.type)
            {
                result = false;
            }
            else if (b.type == 43U)
            {
                var door = (BrickCoindoor)b;
                result = (this.goal == door.goal);
            }
            else if (b.type == 242U)
            {
                var portal = (BrickPortal)b;
                result = (portal.id == this.id && portal.rotation == this.rotation && portal.target == this.target);
            }
            else if (b.type == 1000U)
            {
                var i = (BrickLabel)b;
                result = i.text.Equals(this.text);
            }
            else if (b.type == 77U)
            {
                var j = (BrickPiano)b;
                result = j.sound.Equals(this.id);
            }
            else if (b.type == 83U)
            {
                var k = (BrickDrums)b;
                result = k.sound.Equals(this.id);
            }
            else
            {
                result = true;
            }
            return result;
        }

        public uint type;

        public int layer;

        public uint goal;

        public uint rotation = 0U;

        public uint id = 0U;

        public uint target = 0U;

        public string text = null;

        public List<uint> xs = new List<uint>();

        public List<uint> ys = new List<uint>();
    }
}
