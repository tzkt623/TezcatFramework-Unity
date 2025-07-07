using System;
using System.Collections;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{

    public class TezList<T> : IList<T>, ICollection<T>, ITezCloseable
    {
        static T[] Default = new T[0];
        protected T[] m_Data = Default;

        int m_Count = 0;
        public int count
        {
            get { return m_Count; }
        }

        /// <summary>
        /// 容量
        /// 如果设置的容量小于元素数量
        /// 则容量会被设置为数量大小
        /// </summary>
        public int capacity
        {
            get { return m_Data.Length; }
            set
            {
                this.setCapacity(value);
            }
        }

        public TezList()
        {
        }

        public TezList(int capacity = 4)
        {
            if (capacity > 0)
            {
                m_Data = new T[capacity];
            }
        }

        public TezList(T[] array)
        {
            m_Data = array;
            m_Count = array.Length;
        }

        public T this[int index]
        {
            get { return m_Data[index]; }
            set { m_Data[index] = value; }
        }

        private void setCapacity(int newCapacity)
        {
            if (newCapacity > 0)
            {
                newCapacity = System.Math.Max(newCapacity, m_Count);
                if (newCapacity != m_Data.Length)
                {
                    T[] array = new T[newCapacity];
                    if (m_Count > 0)
                    {
                        Array.Copy(m_Data, array, m_Count);
                    }
                    m_Data = array;
                }
            }
            else
            {
                m_Count = 0;
                m_Data = TezList<T>.Default;
            }
        }

        private void grow(int count)
        {
            if (m_Data.Length < count)
            {
                var capacity = (m_Data.Length == 0) ? 4 : m_Data.Length * 2;

                if (capacity < count)
                {
                    capacity = count;
                }

                if (capacity > int.MaxValue)
                {
                    throw new ArgumentOutOfRangeException(nameof(capacity));
                }

                T[] array = new T[capacity];
                if (m_Count > 0)
                {
                    Array.Copy(m_Data, array, m_Count);
                }
                m_Data = array;
            }
        }

        /// <summary>
        /// 清理
        /// 不会删除当前数组本体
        /// 只会把数量归零并清空里面的内容
        /// </summary>
        public void clear()
        {
            if (m_Count > 0)
            {
                Array.Clear(m_Data, 0, m_Data.Length);
                m_Count = 0;
            }
        }

        /// <summary>
        /// 关闭
        /// 清空一切数据
        /// 此对象将不可再用
        /// </summary>
        void ITezCloseable.closeThis()
        {
            m_Data = null;
            m_Count = 0;
        }

        public void add(T element)
        {
            this.grow(m_Count + 1);

            m_Data[m_Count] = element;
            m_Count++;
        }

        public void insert(int index, T element)
        {
            if (index > m_Count || index < 0)
            {
                throw new IndexOutOfRangeException(string.Format("Index : {0} | Count : {1}", index, m_Count));
            }

            this.grow(m_Count + 1);

            if (index < m_Count)
            {
                ///1,2,3,4,5,6
                ///1,2,3,4
                ///I1 I2 3
                Array.Copy(m_Data, index, m_Data, index + 1, m_Count - index);
            }

            m_Data[index] = element;
            m_Count++;
        }

        public void removeAt(int index)
        {
            if (index >= m_Count || index < 0)
            {
                throw new IndexOutOfRangeException(nameof(index));
            }

            m_Count--;
            if (index == m_Count)
            {
                m_Data[index] = default(T);
            }
            else
            {
                Array.Copy(m_Data, index + 1, m_Data, index, m_Count - index);
                m_Data[m_Count] = default(T);
            }
        }

        /// <summary>
        /// 不移动删除点后续数据的删除方式
        /// 会把当前被删掉的位置用最后一个元素来填充
        /// </summary>
        public void removeAtAndFillWithLast(int index)
        {
            if (index >= m_Count || index < 0)
            {
                throw new IndexOutOfRangeException(nameof(index));
            }

            m_Count--;
            if (index == m_Count)
            {
                m_Data[index] = default(T);
            }
            else
            {
                m_Data[index] = m_Data[m_Count];
                m_Data[m_Count] = default(T);
            }
        }

        /// <summary>
        /// 如果数量小于容量的90%
        /// 则把容量设置为当前数量
        /// 以节省空间
        /// </summary>
        public void trimExcess()
        {
            var flag = (int)(m_Data.Length * 0.9f);
            if (m_Count < flag)
            {
                this.setCapacity(m_Count);
            }
        }

        /// <summary>
        /// 拷贝成一份array
        /// </summary>
        public T[] toArray()
        {
            T[] array = new T[m_Count];
            Array.Copy(m_Data, array, m_Count);
            return array;
        }

        #region IList<T>

        int IList<T>.IndexOf(T item)
        {
            for (int i = 0; i < m_Count; i++)
            {
                if (object.Equals(item, m_Data[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        void IList<T>.Insert(int index, T item)
        {
            this.insert(index, item);
        }

        void IList<T>.RemoveAt(int index)
        {
            this.removeAt(index);
        }

        #endregion

        #region ICollection<T>

        int ICollection<T>.Count
        {
            get { return m_Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        void ICollection<T>.Add(T item)
        {
            this.add(item);
        }

        void ICollection<T>.Clear()
        {
            this.clear();
        }

        bool ICollection<T>.Contains(T item)
        {
            for (int i = 0; i < m_Count; i++)
            {
                if (object.Equals(item, m_Data[i]))
                {
                    return true;
                }
            }

            return false;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            if (arrayIndex >= m_Count || arrayIndex < 0)
            {
                throw new IndexOutOfRangeException();
            }

            Array.Copy(m_Data, array, arrayIndex);
        }

        bool ICollection<T>.Remove(T item)
        {
            for (int i = 0; i < m_Count; i++)
            {
                if (object.Equals(item, m_Data[i]))
                {
                    this.removeAt(i);
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region IEnumerator
        class Enumerator : IEnumerator<T>
        {
            T m_Value;
            public T Current => m_Value;
            object IEnumerator.Current => m_Value;

            private int m_Index;
            private TezList<T> m_List;

            public Enumerator(TezList<T> list)
            {
                m_List = list;
            }

            public void Dispose()
            {
                m_Value = default;
                m_List = null;
            }

            public bool MoveNext()
            {
                if (m_Index < m_List.m_Count)
                {
                    m_Value = m_List[m_Index];
                    m_Index++;
                    return true;
                }
                else
                {
                    m_Value = default;
                    return false;
                }
            }

            public void Reset()
            {
                m_Index = 0;
                m_Value = default;
            }
        }


        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Data.GetEnumerator();
        }

        #endregion

    }
}
