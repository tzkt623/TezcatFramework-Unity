using System;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Utility
{
    public interface ITezBinaryHeapItem<T> : IComparable<T>
    {
        int index { get; set; }
        bool sameAs(T other);
    }

    public class TezTestBHItem : ITezBinaryHeapItem<TezTestBHItem>
    {
        public int id { get; set; }

        int ITezBinaryHeapItem<TezTestBHItem>.index
        {
            get; set;
        }

        int IComparable<TezTestBHItem>.CompareTo(TezTestBHItem other)
        {
            return id.CompareTo(other.id);
        }

        bool ITezBinaryHeapItem<TezTestBHItem>.sameAs(TezTestBHItem other)
        {
            throw new NotImplementedException();
        }
    }


    /// <summary>
    /// 
    /// 二叉堆
    /// 
    /// 高优先级在前
    /// 
    /// </summary>
    public class TezBinaryHeap<T> where T : ITezBinaryHeapItem<T>
    {
        T[] m_Items = null;

        int m_GrowCount = 3;
        int m_Capacity = 8;
        public int capacity
        {
            get { return m_Capacity; }
            set
            {
                if (m_Capacity < value)
                {
                    T[] temp = new T[value];
                    Array.Copy(m_Items, temp, this.count);
                    m_Items = temp;
                }

                m_Capacity = value;
            }
        }

        public int count { get; private set; } = 0;

        public TezBinaryHeap()
        {
            this.init(8);
        }

        public TezBinaryHeap(int capacity)
        {
            this.init(capacity);
        }

        private void init(int capacity)
        {
            m_Capacity = capacity;
            m_Items = new T[m_Capacity];
        }

        private void grow()
        {
            if (count + 1 >= m_Capacity)
            {
                m_Capacity += m_GrowCount;
                m_GrowCount = m_GrowCount + (m_GrowCount >> 1) + 1;

                T[] temp = new T[m_Capacity];
                Array.Copy(m_Items, temp, this.count);
                m_Items = temp;
            }
        }

        public void push(T item)
        {
            this.grow();

            item.index = count;
            m_Items[count] = item;
            this.sortUp(item);

            count += 1;
        }

        public T pop()
        {
            T first = m_Items[0];
            count -= 1;

            m_Items[0] = m_Items[count];
            m_Items[0].index = 0;

            this.sortDown(m_Items[0]);
            m_Items[count] = default(T);

            return first;
        }

        public bool contains(T item)
        {
            var index = item.index;
            if(index >= 0 && index < this.count)
            {
                var select = m_Items[index];
                if (select != null)
                {
                    return select.sameAs(item);
                }
            }

            return false;
        }

        /// <summary>
        /// 重置Heap为初始状态
        /// </summary>
        public void reset()
        {
            count = 0;
            m_GrowCount = 3;
            m_Capacity = 8;
            m_Items = new T[m_Capacity];
        }

        /// <summary>
        /// 清空Heap中的Item
        /// </summary>
        public void clear()
        {
            Array.Clear(m_Items, 0, m_Items.Length);
            count = 0;
        }

        public void debug(TezEventExtension.Action<int, T> action)
        {
            for (int i = 0; i < this.count; i++)
            {
                action(i, m_Items[i]);
            }
        }

        /// <summary>
        /// 将一个元素向下移动
        /// </summary>
        private void sortDown(T item)
        {
            while (true)
            {
                ///左子树
                int left_index = item.index * 2 + 1;
                ///右子树
                int right_index = item.index * 2 + 2;

                int swap_index = 0;

                ///如果左子树没有越界
                if (left_index < count)
                {
                    ///暂时保存
                    swap_index = left_index;
                    ///如果右子树也没有越界
                    if (right_index < count)
                    {
                        ///比较左右子树的优先级
                        ///选取优先级大的一个作为交换对象
                        if (m_Items[left_index].CompareTo(m_Items[right_index]) < 0)
                        {
                            swap_index = right_index;
                        }
                    }

                    ///比较当前排序对象和交换对象的优先级
                    ///如果当前较小则交换
                    if (item.CompareTo(m_Items[swap_index]) < 0)
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

        /// <summary>
        /// 将一个元素向上移动
        /// </summary>
        private void sortUp(T item)
        {
            ///取得父元素
            int parent_index = (item.index - 1) / 2;
            while (true)
            {
                T parent_item = m_Items[parent_index];

                ///如果当前元素的优先级大于父元素
                ///交换元素位置
                if (item.CompareTo(parent_item) > 0)
                {
                    this.swap(parent_item, item);
                }
                else
                {
                    break;
                }

                ///继续取得上一个父元素
                parent_index = (item.index - 1) / 2;
            }
        }

        private void swap(T item1, T item2)
        {
            m_Items[item1.index] = item2;
            m_Items[item2.index] = item1;

            int temp = item1.index;
            item1.index = item2.index;
            item2.index = temp;
        }
    }
}