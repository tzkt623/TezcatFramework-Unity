using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 物品栏视图
    /// 此对象的生命周期不归物品栏管
    /// 因为可以在运行时替换视图结构
    /// </summary>
    public abstract class TezInventoryBaseView : ITezCloseable
    {
        protected TezLifeMonitorSlot mLifeMonitorSlot = null;

        protected TezInventoryFilter mFilterManager = new TezInventoryFilter();
        public TezInventoryFilter filterManager => mFilterManager;

        public TezInventoryBaseView()
        {
            mFilterManager.evtFilterChanged += this.onFilterChanged;
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

        /// <summary>
        /// try to get inventory instance,
        /// if it had been deleted,
        /// return false
        /// </summary>
        public bool tryGetInventory(out TezInventory inventory)
        {
            return mLifeMonitorSlot.tryGetObject(out inventory);
        }

        protected abstract void onFilterChanged();

        public virtual void close()
        {
            mFilterManager.evtFilterChanged -= this.onFilterChanged;
            mFilterManager.close();
            mFilterManager = null;

            mLifeMonitorSlot.close();
            mLifeMonitorSlot = null;
        }

        public abstract void updateViewSlotData(int index);
        public abstract void addViewSlotData(ITezInventoryViewSlotData data);
        public abstract void removeViewSlotData(ITezInventoryViewSlotData data);


        public virtual void debug() { }
    }
}