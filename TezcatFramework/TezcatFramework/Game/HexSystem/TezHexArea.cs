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
        Chunk[,] mChunkArray = null;

        int mWidth;
        public int width => mWidth;

        int mHeight;
        public int height => mHeight;

        int mChunkSize;
        public int chunkSize => mChunkSize;

        int mHalfWidth;
        public int halfWidth => mHalfWidth;

        int mHalfHeight;
        public int halfHeight => mHalfHeight;


        int mChunkXCount = 0;
        int mChunkYCount = 0;

        /// <summary>
        /// 
        /// </summary>
        protected void initChunkArray(int areaWidth, int areaHeight, int chunkSize)
        {
            mWidth = areaWidth;
            mHeight = areaHeight;

            mHalfWidth = mWidth / 2;
            mHalfHeight = mHeight / 2;

            mChunkSize = chunkSize;
            mChunkXCount = Mathf.CeilToInt((float)mWidth / mChunkSize);
            mChunkYCount = Mathf.CeilToInt((float)mHeight / mChunkSize);

            mChunkArray = new Chunk[mChunkXCount, mChunkYCount];

            for (int y = 0; y < mChunkYCount; y++)
            {
                for (int x = 0; x < mChunkXCount; x++)
                {
                    var chunk = new Chunk();
                    chunk.setPos(x, y);
                    chunk.setSize(mChunkSize, mChunkSize);
                    this.onInitChunk(ref chunk);

                    mChunkArray[x, y] = chunk;
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
            if (array_index.isChunkOutOfRange(mChunkXCount, mChunkYCount))
            {
                throw new IndexOutOfRangeException("Chunk Out Of Range");
            }

            block.coordinate = coordinate;
            mChunkArray[array_index.chunkX, array_index.chunkY].initBlock(array_index.blockX, array_index.blockY, block);
            this.onBlockInited(block);
        }

        protected virtual void onBlockInited(Block block)
        {

        }

        public bool tryGetBlock(TezHexOffsetCoordinate coordinate, out Block block)
        {
            var array_index = this.toArrayIndex(ref coordinate);

            if (array_index.isChunkOutOfRange(mChunkXCount, mChunkYCount))
            {
                block = null;
                return false;
            }

            return mChunkArray[array_index.chunkX, array_index.chunkY].tryGetBlock(
                array_index.blockX,
                array_index.blockY,
                out block);
        }

        protected TezHexArrayIndex toArrayIndex(TezHexOffsetCoordinate coordinate)
        {
            var pos_x = mHalfWidth + coordinate.q;
            var pos_y = mHalfHeight + coordinate.r;

            ///计算数组坐标在哪一个chunk的范围内
            var chunk_x = pos_x / mChunkSize;
            var chunk_y = pos_y / mChunkSize;

            ///计算chunk内的Block的数组坐标
            var block_x = pos_x % mChunkSize;
            var block_y = pos_y % mChunkSize;

            return new TezHexArrayIndex(chunk_x, chunk_y, block_x, block_y);
        }

        protected TezHexArrayIndex toArrayIndex(ref TezHexOffsetCoordinate coordinate)
        {
            var pos_x = mHalfWidth + coordinate.q;
            var pos_y = mHalfHeight + coordinate.r;

            ///计算数组坐标在哪一个chunk的范围内
            var chunk_x = pos_x / mChunkSize;
            var chunk_y = pos_y / mChunkSize;

            ///计算chunk内的Block的数组坐标
            var block_x = pos_x % mChunkSize;
            var block_y = pos_y % mChunkSize;

            return new TezHexArrayIndex(chunk_x, chunk_y, block_x, block_y);
        }

        protected Chunk getChunk(int x, int y)
        {
            return mChunkArray[x, y];
        }

        public void foreachChunk(TezEventExtension.Action<Chunk> action)
        {
            var x_length = mChunkArray.GetLength(0);
            var y_length = mChunkArray.GetLength(1);

            for (int y = 0; y < y_length; y++)
            {
                for (int x = 0; x < x_length; x++)
                {
                    action(mChunkArray[x, y]);
                }
            }
        }

        public void foreachChunk(TezEventExtension.Function<bool, Chunk> function)
        {
            var x_length = mChunkArray.GetLength(0);
            var y_length = mChunkArray.GetLength(1);

            for (int y = 0; y < y_length; y++)
            {
                for (int x = 0; x < x_length; x++)
                {
                    if (function(mChunkArray[x, y]))
                    {
                        return;
                    }
                }
            }
        }

        public virtual void close()
        {
            foreach (var chunk in mChunkArray)
            {
                chunk?.close();
            }
            mChunkArray = null;
        }
    }
}
