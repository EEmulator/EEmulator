using System.Diagnostics;

namespace EverybodyEdits.Game.CountWorld
{
    [DebuggerDisplay("Id = {Id}")]
    public struct BackgroundBlock
    {
        private readonly uint type;
        private readonly object arg;

        public BackgroundBlock(uint type)
        {
            this.type = type;
            this.arg = null;
        }

        public BackgroundBlock(uint type, uint arg)
        {
            this.type = type;
            this.arg = arg;
        }

        public uint Number
        {
            get { return (uint)this.arg; }
        }

        public uint Type
        {
            get { return this.type; }
        }
    }
}