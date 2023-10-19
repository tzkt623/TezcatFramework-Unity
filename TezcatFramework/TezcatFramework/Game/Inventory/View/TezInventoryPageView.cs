﻿using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// 卡尺式Inventory分页显示器
    /// </summary>
    public class TezInventoryPageView : TezInventoryBaseView
    {
        /// <summary>
        /// 通单个知槽位刷新
        /// </summary>
        public event TezEventExtension.Action<TezInventoryViewSlot> onSlotRefresh;

        List<ITezInventoryViewSlotData> mDataSlotList = new List<ITezInventoryViewSlotData>();
        TezStepArray<TezInventoryViewSlot> mSlots = null;
        List<int> mFreeIndex = new List<int>();
        bool mFreeIndexDirty = false;

        int mBeginPos = 0;

        List<TezInventoryUniqueItemInfo> mUniqueItems = null;
        Dictionary<long, TezInventoryStackedItemInfo> mStackedItems = null;

        public int capacity
        {
            get { return mSlots.capacity; }
        }

        public TezInventoryViewSlot this[int index]
        {
            get { return mSlots[index]; }
        }

        public override void setInventory(TezInventory inventory, List<TezInventoryUniqueItemInfo> uniqueList, Dictionary<long, TezInventoryStackedItemInfo> stackedDict)
        {
            base.setInventory(inventory, uniqueList, stackedDict);
            mUniqueItems = uniqueList;
            mStackedItems = stackedDict;
            mBeginPos = 0;

            this.onFilterChanged();
        }

        public override void close()
        {
            foreach (var item in mSlots)
            {
                item.close();
            }
            mSlots.close();

            mUniqueItems.Clear();
            mStackedItems.Clear();
            mDataSlotList.Clear();
            mFreeIndex.Clear();

            mUniqueItems = null;
            mStackedItems = null;
            mDataSlotList = null;
            mFreeIndex = null;
            mSlots = null;

            onSlotRefresh = null;

            base.close();
        }

        public void setPageCapacity(int capacity)
        {
            mSlots = new TezStepArray<TezInventoryViewSlot>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                mSlots.add(new TezInventoryViewSlot() { index = i });
            }
        }

        public void paging(int beginPos)
        {
            mBeginPos = beginPos;
            for (int i = 0; i < mSlots.capacity; i++)
            {
                var view_slot = mSlots[i];
                var index = mBeginPos + i;
                if (index < mDataSlotList.Count)
                {
                    view_slot.data = mDataSlotList[index];
                }
                else
                {
                    view_slot.data = null;
                }

                onSlotRefresh?.Invoke(view_slot);
            }
        }

        public void page(int pageIndex)
        {
            mBeginPos = pageIndex * this.capacity;
            for (int i = 0; i < mSlots.capacity; i++)
            {
                var index = mBeginPos + i;
                var view_slot = mSlots[i];
                if (index < mDataSlotList.Count)
                {
                    view_slot.data = mDataSlotList[index];
                }
                else
                {
                    view_slot.data = null;
                }

                onSlotRefresh?.Invoke(view_slot);
            }
        }

        protected override void onFilterChanged()
        {
            mDataSlotList.Clear();
            mFreeIndex.Clear();

            for (int i = 0; i < mUniqueItems.Count; i++)
            {
                var data = mUniqueItems[i];
                if (mFilterManager.filter(data))
                {
                    data.viewIndex = mDataSlotList.Count;
                    mDataSlotList.Add(data);
                }
            }

            foreach (var pair in mStackedItems)
            {
                var info = pair.Value;
                for (int i = 0; i < info.list.Count; i++)
                {
                    var data = info.list[i];
                    if (mFilterManager.filter(data))
                    {
                        data.viewIndex = mDataSlotList.Count;
                        mDataSlotList.Add(data);
                    }
                }
            }

            this.page(0);
        }

        private bool inPageRange(int viewIndex)
        {
            return (viewIndex >= mBeginPos) && (viewIndex < mBeginPos + mSlots.capacity);
        }

        private int giveIndex()
        {
            if (mFreeIndexDirty)
            {
                mFreeIndexDirty = false;
                mFreeIndex.Sort((int a, int b) =>
                {
                    return b.CompareTo(a);
                });
            }

            var index = mFreeIndex[mFreeIndex.Count - 1];
            mFreeIndex.RemoveAt(mFreeIndex.Count - 1);
            return index;
        }

        public override void addViewSlotData(ITezInventoryViewSlotData data)
        {
            if (!mFilterManager.filter(data))
            {
                return;
            }

            if (mFreeIndex.Count > 0)
            {
                data.viewIndex = this.giveIndex();
                mDataSlotList[data.viewIndex] = data;
            }
            else
            {
                data.viewIndex = mDataSlotList.Count;
                mDataSlotList.Add(data);
            }

            var view_index = data.viewIndex;
            if (this.inPageRange(view_index))
            {
                var view_slot = mSlots[view_index - mBeginPos];
                view_slot.data = data;
                onSlotRefresh?.Invoke(view_slot);
            }
        }

        public override void removeViewSlotData(ITezInventoryViewSlotData data)
        {
            mFreeIndex.Add(data.viewIndex);
            mFreeIndexDirty = true;

            mDataSlotList[data.viewIndex] = null;

            var view_index = data.viewIndex;
            if (this.inPageRange(view_index))
            {
                var view_slot = mSlots[view_index - mBeginPos];
                view_slot.data = null;
                onSlotRefresh?.Invoke(view_slot);
            }
        }

        public override void updateViewSlotData(int viewIndex)
        {
            if (this.inPageRange(viewIndex))
            {
                onSlotRefresh?.Invoke(mSlots[viewIndex - mBeginPos]);
            }
        }
    }
}