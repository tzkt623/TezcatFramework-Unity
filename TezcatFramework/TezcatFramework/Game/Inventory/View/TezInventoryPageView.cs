using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 卡尺式Inventory分页显示器
    /// </summary>
    public class TezInventoryPageView : TezInventoryBaseView
    {
        /// <summary>
        /// 通知单个槽位刷新
        /// 需要刷新的数据,当前索引
        /// </summary>
        public event TezEventExtension.Action<TezInventoryViewSlot, int> evtSlotRefresh;

        /// <summary>
        /// 当前页码,最大页码
        /// </summary>
        public event TezEventExtension.Action<int, int> evtPageChanged;

        List<ITezInventoryViewSlotData> mDataSlotList = new List<ITezInventoryViewSlotData>();
        TezStepArray<TezInventoryViewSlot> mSlots = null;
        List<int> mFreeIndex = new List<int>();
        bool mFreeIndexDirty = false;

        int mCurrentPage = -1;
        int mMaxPage = -1;
        int mBeginPos = -1;

        public int currentPage => mCurrentPage;
        public int maxPage => mMaxPage;

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

        protected override void onClose()
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

            evtSlotRefresh = null;
            evtPageChanged = null;

            base.onClose();
        }

        public void setPageCapacity(int capacity)
        {
            mSlots = new TezStepArray<TezInventoryViewSlot>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                mSlots.add(new TezInventoryViewSlot() { index = i });
            }
        }

        public void pageDown()
        {
            if (mCurrentPage == 0)
            {
                return;
            }

            this.page(--mCurrentPage);
        }

        public void pageUp()
        {
            if (mCurrentPage == mMaxPage)
            {
                return;
            }

            this.page(++mCurrentPage);
        }

        public void page(int pageIndex)
        {
            if (pageIndex < 0 || pageIndex > mMaxPage)
            {
                return;
            }

            mCurrentPage = pageIndex;
            evtPageChanged?.Invoke(mCurrentPage, mMaxPage);

            mBeginPos = mCurrentPage * this.capacity;
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

                evtSlotRefresh?.Invoke(view_slot, i);
            }
        }

        protected override void onFilterChanged()
        {
            mDataSlotList.Clear();
            mFreeIndex.Clear();

            for (int i = 0; i < mSlots.count; i++)
            {
                mSlots[i].clear();
            }

            for (int i = 0; i < mUniqueItems.Count; i++)
            {
                var data = mUniqueItems[i];
                if (mFilterManager.filter(data))
                {
                    data.viewIndex = mDataSlotList.Count;
                    mDataSlotList.Add(data);
                }
                else
                {
                    data.viewIndex = -1;
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
                    else
                    {
                        data.viewIndex = -1;
                    }
                }
            }

            this.calculateMaxPage();

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

        private void calculateMaxPage()
        {
            mMaxPage = mDataSlotList.Count / this.capacity;
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

            this.calculateMaxPage();

            var view_index = data.viewIndex;
            if (this.inPageRange(view_index))
            {
                var view_slot = mSlots[view_index - mBeginPos];
                view_slot.data = data;
                evtSlotRefresh?.Invoke(view_slot, view_index - mBeginPos);
            }
        }

        public override void removeViewSlotData(ITezInventoryViewSlotData data)
        {
            if(data.viewIndex == -1)
            {
                return;
            }

            var view_index = data.viewIndex;
            mFreeIndex.Add(view_index);
            mFreeIndexDirty = true;

            mDataSlotList[view_index] = null;
            data.viewIndex = -1;

            if (this.inPageRange(view_index))
            {
                var view_slot = mSlots[view_index - mBeginPos];
                view_slot.data = null;
                evtSlotRefresh?.Invoke(view_slot, view_index - mBeginPos);
            }
        }

        public override void updateViewSlotData(int viewIndex)
        {
            if (viewIndex == -1)
            {
                return;
            }

            if (this.inPageRange(viewIndex))
            {
                evtSlotRefresh?.Invoke(mSlots[viewIndex - mBeginPos], viewIndex - mBeginPos);
            }
        }

        public override void debug()
        {
            Console.WriteLine("Debug Being..............");
            for (int i = 0; i < mSlots.capacity; i++)
            {
                evtSlotRefresh?.Invoke(mSlots[i], i);
            }
        }
    }
}