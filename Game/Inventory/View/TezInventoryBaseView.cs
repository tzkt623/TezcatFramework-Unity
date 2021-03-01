using UnityEngine;
using System.Collections;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game.Inventory
{
    public abstract class TezInventoryBaseView : ITezCloseable
    {
        public TezInventory inventory => m_InventoryRef.get();
        protected TezWeakRef<TezInventory> m_InventoryRef = null;


        public TezInventoryFilter filterManager { get; private set; } = new TezInventoryFilter();

        public TezInventoryBaseView()
        {
            this.filterManager.onItemChanged += this.onItemChanged;
            this.filterManager.onFilterChanged += this.onFilterChanged;
        }

        public virtual void setInventory(TezInventory inventory)
        {
            m_InventoryRef = inventory;
            this.filterManager.setInventory(inventory);
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

            this.filterManager.close();
            this.filterManager = null;
        }
    }
}
