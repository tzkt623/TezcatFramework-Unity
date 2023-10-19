using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game.Inventory
{
    public abstract class TezInventoryBaseView : ITezCloseable
    {
        public TezInventory inventory => mInventoryRef.get();
        protected TezFlagableRef<TezInventory> mInventoryRef = null;

        protected TezInventoryFilter mFilterManager = new TezInventoryFilter();
        public TezInventoryFilter filterManager => mFilterManager;

        public TezInventoryBaseView()
        {
            mFilterManager.onFilterChanged += this.onFilterChanged;
        }

        public virtual void setInventory(TezInventory inventory)
        {
            mInventoryRef = new TezFlagableRef<TezInventory>(inventory);
        }

        public virtual void setInventory(TezInventory inventory, List<TezInventoryUniqueItemInfo> uniqueList, Dictionary<long, TezInventoryStackedItemInfo> stackedDict)
        {
            mInventoryRef = new TezFlagableRef<TezInventory>(inventory);
        }

        protected abstract void onFilterChanged();

        public virtual void close()
        {
            mFilterManager.onFilterChanged -= this.onFilterChanged;
            mFilterManager.close();
            mFilterManager = null;

            mInventoryRef.close();
            mInventoryRef = null;
        }

        public abstract void updateViewSlotData(int index);
        public abstract void addViewSlotData(ITezInventoryViewSlotData data);
        public abstract void removeViewSlotData(ITezInventoryViewSlotData data);
    }
}