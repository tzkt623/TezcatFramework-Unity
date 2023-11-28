using System;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    public class MyHexArea : TezHexArea<MyHexChunk, MyHexBlock>
    {
        public MyHexArea(int width, int height, int chunkSize)
        {
            this.initChunkArray(width, height, chunkSize);
        }

        protected override void onInitChunk(ref MyHexChunk chunk)
        {

        }
    }

    public class MyHexChunk : TezHexChunk<MyHexBlock>
    {

    }

    public class MyHexBlock : TezHexBlock
    {

    }

    public class TestHexSystem : TezBaseTest
    {
        /// <summary>
        /// Hex边长为2.0f
        /// 布局为Pointy
        /// </summary>
        TezHexGrid mHexGrid = null;

        /// <summary>
        /// 100W
        /// 100H
        /// 100S
        /// ChunkCount = 100W/100S * 100H/100S
        /// </summary>
        MyHexArea mHexArea = null;

        public TestHexSystem() : base("HexSystem")
        {
        }

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
                            mHexArea.initBlock(new TezHexCubeCoordinate(x, y, z), mHexGrid.layout, new MyHexBlock());
                        }
                    }
                }
            }
        }

        public override void init()
        {
//             mHexGrid = new TezHexGrid(2.0f, TezHexGrid.Layout.Pointy);
//             mHexArea = new MyHexArea(100, 100, 100);
        }

        public override void close()
        {
//             mHexGrid.close();
//             mHexArea.close();
        }

        public override void run()
        {
            for (int q = -2; q <= 2; q++)
            {
                for (int r = -2; r <= 2; r++)
                {
                    var cood = new TezHexOffsetCoordinate(q, r);
                    Console.WriteLine(cood.GetHashCode());
                }
            }
        }
    }
}