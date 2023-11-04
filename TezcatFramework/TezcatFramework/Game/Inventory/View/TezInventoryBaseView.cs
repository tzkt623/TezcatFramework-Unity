using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// 物品栏视图
    /// 生命周期不归物品栏管
    /// 因为可以在运输时替换视图结构
    /// </summary>
    public abstract class TezInventoryBaseView : ITezCloseable
    {
        public TezInventory inventory
        {
            get
            {
                mLifeMonitorSlot.tryGetObject<TezInventory>(out var result);
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

        public virtual void debug() { }

        public abstract void updateViewSlotData(int index);
        public abstract void addViewSlotData(ITezInventoryViewSlotData data);
        public abstract void removeViewSlotData(ITezInventoryViewSlotData data);
    }
}