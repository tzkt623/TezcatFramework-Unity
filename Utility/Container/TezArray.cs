using System;
using System.Collections;
using System.Collections.Generic;
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
        , IEnumerable<T>
    {
        class Enumerator : IEnumerator<T>
        {
            public T Current { get; private set; }
            object IEnumerator.Current => this.Current;

            private int m_Index;
            private TezArray<T> m_Array;

            public Enumerator(TezArray<T> array)
            {
                m_Array = array;
            }

            public void Dispose()
            {
                this.Current = default;
                m_Array = null;
            }

            public bool MoveNext()
            {
                if (m_Index < m_Array.count)
                {
                    this.Current = m_Array[m_Index];
                    m_Index++;
                    return true;
                }
                else
                {
                    this.Current = default;
                    return false;
                }
            }

            public void Reset()
            {
                m_Index = 0;
                this.Current = default;
            }
        }

        public int count { get; private set; } = 0;

        /// <summary>
        /// 内部数组长度
        /// 不是元素个数
        /// </summary>
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

        public TezArray(T[] array)
        {
            m_Array = array;
            this.count = m_Array.Length;
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

        public T[] toArray()
        {
            T[] array = new T[this.count];
            Array.Copy(m_Array, 0, array, 0, this.count);
            return array;
        }

        /// <summary>
        /// 只清空数组内容
        /// 并将Count设为0
        /// 不会删除数组
        /// 不会改变数组Capacity
        /// </summary>
        public void clear()
        {
            Array.Clear(m_Array, 0, m_Array.Length);
            this.count = 0;
        }

        /// <summary>
        /// 关闭
        /// 会删除整个数组
        /// </summary>
        public void close()
        {
            m_Array = null;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Array.GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }
    }
}