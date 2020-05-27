namespace EverybodyEdits.Game.CountWorld
{
    public class BlockLayer<T> where T : struct
    {
        private readonly T[,] blocks;

        internal BlockLayer(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.blocks = new T[width, height];
        }

        public int Width { get; private set; }
        public int Height { get; private set; }

        public virtual T this[uint x, uint y]
        {
            get { return this.blocks[x, y]; }
            set { this.blocks[x, y] = value; }
        }

        public T[,] Clone()
        {
            return (T[,]) this.blocks.Clone();
        }
    }
}