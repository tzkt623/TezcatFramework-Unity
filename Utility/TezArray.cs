using System;
using System.Collections;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 轻量级Array
    /// 用于小数量
    /// 或者不常变化
    /// 的对象管理 
    /// </summary>
    public class TezArray<T>
        : ITezCloseable
        , IEnumerable
    {
        public int count { get; private set; } = 0;
        public int capacity
        {
            get { return m_Array.Length; }
        }

        T[] m_Array = null;

        public TezArray(int capacity = 0)
        {
            m_Array = new T[capacity];
            this.count = 0;
        }

        public T this[int index]
        {
            get { return m_Array[index]; }
            set { m_Array[index] = value; }
        }

        public void add(T item)
        {
            if (this.count == m_Array.Length)
            {
                T[] new_arrya = new T[this.count + 1];
                Array.Copy(m_Array, new_arrya, m_Array.Length);
                m_Array = new_arrya;
            }

            m_Array[count] = item;
            this.count++;
        }

        public bool remove(T item)
        {
            for (int i = 0; i < this.count; i++)
            {
                if (object.Equals(item, m_Array[i]))
                {
                    this.removeAt(i);
                    return true;
                }
            }

            return false;
        }

        public void removeAt(int index)
        {
            if (index >= count || index < 0)
            {
                throw new IndexOutOfRangeException(string.Format("{0} : Index[{1}] / Count[{2}]", this.GetType().Name, index, this.count));
            }

            if (index == this.count - 1)
            {
                m_Array[index] = default;
            }
            else
            {
                Array.Copy(m_Array, index + 1, m_Array, index, count - index - 1);
                m_Array[count - 1] = default;
            }

            this.count--;
        }

        public void close()
        {
            m_Array = null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Array.GetEnumerator();
        }
    }
}