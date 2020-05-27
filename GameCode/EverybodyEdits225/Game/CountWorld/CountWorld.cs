using System.Diagnostics;

namespace EverybodyEdits.Game.CountWorld
{
    [DebuggerDisplay("Width = {Width}, Height = {Height}")]
    public class CountWorld
    {
        public CountWorld(int width, int height)
        {
            this.Height = height;
            this.Width = width;
            this.Foreground = new CountForegroundLayer(width, height);
            this.Background = new BlockLayer<BackgroundBlock>(width, height);
        }

        public BlockLayer<BackgroundBlock> Background { get; private set; }
        public CountForegroundLayer Foreground { get; private set; }
        public int Height { get; private set; }
        public int Width { get; private set; }
    }
}