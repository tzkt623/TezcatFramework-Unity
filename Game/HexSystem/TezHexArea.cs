using System;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using UnityEngine;

namespace tezcat.Framework.Game
{
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
            var array_index = this.toArrayIndex(ref coordinate);

            if (array_index.isChunkOutOfRange(m_ChunkWidth, m_ChunkHeight))
            {
                block = null;
                return false;
            }

            return m_ChunkArray[array_index.chunk_x, array_index.chunk_y].tryGetBlock(
                array_index.block_x,
                array_index.block_y,
                out block);
        }

        protected TezHexArrayIndex toArrayIndex(ref TezHexOffsetCoordinate coordinate)
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

        public virtual void close(bool self_close = true)
        {
            foreach (var chunk in m_ChunkArray)
            {
                chunk?.close(false);
            }
            m_ChunkArray = null;
        }
    }
}
