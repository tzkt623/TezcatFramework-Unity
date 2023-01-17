using tezcat.Framework.Utility;

namespace tezcat.Framework.Game.Inventory
{
    public class TezInventoryFilterSlot : TezInventoryDataSlot
    {
        public int indexInFilter { get; set; } = -1;

        public override Category category => Category.Filter;

        TezInventoryItemSlot mItemSlot = null;
        public override TezInventoryItemSlot itemSlot => mItemSlot;

        public static TezInventoryFilterSlot create()
        {
            return TezSamplePool<TezInventoryFilterSlot>.instance.create();
        }

        public void bindItemSlot(TezInventoryItemSlot slot)
        {
            mItemSlot = slot;
        }

        public override void close()
        {
            base.close();
            this.indexInFilter = -1;
            mItemSlot = null;
            TezSamplePool<TezInventoryFilterSlot>.instance.recycle(this);
        }
    }
}