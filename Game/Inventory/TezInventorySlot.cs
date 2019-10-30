using tezcat.Framework.Core;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Game.Inventory
{
    public class TezInventorySlot
        : ITezCloseable
    {
        public bool isBound { get; set; } = false;
        public int slotIndex { get; set; } = -1;

        public int count { get; set; } = -1;
        public TezGameObject item { get; set; } = null;

        public T getItem<T>() where T : TezGameObject
        {
            return (T)this.item;
        }

        public ITezInventory owner { get; private set; } = null;

        public TezInventorySlot(ITezInventory inventory)
        {
            this.owner = inventory;
        }

        public virtual void close()
        {
            this.item = null;
            this.owner = null;
        }
    }
}