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

        int m_Width = 0;
        int m_Height = 0;

        Block[,] m_BlockArray = null;

        public void setPos(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public void setSize(int width, int height)
        {
            m_Width = width;
            m_Height = height;
            m_BlockArray = new Block[m_Width, m_Height];
        }

        public void set(int x, int y, Block block)
        {
            m_BlockArray[x, y] = block;
            this.onBlockSetted(block);
        }

        protected virtual void onBlockSetted(Block block)
        {

        }

        public Block get(int x, int y)
        {
            return m_BlockArray[x, y];
        }

        public void initBlock(int localX, int localY, Block block)
        {
            if (localX < 0 || localX >= m_Width || localY < 0 || localY >= m_Height)
            {
                throw new IndexOutOfRangeException("Position Out of range");
            }

            m_BlockArray[localX, localY] = block;
            this.onBlockInited(block);
        }

        protected virtual void onBlockInited(Block block)
        {

        }

        public bool tryGetBlock(int localX, int localY, out Block block)
        {
            if (localX < 0 || localX >= m_Width || localY < 0 || localY >= m_Height)
            {
                block = null;
                return false;
            }

            block = m_BlockArray[localX, localY];
            return block != null;
        }

        public void foreachBlock(TezEventExtension.Action<Block> action)
        {
            foreach (var block in m_BlockArray)
            {
                if (block != null)
                {
                    action(block);
                }
            }
        }

        public void foreachBlock(TezEventExtension.Function<bool, Block> function)
        {
            foreach (var block in m_BlockArray)
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

        public virtual void close()
        {
            foreach (var block in m_BlockArray)
            {
                block?.close();
            }
            m_BlockArray = null;
        }
    }
}
