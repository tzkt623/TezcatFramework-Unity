using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    public interface ITezSparseSetItem
    {
        int sparseID { get; }
    }

    public interface ITezReadOnlySparseSet
    {
        bool contain(int id);
        ITezSparseSetItem get(int id);
        bool tryGet(int id, out ITezSparseSetItem item);
        void forEach(Action<ITezSparseSetItem> func);
    }

    public class TezSparseSet : ITezReadOnlySparseSet, ITezCloseable
    {
        int[] mSparseArray;
        List<ITezSparseSetItem> mDenseArray;

        public void init(int capactiy)
        {
            mSparseArray = new int[capactiy];
            mDenseArray = new List<ITezSparseSetItem>(capactiy);
            Array.Fill(mSparseArray, -1);
        }

        public void add(ITezSparseSetItem item)
        {
            mSparseArray[item.sparseID] = mDenseArray.Count;
            mDenseArray.Add(item);
        }

        public void remove(ITezSparseSetItem item)
        {
            var index = mSparseArray[item.sparseID];
            var last_item = mDenseArray[mDenseArray.Count - 1];
            mDenseArray[index] = last_item;

            mSparseArray[last_item.sparseID] = index;
            mSparseArray[item.sparseID] = -1;

            mDenseArray.RemoveAt(mDenseArray.Count - 1);
        }

        public bool contain(int id)
        {
            return mSparseArray[id] != -1;
        }

        public ITezSparseSetItem get(int id)
        {
            return mDenseArray[mSparseArray[id]];
        }

        public bool tryGet(int id, out ITezSparseSetItem item)
        {
            int index = mSparseArray[id];
            if (index != -1)
            {
                item = mDenseArray[index];
                return true;
            }

            item = null;
            return false;
        }

        public void forEach(Action<ITezSparseSetItem> func)
        {
            foreach (var item in mDenseArray)
            {
                func(item);
            }
        }

        public void clear()
        {
            Array.Fill(mSparseArray, -1);
            mDenseArray.Clear();
        }

        public void close()
        {
            mDenseArray.Clear();

            mSparseArray = null;
            mDenseArray = null;
        }
    }

    public interface ITezReadOnlySparseSet<T> where T : class, ITezSparseSetItem
    {
        bool contain(int id);
        T get(int id);
        bool tryGet(int id, out T item);
        void forEach(Action<T> func);
    }

    public class TezSparseSet<T>
        : ITezCloseable
        , ITezReadOnlySparseSet<T>
        where T : class, ITezSparseSetItem
    {
        int[] mSparseArray;
        List<T> mDenseArray;

        public int count => mDenseArray.Count;

        public void init(int capactiy)
        {
            mSparseArray = new int[capactiy];
            mDenseArray = new List<T>(capactiy);
            Array.Fill(mSparseArray, -1);
        }

        public void add(T item)
        {
            mSparseArray[item.sparseID] = mDenseArray.Count;
            mDenseArray.Add(item);
        }

        public void remove(T item)
        {
            //被删除元素的密集数组索引
            var remove_index = mSparseArray[item.sparseID];
            //最后一个密集数组元素
            var last_item = mDenseArray[mDenseArray.Count - 1];
            //交换两个对象在密集数组中的位置
            mDenseArray[remove_index] = last_item;

            //重设最后一个元素在稀疏集中保存的位置
            mSparseArray[last_item.sparseID] = remove_index;
            //删除被删除元素在稀疏集中的位置
            mSparseArray[item.sparseID] = -1;

            //删除密集数组最后一个
            mDenseArray.RemoveAt(mDenseArray.Count - 1);
        }

        public bool contain(int id)
        {
            return mSparseArray[id] != -1;
        }

        public T get(int id)
        {
            return mDenseArray[mSparseArray[id]];
        }

        public bool tryGet(int id, out T item)
        {
            int index = mSparseArray[id];
            if (index != -1)
            {
                item = mDenseArray[index];
                return true;
            }

            item = null;
            return false;
        }

        public void clear()
        {
            Array.Fill(mSparseArray, -1);
            mDenseArray.Clear();
        }

        public void forEach(Action<T> func)
        {
            foreach (var item in mDenseArray)
            {
                func(item);
            }
        }

        public void close()
        {
            mDenseArray.Clear();

            mSparseArray = null;
            mDenseArray = null;
        }
    }
}