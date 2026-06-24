using System;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 赌轮算法
    /// </summary>
    public class TezIntRoulette<Name> : ITezCloseable
    {
        TezStepArray<TezTuple<Name, int>> m_Value = null;

        public int maxValue { get; private set; }

        public TezIntRoulette(int max_capacity)
        {
            m_Value = new TezStepArray<TezTuple<Name, int>>(max_capacity);
        }

        public TezIntRoulette(TezTuple<Name, int>[] tuples)
        {
            m_Value = new TezStepArray<TezTuple<Name, int>>(tuples);
            foreach (var value in m_Value)
            {
                this.maxValue += value.v2;
            }
        }

        public void add(Name name, int point)
        {
            if (point <= 0)
            {
                throw new ArgumentOutOfRangeException("TezIntRoulette [Point] must greater than 0");
            }

            if (m_Value.count == m_Value.capacity)
            {
                throw new ArgumentOutOfRangeException("TezIntRoulette capacity not enough");
            }

            m_Value.add(new TezTuple<Name, int>(name, point));
            this.maxValue += point;
        }

        /// <summary>
        /// 按Index移除
        /// </summary>
        public void removeAt(int index)
        {
            this.maxValue -= m_Value[index].v2;
            m_Value.removeAt(index);
        }

        public Name roll(int value)
        {
            int sun = 0;
            for (int i = 0; i < m_Value.count; i++)
            {
                sun += m_Value[i].v2;
                if (value < sun)
                {
                    return m_Value[i].v1;
                }
            }

            throw new ArgumentOutOfRangeException(string.Format("TezIntRoulette roll {0}/{1}/{2}", value, sun, this.maxValue));
        }

        public void reset()
        {
            m_Value.clear();
        }

        public void close()
        {
            m_Value.close();
            m_Value = null;
        }
    }
}