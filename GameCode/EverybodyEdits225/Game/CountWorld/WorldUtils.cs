namespace EverybodyEdits.Game.CountWorld
{
    public static class WorldUtils
    {
        public static CountWorld GetClearedWorld(int width, int height, uint borderBlock)
        {
            var world = new CountWorld(width, height);

            // Border drawing
            var maxX = (uint) (width - 1);
            var maxY = (uint) (height - 1);
            var block = new ForegroundBlock(borderBlock);
            for (uint y = 0; y <= maxY; y++)
            {
                world.Foreground[0, y] = block;
                world.Foreground[maxX, y] = block;
            }
            for (uint x = 0; x <= maxX; x++)
            {
                world.Foreground[x, 0] = block;
                world.Foreground[x, maxY] = block;
            }

            return world;
        }
    }
}