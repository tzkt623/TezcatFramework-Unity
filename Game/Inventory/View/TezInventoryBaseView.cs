using UnityEngine;
using System.Collections;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game.Inventory
{
    public abstract class TezInventoryBaseView : ITezCloseable
    {
        public TezInventory inventory => m_InventoryRef.get();
        protected TezWeakRef<TezInventory> m_InventoryRef = null;


        public TezInventoryFilterManager filterManager { get; private set; } = new TezInventoryFilterManager();

        public virtual void setInventory(TezInventory inventory)
        {
            m_InventoryRef = inventory;
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
