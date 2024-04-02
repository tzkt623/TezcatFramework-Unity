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
        protected TezLifeHolder mLifeHolder = new TezLifeHolder();

        protected TezInventoryFilter mFilterManager = new TezInventoryFilter();
        public TezInventoryFilter filterManager => mFilterManager;

        public TezInventoryBaseView()
        {
            mFilterManager.evtFilterChanged += this.onFilterChanged;
        }

        public virtual void setInventory(TezInventory inventory)
        {
            mLifeHolder.create(inventory);
        }

        public virtual void setInventory(TezInventory inventory, List<TezInventoryUniqueItemInfo> uniqueList, Dictionary<long, TezInventoryStackedItemInfo> stackedDict)
        {
            mLifeHolder.create(inventory);
        }

        /// <summary>
        /// try to get inventory instance,
        /// if it had been deleted,
        /// return false
        /// </summary>
        public bool tryGetInventory(out TezInventory inventory)
        {
            return mLifeHolder.tryGetObject(out inventory);
        }

        protected abstract void onFilterChanged();

        void ITezCloseable.deleteThis()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {
            mFilterManager.evtFilterChanged -= this.onFilterChanged;
            mFilterManager.close();
            mFilterManager = null;

            mLifeHolder.close();
            mLifeHolder = null;
        }

        public abstract void updateViewSlotData(int index);
        public abstract void addViewSlotData(ITezInventoryViewSlotData data);
        public abstract void removeViewSlotData(ITezInventoryViewSlotData data);


        public virtual void debug() { }
    }
}