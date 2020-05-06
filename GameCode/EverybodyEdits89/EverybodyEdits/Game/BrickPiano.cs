namespace EverybodyEdits.Game
{
    internal class BrickPiano : Brick
    {
        public BrickPiano(uint offset) : base(77U)
        {
            if (offset > 26U)
            {
                offset = 26U;
            }
            this.sound = offset;
        }

        public uint sound = 0U;
    }
}
