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

        TezStepArray<TezInventoryViewSlot> mSlots = null;
        int mBeginPos = 0;

        public int capacity
        {
            get { return mSlots.capacity; }
        }

        public TezInventoryViewSlot this[int index]
        {
            get { return mSlots[index]; }
        }

        public override void close()
        {
            foreach (var item in mSlots)
            {
                item.close();
            }
            mSlots.close();

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
                var index = mBeginPos + i;
                var view_slot = mSlots[i];
                if (index < this.filterManager.count)
                {
                    view_slot.itemSlot = this.filterManager[index].itemSlot;
                }
                else
                {
                    view_slot.itemSlot = null;
                }

                onSlotRefresh?.Invoke(view_slot);
            }
        }

        protected override void onItemChanged(TezInventoryDataSlot dataSlot)
        {
            var slot_index = dataSlot.index;
            if ((slot_index >= mBeginPos) && (slot_index < mBeginPos + mSlots.capacity))
            {
                var view_slot = mSlots[slot_index - mBeginPos];
                view_slot.itemSlot = dataSlot.itemSlot;
                onSlotRefresh?.Invoke(view_slot);
            }
        }

        protected override void onFilterChanged(TezInventoryFilter filterManager)
        {
            this.paging(0);
        }
    }
}