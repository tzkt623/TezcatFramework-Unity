using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.Game.Inventory
{
    [System.Obsolete("此类没用")]
    public class TezStorableInfo : ITezCloseable
    {
        ITezItemableObject mStoreItem = null;
        TezCategory mCategory = null;
        TezItemID mItemID = null;
        int mStackCount = 0;

        public ITezItemableObject source => mStoreItem;
        public TezCategory category => mCategory;
        public int stackCount => mStackCount;

        public TezStorableInfo(ITezItemableObject owner, int stackCount, TezCategory category)
        {
            mStoreItem = owner;
            mStackCount = stackCount;
            mCategory = category;
        }

        public TezStorableInfo(ITezItemableObject owner) : this(owner, 0, owner.category)
        {

        }

        public TezStorableInfo(ITezItemableObject owner, int stackCount) : this(owner, stackCount, owner.category)
        {

        }

        public bool templateAs(TezStorableInfo other)
        {
            return mCategory.sameAs(other.mCategory);
        }

        public bool sameAs(TezStorableInfo entry)
        {
            return mItemID.sameAs(entry.mItemID);
        }

        public void recycle()
        {
            mStoreItem.close();
            mStoreItem = null;
            mCategory = null;
        }

        public void close()
        {
            mCategory = null;
            mStoreItem = null;
        }
    }
}