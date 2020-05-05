using tezcat.Framework.Core;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Game.Inventory
{
    public interface ITezInventoryVisualSelector
    {
        void onSelect(TezGameObject game_object);
        void onComplete();
    }

    public class TezInventorySelector<Object>
        : ITezService
        where Object : TezGameObject, ITezInventoryItem
    {
        public bool isActive
        {
            get { return this.sourceSlot != null; }
        }

        public ITezInventory sourceInventory { get; private set; }
        public ITezInventory targetInventory { get; set; }

        public TezInventorySlot sourceSlot { get; private set; }

        ITezInventoryVisualSelector m_VisualSelector = null;

        public void setVisualSelector(ITezInventoryVisualSelector visual_selector)
        {
            m_VisualSelector = visual_selector;
        }

        public void setSlot(TezInventorySlot slot)
        {
            this.sourceSlot = slot;
            this.sourceInventory = slot.inventory;
            m_VisualSelector.onSelect(slot.item);
        }

        public void putToTarget(int count = 1)
        {
            ((TezInventory<Object>)this.targetInventory).add((Object)sourceSlot.item, count);
            ((TezInventory<Object>)this.sourceInventory).remove(sourceSlot.index, count);
        }

        public void removeFromSource(int count = 1)
        {
            ((TezInventory<Object>)this.sourceInventory).remove(sourceSlot.index, count);
        }

        public void complete()
        {
            m_VisualSelector.onComplete();
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