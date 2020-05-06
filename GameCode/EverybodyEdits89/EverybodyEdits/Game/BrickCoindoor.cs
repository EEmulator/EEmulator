namespace EverybodyEdits.Game
{
    public class BrickCoindoor : Brick
    {
        public BrickCoindoor(uint goal) : base(43U)
        {
            this.goal = goal;
        }

        public uint goal = 5U;
    }
}
