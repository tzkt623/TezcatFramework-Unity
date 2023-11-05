using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 
    /// 不重复的随机Index生成器
    /// 
    /// 用于在固定的Index数组里面取得不重复的Index
    /// 
    /// </summary>
    public class TezRandomIndex : ITezCloseable
    {
        int[] mData;
        int mSize;
        TezEventExtension.Function<int, int, int> mGenerator;

        public TezRandomIndex(int min, int max, TezEventExtension.Function<int, int, int> randomGenerator)
        {
            if(min == max)
            {
                mSize = 1;
                mData = new int[mSize];
                mData[0] = min;
            }
            else
            {
                int count = max - min;
                mData = new int[count];

                for (int i = 0; i < count; i++)
                {
                    mData[i] = min + i;
                }

                mSize = mData.Length;
            }

            mGenerator = randomGenerator;
        }

        public TezRandomIndex(int[] sequence, TezEventExtension.Function<int, int, int> randomGenerator)
        {
            mData = sequence;
            mSize = mData.Length;
            mGenerator = randomGenerator;
        }

        public void close()
        {
            mData = null;
            mGenerator = null;
        }

        public int nextIndex()
        {
            if (mSize == 0)
            {
                mSize = mData.Length;
            }

            if (mSize == 1)
            {
                return mData[0];
            }

            var data_index = mGenerator(0, mSize);
            var temp = mData[data_index];

            ///交换
            var last = mData[mSize - 1];
            mData[mSize - 1] = temp;
            mData[data_index] = last;
            mSize--;

            return temp;
        }

        public void reset()
        {
            mSize = mData.Length;
        }
    }
}
