namespace tezcat.Framework.Game.Inventory
{
    public class TezInventoryViewSlot : TezInventoryBaseSlot
    {
        public TezInventoryItemSlot bindSlot { get; set; }

        public override void close()
        {
            base.close();
            this.bindSlot = null;
        }
    }
}