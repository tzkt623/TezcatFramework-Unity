using System;


namespace tezcat.Utility
{
    public interface ITezBinaryHeapItem<T> : IComparable<T>
    {
        int index { get; set; }
    }

    public class TezBinaryHeap<T> where T : ITezBinaryHeapItem<T>
    {
        T[] m_Items = null;

        int m_Capacity = 0;

        int m_Count;
        public int count
        {
            get { return m_Count; }
        }

        public TezBinaryHeap()
        {

        }

        public TezBinaryHeap(int capacity)
        {
            this.init(capacity);
        }

        public void init(int capacity)
        {
            m_Capacity = capacity;
            m_Items = new T[m_Capacity];
        }

        public void push(T item)
        {
            item.index = m_Count;
            m_Items[m_Count] = item;
            this.sortUp(item);
            m_Count++;
        }

        public T popFirst()
        {
            T first = m_Items[0];
            m_Count--;
            m_Items[0] = m_Items[m_Count];
            m_Items[0].index = 0;
            this.sortDown(m_Items[0]);
            return first;
        }

        public bool contains(T item)
        {
            return Equals(m_Items[item.index], item);
        }

        public void clear()
        {
            m_Count = 0;
        }

        void sortDown(T item)
        {
            while(true)
            {
                int left_index = item.index * 2 + 1;
                int right_index = item.index * 2 + 2;
                int swap_index = 0;

                if(left_index < m_Count)
                {
                    swap_index = left_index;
                    if(right_index < m_Count)
                    {
                        if(m_Items[left_index].CompareTo(m_Items[right_index]) < 0)
                        {
                            swap_index = right_index;
                        }
                    }

                    if(item.CompareTo(m_Items[swap_index]) < 0)
                    {
                        this.swap(item, m_Items[swap_index]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        void sortUp(T item)
        {
            int parent_index = (item.index - 1) / 2;
            while(true)
            {
                T parent_item = m_Items[parent_index];
                if(item.CompareTo(parent_item) > 0)
                {
                    this.swap(parent_item, item);
                }
                else
                {
                    break;
                }

                parent_index = (item.index - 1) / 2;
            }
        }

        void swap(T item1, T item2)
        {
            m_Items[item1.index] = item2;
            m_Items[item2.index] = item1;

            int temp = item1.index;
            item1.index = item2.index;
            item2.index = temp;
        }
    }
}