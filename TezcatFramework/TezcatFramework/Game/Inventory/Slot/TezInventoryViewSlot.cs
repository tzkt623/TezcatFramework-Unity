namespace tezcat.Framework.Game.Inventory
{
    public class TezInventoryViewSlot : TezInventoryBaseSlot
    {
        public TezInventoryItemSlot itemSlot { get; set; }

        public override void close()
        {
            base.close();
            this.itemSlot = null;
        }

        public virtual void pick()
        {
            TezInventoryManager.selectSlot(this.itemSlot.inventory, this.itemSlot);
        }
    }
}