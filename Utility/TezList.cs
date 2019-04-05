using System;
using System.Collections;
using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    public class TezList<T> : IList<T>, ICollection<T>, IEnumerable<T>
    {
        protected T[] m_Data = new T[4];

        public int count { get; private set; } = 0;
        public int capacity { get; private set; } = 0;

        int ICollection<T>.Count
        {
            get { return count; }
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
            for (int i = 0; i < count; i++)
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
            if (arrayIndex >= count || arrayIndex < 0)
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

        public TezList(int capacity = 4)
        {
            if (capacity <= 0)
            {
                capacity = 4;
            }
            this.setCapacity(capacity);
        }

        public void add(T element)
        {
            if (count == capacity)
            {
                this.setCapacity(capacity * 2);
            }

            m_Data[count] = element;
            count++;
        }

        public void clear()
        {
            this.count = 0;
            this.capacity = 4;
            m_Data = new T[this.capacity];
        }

        public void insert(int index, T element)
        {
            if (index > count || index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            if (count == capacity)
            {
                this.setCapacity(capacity * 2);
            }
            if (index < count)
            {
                Array.Copy(m_Data, index, m_Data, index + 1, count - index);
            }
            m_Data[index] = element;
            count++;
        }

        public void removeAt(int index)
        {
            if (index >= count || index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            if (index == count - 1)
            {
                m_Data[index] = default(T);
            }
            else
            {
                Array.Copy(m_Data, index + 1, m_Data, index, count - index - 1);
                m_Data[count - 1] = default(T);
            }
            count--;
        }

        public void unsortedRemoveAt(int index)
        {
            if (index >= count || index < 0)
            {
                throw new IndexOutOfRangeException();
            }
            if (index == count - 1)
            {
                m_Data[index] = default(T);
            }
            else
            {
                m_Data[index] = m_Data[count - 1];
                m_Data[count - 1] = default(T);
            }
            count--;
        }

        private void setCapacity(int new_capacity)
        {
            int old = capacity;
            capacity = System.Math.Max(new_capacity, count);

            if (capacity != old)
            {
                T[] array = new T[capacity];
                Array.Copy(m_Data, array, count);
                m_Data = array;
            }
        }
    }
}
