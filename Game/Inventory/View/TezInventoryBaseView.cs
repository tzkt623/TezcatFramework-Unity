using UnityEngine;
using System.Collections;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game.Inventory
{
    public abstract class TezInventoryBaseView : ITezCloseable
    {
        public ITezInventory inventory => m_InventoryRef.get();
        protected TezWeakRef<ITezInventory> m_InventoryRef = null;

        public virtual void setInventory(ITezInventory inventory)
        {
            m_InventoryRef = new TezWeakRef<ITezInventory>(inventory);
        }

        public virtual void close()
        {
            m_InventoryRef.close();
            m_InventoryRef = null;
        }
    }
}
