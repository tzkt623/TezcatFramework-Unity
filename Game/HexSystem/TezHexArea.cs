using System;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using UnityEngine;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// Hex区域
    /// 用于包含整个Hex
    /// 里面用Chunk划分出多个块
    /// 用于解决Mesh顶点问题
    /// 
    /// width,height是抽象意义上的
    /// 用于内部坐标计算的转换
    /// 除以chunksize得到的是chunk的数量
    /// </summary>
    public class TezHexArea<Chunk, Block>
        : ITezCloseable
        where Chunk : TezHexChunk<Block>, new()
        where Block : TezHexBlock, new()
    {
        Chunk[,] m_ChunkArray = null;

        int m_Width;
        public int width => m_Width;

        int m_Height;
        public int height => m_Height;

        int m_ChunkSize;
        public int chunkSize => m_ChunkSize;

        int m_HalfWidth;
        public int halfWidth => m_HalfWidth;

        int m_HalfHeight;
        public int halfHeight => m_HalfHeight;


        int m_ChunkXCount = 0;
        int m_ChunkYCount = 0;

        /// <summary>
        /// 
        /// </summary>
        protected void initChunkArray(int areaWidth, int areaHeight, int chunkSize)
        {
            m_Width = areaWidth;
            m_Height = areaHeight;

            m_HalfWidth = m_Width / 2;
            m_HalfHeight = m_Height / 2;

            m_ChunkSize = chunkSize;
            m_ChunkXCount = Mathf.CeilToInt((float)m_Width / m_ChunkSize);
            m_ChunkYCount = Mathf.CeilToInt((float)m_Height / m_ChunkSize);

            m_ChunkArray = new Chunk[m_ChunkXCount, m_ChunkYCount];

            for (int y = 0; y < m_ChunkYCount; y++)
            {
                for (int x = 0; x < m_ChunkXCount; x++)
                {
                    var chunk = new Chunk();
                    chunk.setPos(x, y);
                    chunk.setSize(m_ChunkSize, m_ChunkSize);
                    this.onInitChunk(ref chunk);

                    m_ChunkArray[x, y] = chunk;
                }
            }
        }

        protected virtual void onInitChunk(ref Chunk chunk)
        {
            throw new Exception(string.Format("{0} : Please override this method", this.GetType().Name));
        }

        /// <summary>
        /// 初始化Block
        /// </summary>
        public void initBlock(TezHexCubeCoordinate coordinate, TezHexGrid.Layout layout, Block block)
        {
            var array_index = this.toArrayIndex(coordinate.toOffset(layout));
            if (array_index.isChunkOutOfRange(m_ChunkXCount, m_ChunkYCount))
            {
                throw new IndexOutOfRangeException("Chunk Out Of Range");
            }

            block.coordinate = coordinate;
            m_ChunkArray[array_index.chunkX, array_index.chunkY].initBlock(array_index.blockX, array_index.blockY, block);
            this.onBlockInited(block);
        }

        protected virtual void onBlockInited(Block block)
        {

        }

        public bool tryGetBlock(TezHexOffsetCoordinate coordinate, out Block block)
        {
            var array_index = this.toArrayIndex(ref coordinate);

            if (array_index.isChunkOutOfRange(m_ChunkXCount, m_ChunkYCount))
            {
                block = null;
                return false;
            }

            return m_ChunkArray[array_index.chunkX, array_index.chunkY].tryGetBlock(
                array_index.blockX,
                array_index.blockY,
                out block);
        }

        protected TezHexArrayIndex toArrayIndex(TezHexOffsetCoordinate coordinate)
        {
            var pos_x = m_HalfWidth + coordinate.q;
            var pos_y = m_HalfHeight + coordinate.r;

            ///计算数组坐标在哪一个chunk的范围内
            var chunk_x = pos_x / m_ChunkSize;
            var chunk_y = pos_y / m_ChunkSize;

            ///计算chunk内的Block的数组坐标
            var block_x = pos_x % m_ChunkSize;
            var block_y = pos_y % m_ChunkSize;

            return new TezHexArrayIndex(chunk_x, chunk_y, block_x, block_y);
        }

        protected TezHexArrayIndex toArrayIndex(ref TezHexOffsetCoordinate coordinate)
        {
            var pos_x = m_HalfWidth + coordinate.q;
            var pos_y = m_HalfHeight + coordinate.r;

            ///计算数组坐标在哪一个chunk的范围内
            var chunk_x = pos_x / m_ChunkSize;
            var chunk_y = pos_y / m_ChunkSize;

            ///计算chunk内的Block的数组坐标
            var block_x = pos_x % m_ChunkSize;
            var block_y = pos_y % m_ChunkSize;

            return new TezHexArrayIndex(chunk_x, chunk_y, block_x, block_y);
        }

        protected Chunk getChunk(int x, int y)
        {
            return m_ChunkArray[x, y];
        }

        public void foreachChunk(TezEventExtension.Action<Chunk> action)
        {
            var x_length = m_ChunkArray.GetLength(0);
            var y_length = m_ChunkArray.GetLength(1);

            for (int y = 0; y < y_length; y++)
            {
                for (int x = 0; x < x_length; x++)
                {
                    action(m_ChunkArray[x, y]);
                }
            }
        }

        public void foreachChunk(TezEventExtension.Function<bool, Chunk> function)
        {
            var x_length = m_ChunkArray.GetLength(0);
            var y_length = m_ChunkArray.GetLength(1);

            for (int y = 0; y < y_length; y++)
            {
                for (int x = 0; x < x_length; x++)
                {
                    if (function(m_ChunkArray[x, y]))
                    {
                        return;
                    }
                }
            }
        }

        public virtual void close()
        {
            foreach (var chunk in m_ChunkArray)
            {
                chunk?.close();
            }
            m_ChunkArray = null;
        }
    }
}
