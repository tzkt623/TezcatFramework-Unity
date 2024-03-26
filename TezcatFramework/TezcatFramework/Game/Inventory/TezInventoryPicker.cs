using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 物品虚拟选择接口
    /// </summary>
    public interface ITezInventorySelectorIcon
    {
        void onSelect(TezItemObject item);
        void onComplete();
    }

    /// <summary>
    /// 物品选择器
    /// </summary>
    public class TezInventoryPicker
    {
        public bool isActive
        {
            get { return this.sourceSlot != null; }
        }

        public TezInventory sourceInventory { get; private set; }
        public TezInventory targetInventory { get; set; }

        public ITezInventoryViewSlotData sourceSlot { get; private set; }

        ITezInventorySelectorIcon mSelectorIcon = null;

        public void setVisualSelector(ITezInventorySelectorIcon visualSelector)
        {
            mSelectorIcon = visualSelector;
        }

        public void pickSlot(ITezInventoryViewSlotData slot)
        {
            this.sourceSlot = slot;
            this.sourceInventory = slot.inventory;
            mSelectorIcon.onSelect(slot.item);
        }

        public void putToTarget(int count = 1)
        {
            this.targetInventory.store(sourceSlot.item, count);
            //this.sourceInventory.take(sourceSlot.index, count);
        }

        public void removeFromSource(int count = 1)
        {
            //this.sourceInventory.take(sourceSlot.index, count);
        }

        public void complete()
        {
            mSelectorIcon.onComplete();
            this.sourceInventory = null;
            this.targetInventory = null;
            this.sourceSlot = null;
        }

        public virtual void close()
        {
            this.sourceInventory = null;
            this.targetInventory = null;

            this.sourceSlot = null;
        }
    }
}