using tezcat.DataBase;

namespace tezcat.Core
{
    public abstract class TezItemSlot : TezSlot
    {
        public TezItem myItem { get; set; } = null;

        public bool theSameAs(TezItem item)
        {
            if (myItem == null)
            {
                return false;
            }

            return item.GUID == item.GUID;
        }

        public override void close()
        {
            this.myItem = null;
        }
    }
}