using tezcat.Framework.Utility;

namespace tezcat.Framework.Game.Inventory
{
    public class TezInventoryFilterSlot : TezInventoryDataSlot
    {
        public int indexInFilter { get; set; } = -1;

        public override Category category => Category.Filter;

        TezInventoryItemSlot m_ItemSlot = null;
        public override TezInventoryItemSlot itemSlot => m_ItemSlot;

        public static TezInventoryFilterSlot create()
        {
            return TezSamplePool<TezInventoryFilterSlot>.instance.create();
        }

        public void bindItemSlot(TezInventoryItemSlot slot)
        {
            m_ItemSlot = slot;
        }

        public override void close()
        {
            base.close();
            this.indexInFilter = -1;
            m_ItemSlot = null;
            TezSamplePool<TezInventoryFilterSlot>.instance.recycle(this);
        }
    }
}