namespace tezcat.Framework.Game
{
    public struct TezHexArrayIndex
    {
        public int chunkX;
        public int chunkY;
        public int blockX;
        public int blockY;

        public TezHexArrayIndex(int chunkX, int chunkY, int blockX, int blockY)
        {
            this.chunkX = chunkX;
            this.chunkY = chunkY;
            this.blockX = blockX;
            this.blockY = blockY;
        }

        public bool isChunkOutOfRange(int width, int height)
        {
            if (this.chunkX < 0 || this.chunkX >= width || this.chunkY < 0 || this.chunkY >= height)
            {
                return true;
            }

            return false;
        }
    }
}