using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game.Inventory
{
    public abstract class TezInventoryBaseView : ITezCloseable
    {
        public TezInventory inventory
        {
            get
            {
                mLifeMonitorSlot.tryUse<TezInventory>(out var result);
                return result;
            }
        }
        protected TezLifeMonitorSlot mLifeMonitorSlot = null;

        protected TezInventoryFilter mFilterManager = new TezInventoryFilter();
        public TezInventoryFilter filterManager => mFilterManager;

        public TezInventoryBaseView()
        {
            mFilterManager.onFilterChanged += this.onFilterChanged;
        }

        public virtual void setInventory(TezInventory inventory)
        {
            mLifeMonitorSlot?.close();
            mLifeMonitorSlot = new TezLifeMonitorSlot(inventory);
        }

        public virtual void setInventory(TezInventory inventory, List<TezInventoryUniqueItemInfo> uniqueList, Dictionary<long, TezInventoryStackedItemInfo> stackedDict)
        {
            mLifeMonitorSlot?.close();
            mLifeMonitorSlot = new TezLifeMonitorSlot(inventory);
        }

        protected abstract void onFilterChanged();

        public virtual void close()
        {
            mFilterManager.onFilterChanged -= this.onFilterChanged;
            mFilterManager.close();
            mFilterManager = null;

            mLifeMonitorSlot.close();
            mLifeMonitorSlot = null;
        }

        public abstract void updateViewSlotData(int index);
        public abstract void addViewSlotData(ITezInventoryViewSlotData data);
        public abstract void removeViewSlotData(ITezInventoryViewSlotData data);
    }
}