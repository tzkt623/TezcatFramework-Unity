using System;
using System.Collections;
using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    public class TezList<T> : IList<T>, ICollection<T>, IEnumerable<T>
    {
        private T[] m_Data = new T[4];

        int m_Count = 0;
        public int count
        {
            get { return m_Count; }
        }

        int m_Capacity = 4;
        public int capacity
        {
            get { return m_Capacity; }
        }

        int ICollection<T>.Count
        {
            get { return m_Count; }
        }

        bool ICollection<T>.IsReadOnly
        {
            get { return false; }
        }

        public T this[int index]
        {
            get { return m_Data[index]; }
            set { m_Data[index] = value; }
        }

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
            var count = m_Data.Length;
            for (int i = 0; i < count; i++)
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
            var count = m_Data.Length;
            for (int i = 0; i < count; i++)
            {
                if (object.Equals(item, m_Data[i]))
                {
                    this.removeAt(i);
                    return true;
                }
            }

            return false;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return (IEnumerator<T>)m_Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Data.GetEnumerator();
        }

        public TezList(int capacity = -1)
        {
            if (capacity <= 0)
            {
                capacity = 4;
            }
            this.setCapacity(capacity);
        }

        public void add(T element)
        {
            if (m_Count == m_Capacity)
            {
                this.setCapacity(m_Capacity * 2);
            }

            m_Data[m_Count] = element;
            m_Count++;
        }

        public void clear()
        {
            m_Count = 0;
        }

        public void insert(int index, T element)
        {
            if (index > m_Count || index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            if (m_Count == m_Capacity)
            {
                this.setCapacity(m_Capacity * 2);
            }
            if (index < m_Count)
            {
                Array.Copy(m_Data, index, m_Data, index + 1, m_Count - index);
            }
            m_Data[index] = element;
            m_Count++;
        }

        public void removeAt(int index)
        {
            if (index >= m_Count || index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            if (index == m_Count - 1)
            {
                m_Data[index] = default(T);
            }
            else
            {
                Array.Copy(m_Data, index + 1, m_Data, index, m_Count - index - 1);
                m_Data[m_Count - 1] = default(T);
            }
            m_Count--;
        }

        public void unsortedRemoveAt(int index)
        {
            if (index >= m_Count || index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            if (index == m_Count - 1)
            {
                m_Data[index] = default(T);
            }
            else
            {
                m_Data[index] = m_Data[m_Count - 1];
                m_Data[m_Count - 1] = default(T);
            }
            m_Count--;
        }

        private void setCapacity(int new_capacity)
        {
            int old = m_Capacity;
            m_Capacity = System.Math.Max(new_capacity, m_Count);

            if (m_Capacity != old)
            {
                T[] array = new T[m_Capacity];
                Array.Copy(m_Data, array, m_Count);
                m_Data = array;
            }
        }
    }
}
