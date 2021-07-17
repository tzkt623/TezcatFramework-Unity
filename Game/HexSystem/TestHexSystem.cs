namespace tezcat.Framework.Game
{
    public class TestHexSystem
    {
        /// <summary>
        /// Hex边长为2.0f
        /// 布局为Pointy
        /// </summary>
        TezHexGrid m_HexGrid = new TezHexGrid(2.0f, TezHexGrid.Layout.Pointy);

        /// <summary>
        /// 100W
        /// 100H
        /// 100S
        /// ChunkCount = 100W/100S * 100H/100S
        /// </summary>
        TestHexArea m_HexArea = new TestHexArea(100, 100, 100);

        public void build(int size)
        {
            for (int z = -size; z <= size; z++)
            {
                for (int y = -size; y <= size; y++)
                {
                    for (int x = -size; x <= size; x++)
                    {
                        if (x + y + z == 0)
                        {
                            m_HexArea.initBlock(new TezHexCubeCoordinate(x, y, z), m_HexGrid.layout, new TestHexBlock());
                        }
                    }
                }
            }
        }
    }

    public class TestHexArea : TezHexArea<TestHexChunk, TestHexBlock>
    {
        public TestHexArea(int width, int height, int chunkSize)
        {
            this.initChunkArray(width, height, chunkSize);
        }

        protected override void onInitChunk(ref TestHexChunk chunk)
        {

        }
    }

    public class TestHexChunk : TezHexChunk<TestHexBlock>
    {

    }

    public class TestHexBlock : TezHexBlock
    {

    }
}