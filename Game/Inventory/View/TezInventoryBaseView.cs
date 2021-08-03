using UnityEngine;
using System.Collections;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game.Inventory
{
    public abstract class TezInventoryBaseView : ITezCloseable
    {
        public TezInventory inventory => m_InventoryRef.get();
        protected TezWeakRef<TezInventory> m_InventoryRef = null;

        TezInventoryFilter m_FilterManager = new TezInventoryFilter();
        public TezInventoryFilter filterManager => m_FilterManager;

        public TezInventoryBaseView()
        {
            m_FilterManager.onItemChanged += this.onItemChanged;
            m_FilterManager.onFilterChanged += this.onFilterChanged;
        }

        public virtual void setInventory(TezInventory inventory)
        {
            m_InventoryRef = inventory;
            m_FilterManager.setInventory(inventory);
        }

        protected virtual void onItemChanged(TezInventoryDataSlot dataSlot)
        {

        }

        protected virtual void onFilterChanged(TezInventoryFilter filterManager)
        {
            throw new System.NotImplementedException();
        }

        public virtual void close()
        {
            m_InventoryRef.close();
            m_InventoryRef = null;

            m_FilterManager.close();
            m_FilterManager = null;
        }
    }
}
