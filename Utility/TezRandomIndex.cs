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
        int[] m_Data;
        int m_Size;
        TezEventExtension.Function<int, int, int> m_Generator;

        public TezRandomIndex(int count, TezEventExtension.Function<int, int, int> randomGenerator)
        {
            m_Data = new int[count];
            for (int i = 0; i < m_Data.Length; i++)
            {
                m_Data[i] = i;
            }

            m_Size = m_Data.Length;
            m_Generator = randomGenerator;
        }

        public TezRandomIndex(int[] sequence, TezEventExtension.Function<int, int, int> randomGenerator)
        {
            m_Data = sequence;
            m_Size = m_Data.Length;
            m_Generator = randomGenerator;

        }

        public void close()
        {
            m_Data = null;
            m_Generator = null;
        }

        public int nextIndex()
        {
            if (m_Size == 0)
            {
                m_Size = m_Data.Length;
            }

            if (m_Size == 1)
            {
                return m_Data[0];
            }

            var data_index = m_Generator(0, m_Size);
            var temp = m_Data[data_index];

            ///交换
            var last = m_Data[m_Size - 1];
            m_Data[m_Size - 1] = temp;
            m_Data[data_index] = last;
            m_Size--;

            return temp;
        }

        public void reset()
        {
            m_Size = m_Data.Length;
        }
    }
}
