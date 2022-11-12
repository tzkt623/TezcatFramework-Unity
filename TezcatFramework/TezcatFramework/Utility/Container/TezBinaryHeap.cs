using System;
using System.Collections;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Utility
{
    public interface ITezBinaryHeapItem
    {
        int index { get; set; }
    }
    public interface ITezBinaryHeapItem<T>
        : ITezBinaryHeapItem
        , IComparable<T>
    {
    }

    /// <summary>
    /// 
    /// 二叉堆
    /// 
    /// 高优先级在前
    /// 
    /// </summary>
    public class TezBinaryHeap<T>
        : ITezCloseable
        , IEnumerable<T>
        where T : ITezBinaryHeapItem<T>
    {
        T[] mItems = null;

        int mGrowCount = 3;
        int mCapacity = 8;
        public int capacity
        {
            get { return mCapacity; }
            set
            {
                if (mCapacity < value)
                {
                    mCapacity = value;
                    T[] temp = new T[mCapacity];
                    Array.Copy(mItems, temp, mCount);
                    mItems = temp;
                }
            }
        }

        public int count => mCount;
        int mCount = 0;

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
            mCapacity = capacity;
            mItems = new T[mCapacity];
        }

        private void grow()
        {
            if (mCount + 1 >= mCapacity)
            {
                mCapacity += mGrowCount;
                mGrowCount = mGrowCount + (mGrowCount >> 1) + 1;

                T[] temp = new T[mCapacity];
                Array.Copy(mItems, temp, mCount);
                mItems = temp;
            }
        }

        public void push(T item)
        {
            this.grow();

            item.index = mCount;
            mItems[mCount] = item;
            this.sortUp(item);

            mCount += 1;
        }

        public T pop()
        {
            T first = mItems[0];
            mCount -= 1;

            mItems[0] = mItems[mCount];
            mItems[0].index = 0;

            this.sortDown(mItems[0]);
            mItems[mCount] = default(T);

            return first;
        }

        public bool contains(T item)
        {
            var index = item.index;
            if (index >= 0 && index < mCount)
            {
                return item.Equals(mItems[index]);
            }

            return false;
        }

        /// <summary>
        /// 重置Heap为初始状态
        /// </summary>
        public void reset()
        {
            mCount = 0;
            mGrowCount = 3;
            mCapacity = 8;
            mItems = new T[mCapacity];
        }

        /// <summary>
        /// 清空Heap中的Item
        /// </summary>
        public void clear()
        {
            Array.Clear(mItems, 0, mItems.Length);
            mCount = 0;
        }

        public void debug(TezEventExtension.Action<int, T> action)
        {
            for (int i = 0; i < mCount; i++)
            {
                action(i, mItems[i]);
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
                if (left_index < mCount)
                {
                    ///暂时保存
                    swap_index = left_index;
                    ///如果右子树也没有越界
                    if (right_index < mCount)
                    {
                        ///比较左右子树的优先级
                        ///选取优先级大的一个作为交换对象
                        if (mItems[left_index].CompareTo(mItems[right_index]) < 0)
                        {
                            swap_index = right_index;
                        }
                    }

                    ///比较当前排序对象和交换对象的优先级
                    ///如果当前较小则交换
                    if (item.CompareTo(mItems[swap_index]) < 0)
                    {
                        this.swap(item, mItems[swap_index]);
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
                T parent_item = mItems[parent_index];

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
            mItems[item1.index] = item2;
            mItems[item2.index] = item1;

            int temp = item1.index;
            item1.index = item2.index;
            item2.index = temp;
        }

        public void close()
        {
            mItems = null;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new TezCountArrayEnumerator<T>(mItems, mCount);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new TezCountArrayEnumerator<T>(mItems, mCount);
        }
    }
}