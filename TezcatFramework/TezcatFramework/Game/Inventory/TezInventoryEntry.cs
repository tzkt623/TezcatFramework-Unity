using tezcat.Framework.Core;

namespace tezcat.Framework.Game.Inventory
{
    public class TezInventoryEntry : ITezCloseable
    {
        ITezInventoryObject m_Owner = null;
        int m_StackCount = 0;

        public ITezInventoryObject owner => m_Owner;
        public int stackCount => m_StackCount;

        public TezInventoryEntry(ITezInventoryObject owner)
        {
            m_Owner = owner;
        }
        public TezInventoryEntry(ITezInventoryObject owner, int stackCount)
        {
            m_Owner = owner;
            m_StackCount = stackCount;
        }

        public bool templateAs(TezInventoryEntry other)
        {
            return this.owner.category == other.owner.category;
        }

        public void recycle()
        {
            m_Owner.close();
            m_Owner = null;
        }

        public void close()
        {
            m_Owner = null;
        }
    }
}