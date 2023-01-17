using tezcat.Framework.Core;

namespace tezcat.Framework.Game.Inventory
{
    public abstract class TezInventoryBaseView : ITezCloseable
    {
        public TezInventory inventory => mInventoryRef.get();
        protected TezFlagableRef<TezInventory> mInventoryRef = null;

        TezInventoryFilter mFilterManager = new TezInventoryFilter();
        public TezInventoryFilter filterManager => mFilterManager;

        public TezInventoryBaseView()
        {
            mFilterManager.onItemChanged += this.onItemChanged;
            mFilterManager.onFilterChanged += this.onFilterChanged;
        }

        public virtual void setInventory(TezInventory inventory)
        {
            mInventoryRef = new TezFlagableRef<TezInventory>(inventory);
            mFilterManager.setInventory(inventory);
        }

        protected abstract void onItemChanged(TezInventoryDataSlot dataSlot);
        protected abstract void onFilterChanged(TezInventoryFilter filterManager);

        public virtual void close()
        {
            mInventoryRef.close();
            mInventoryRef = null;

            mFilterManager.close();
            mFilterManager = null;
        }
    }
}
