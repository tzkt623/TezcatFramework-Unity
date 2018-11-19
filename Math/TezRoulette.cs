using System;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Math
{
    /// <summary>
    /// 赌轮算法
    /// </summary>
    public class TezIntRoulette<Name>
    {
        TezTuple<Name, int>[] m_Value = null;
        int m_Size = 0;
        int m_Capacity = 0;

        public int maxValue { get; private set; }

        public TezIntRoulette(int max_capacity)
        {
            m_Capacity = max_capacity;
            m_Value = new TezTuple<Name, int>[m_Capacity];
        }

        ~TezIntRoulette()
        {
            m_Value = null;
        }

        public void add(Name name, int point)
        {
            if(point == 0)
            {
                return;
            }

            if(m_Size >= m_Capacity)
            {
                throw new ArgumentOutOfRangeException("TezIntRoulette m_Size");
            }

            m_Value[m_Size++] = new TezTuple<Name, int>(name, point);
            this.maxValue += point;
        }

        public Name roll(int value)
        {
            int sun = 0;
            for (int i = 0; i < m_Size; i++)
            {
                sun += m_Value[i].v2;
                if(value < sun)
                {
                    return m_Value[i].v1;
                }
            }

            throw new ArgumentOutOfRangeException(string.Format("TezIntRoulette roll {0}/{1}", value, sun));
        }

        public void reset()
        {
            m_Value = new TezTuple<Name, int>[m_Capacity];
        }
    }
}