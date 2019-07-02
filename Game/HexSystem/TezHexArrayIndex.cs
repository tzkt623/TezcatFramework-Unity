namespace tezcat.Framework.Game
{
    public struct TezHexArrayIndex
    {
        public int chunk_x;
        public int chunk_y;
        public int block_x;
        public int block_y;

        public TezHexArrayIndex(int c_x, int c_y, int b_x, int b_y)
        {
            this.chunk_x = c_x;
            this.chunk_y = c_y;
            this.block_x = b_x;
            this.block_y = b_y;
        }

        public bool isChunkOutOfRange(int width, int height)
        {
            if (chunk_x < 0 || chunk_x >= width || chunk_y < 0 || chunk_y >= height)
            {
                return true;
            }

            return false;
        }
    }
}