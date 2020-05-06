namespace EverybodyEdits.Game
{
    public class BrickPortal : Brick
    {
        public BrickPortal(uint rotation, uint id, uint target) : base(242U)
        {
            this.rotation = rotation;
            this.id = id;
            this.target = target;
        }

        public uint rotation = 0U;

        public uint id = 0U;

        public uint target = 0U;
    }
}
