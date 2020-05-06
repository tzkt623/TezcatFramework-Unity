using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game
{
    public class TezHexChunk<Block>
        : ITezCloseable
        where Block : TezHexBlock
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
            this.onInitBlockArray(ref m_BlockArray);
        }

        protected virtual void onInitBlockArray(ref Block[,] array)
        {

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

        public bool tryGetBlock(int local_x, int local_y, out Block block)
        {
            if (local_x < 0 || local_x >= m_Width || local_y < 0 || local_y >= m_Height)
            {
                block = null;
                return false;
            }

            block = m_BlockArray[local_x, local_y];
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

        public virtual void close(bool self_close = true)
        {
            foreach (var block in m_BlockArray)
            {
                block?.close(false);
            }
            m_BlockArray = null;
        }
    }
}
