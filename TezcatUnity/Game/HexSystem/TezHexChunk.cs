using System;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// Hex块
    /// 用于解决Mesh顶点问题
    /// </summary>
    public class TezHexChunk<Block>
        : ITezCloseable
        where Block : TezHexBlock, new()
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        int mWidth = 0;
        int mHeight = 0;

        Block[,] mBlockArray = null;

        public void setPos(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public void setSize(int width, int height)
        {
            mWidth = width;
            mHeight = height;
            mBlockArray = new Block[mWidth, mHeight];
        }

        public void set(int x, int y, Block block)
        {
            mBlockArray[x, y] = block;
            this.onBlockSetted(block);
        }

        protected virtual void onBlockSetted(Block block)
        {

        }

        public Block get(int x, int y)
        {
            return mBlockArray[x, y];
        }

        public void initBlock(int localX, int localY, Block block)
        {
            if (localX < 0 || localX >= mWidth || localY < 0 || localY >= mHeight)
            {
                throw new IndexOutOfRangeException("Position Out of range");
            }

            mBlockArray[localX, localY] = block;
            this.onBlockInited(block);
        }

        protected virtual void onBlockInited(Block block)
        {

        }

        public bool tryGetBlock(int localX, int localY, out Block block)
        {
            if (localX < 0 || localX >= mWidth || localY < 0 || localY >= mHeight)
            {
                block = null;
                return false;
            }

            block = mBlockArray[localX, localY];
            return block != null;
        }

        public void foreachBlock(TezEventExtension.Action<Block> action)
        {
            foreach (var block in mBlockArray)
            {
                if (block != null)
                {
                    action(block);
                }
            }
        }

        public void foreachBlock(TezEventExtension.Function<bool, Block> function)
        {
            foreach (var block in mBlockArray)
            {
                if (block != null)
                {
                    if (function(block))
                    {
                        return;
                    }
                }
            }
        }

        public void close()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {
            foreach (var block in mBlockArray)
            {
                block?.close();
            }
            mBlockArray = null;
        }
    }
}
