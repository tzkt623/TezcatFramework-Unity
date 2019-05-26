using System;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using UnityEngine;

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
    }

    public class TezHexArea<Chunk, Block>
        : ITezCloseable
        where Chunk : TezHexChunk<Block>, new()
        where Block : TezHexBlock, new()
    {
        Chunk[,] m_ChunkArray = null;

        public int width { get; private set; }
        public int height { get; private set; }
        public int chunkSize { get; private set; }

        public int halfWidth { get; private set; }
        public int halfHeight { get; private set; }

        int m_ChunkWidth = 0;
        int m_ChunkHeight = 0;

        protected void initChunkArray(int width, int height, int chunk_size)
        {
            this.width = width;
            this.height = height;

            halfWidth = this.width / 2;
            halfHeight = this.height / 2;

            chunkSize = chunk_size;
            m_ChunkWidth = Mathf.CeilToInt((float)this.width / chunkSize);
            m_ChunkHeight = Mathf.CeilToInt((float)this.height / chunkSize);

            m_ChunkArray = new Chunk[m_ChunkWidth, m_ChunkHeight];

            for (int y = 0; y < m_ChunkHeight; y++)
            {
                for (int x = 0; x < m_ChunkWidth; x++)
                {
                    var chunk = new Chunk();
                    chunk.setPos(x, y);
                    chunk.setSize(chunkSize, chunkSize);
                    this.onInitChunk(ref chunk);
                    m_ChunkArray[x, y] = chunk;
                }
            }
        }

        protected virtual void onInitChunk(ref Chunk chunk)
        {
            throw new Exception(string.Format("{0} : Please override this method", this.GetType().Name));
        }

        public bool tryGetBlock(TezHexOffsetCoordinate coordinate, out Block block)
        {
            var array_index = this.toArrayIndex(coordinate);

            int chunk_x = array_index.chunk_x;
            int chunk_y = array_index.chunk_y;

            if (chunk_x < 0 || chunk_x >= m_ChunkWidth || chunk_y < 0 || chunk_y >= m_ChunkHeight)
            {
                block = null;
                return false;
            }

            var local_x = array_index.block_x;
            var local_y = array_index.block_y;

            return m_ChunkArray[chunk_x, chunk_y].tryGetBlock(local_x, local_y, out block);
        }

        public TezHexArrayIndex toArrayIndex(TezHexOffsetCoordinate coordinate)
        {
            var pos_x = halfWidth + coordinate.q;
            var pos_y = halfHeight + coordinate.r;

            ///计算数组坐标在哪一个chunk的范围内
            var chunk_x = pos_x / chunkSize;
            var chunk_y = pos_y / chunkSize;

            ///计算chunk内的Block的数组坐标
            var block_x = pos_x % chunkSize;
            var block_y = pos_y % chunkSize;

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
