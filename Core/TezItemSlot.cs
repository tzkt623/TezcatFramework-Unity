using tezcat.Framework.Database;

namespace tezcat.Framework.Core
{
    public abstract class TezItemSlot : TezSlot
    {
        public TezDatabaseItem myItem { get; set; } = null;

        public bool theSameAs(TezDatabaseItem item)
        {
            if (myItem == null)
            {
                return false;
            }

            return myItem == item;
        }

        public override void close()
        {
            this.myItem = null;
        }
    }
}