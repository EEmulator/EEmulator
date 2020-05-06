namespace EverybodyEdits.Game
{
    internal class BrickDrums : Brick
    {
        public BrickDrums(uint offset) : base(83U)
        {
            if (offset > 9U)
            {
                offset = 9U;
            }
            this.sound = offset;
        }

        public uint sound = 0U;
    }
}
