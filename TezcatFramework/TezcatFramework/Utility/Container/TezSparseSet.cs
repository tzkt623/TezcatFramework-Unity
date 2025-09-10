using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    public interface ITezSparseSetItem
    {
        int sparseID { get; }
    }

    public class TezSparseSet : ITezCloseable
    {
        int[] mSparseArray;
        List<ITezSparseSetItem> mDenseArray;

        public void init(int capactiy)
        {
            mSparseArray = new int[capactiy];
            Array.Clear(mSparseArray, -1, capactiy);
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

        public void clear()
        {
            Array.Clear(mSparseArray, -1, mSparseArray.Length);
            mDenseArray.Clear();
        }

        public void close()
        {
            mDenseArray.Clear();

            mSparseArray = null;
            mDenseArray = null;
        }
    }
}