namespace EverybodyEdits.Game
{
    internal class BrickLabel : Brick
    {
        public BrickLabel(string txt) : base(1000U)
        {
            this.text = txt;
        }

        public string text = null;
    }
}
