using System;
using System.Collections;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 轻量级Array
    /// 用于容量不常变化的对象管理
    /// 添加元素超过容量时
    /// 容量只会1个1个地增长
    /// </summary>
    public class TezStepArray<T>
        : ITezCloseable
        , IEnumerable<T>
    {
        class Enumerator : IEnumerator<T>
        {
            public T Current { get; private set; }
            object IEnumerator.Current => this.Current;

            private int m_Index;
            private TezStepArray<T> m_Array;

            public Enumerator(TezStepArray<T> array)
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
                if (m_Index < m_Array.m_Count)
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

        static T[] m_Default = new T[0];
        T[] m_Array = m_Default;
        int m_Count = 0;

        public int count => m_Count;

        /// <summary>
        /// 内部数组长度
        /// 不是元素个数
        /// </summary>
        public int capacity
        {
            get { return m_Array.Length; }
            set
            {
                this.grow(value);
            }
        }


        public TezStepArray(int capacity = 4)
        {
            m_Array = new T[capacity];
            m_Count = 0;
        }

        public TezStepArray(T[] array)
        {
            m_Array = array;
            m_Count = m_Array.Length;
        }

        public TezStepArray(TezStepArray<T> other)
        {
            m_Array = new T[other.count];
            m_Count = other.count;
            Array.Copy(other.m_Array, m_Array, m_Count);
        }

        public T this[int index]
        {
            get { return m_Array[index]; }
            set { m_Array[index] = value; }
        }

        private void grow(int count)
        {
            if (count > m_Array.Length)
            {
                T[] new_array = new T[count];
                Array.Copy(m_Array, new_array, m_Count);
                m_Array = new_array;
            }
        }

        public void add(T item)
        {
            this.grow(m_Count + 1);
            m_Array[m_Count] = item;
            m_Count++;
        }

        public bool remove(T item)
        {
            for (int i = 0; i < this.m_Count; i++)
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
            if (index >= m_Count || index < 0)
            {
                throw new IndexOutOfRangeException(string.Format("{0} : Index[{1}] / Count[{2}]", this.GetType().Name, index, this.m_Count));
            }

            m_Count--;
            if (index == m_Count)
            {
                m_Array[m_Count] = default;
            }
            else
            {
                Array.Copy(m_Array, index + 1, m_Array, index, m_Count - index);
                m_Array[m_Count] = default;
            }
        }

        public T[] toArray()
        {
            T[] array = new T[m_Count];
            Array.Copy(m_Array, 0, array, 0, m_Count);
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
            if (m_Count > 0)
            {
                Array.Clear(m_Array, 0, m_Array.Length);
                m_Count = 0;
            }
        }

        /// <summary>
        /// 关闭
        /// 清空一切数据
        /// 此对象将不可再用
        /// </summary>
        void ITezCloseable.deleteThis()
        {
            m_Array = null;
            m_Count = 0;
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