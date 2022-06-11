using tezcat.Framework.Core;

namespace tezcat.Framework.Game.Inventory
{
    public abstract class TezInventoryBaseSlot : ITezCloseable
    {
        public int index { get; set; }

        public virtual void close()
        {
            this.index = -1;
        }
    }
}