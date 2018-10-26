using tezcat.Framework.DataBase;

namespace tezcat.Framework.Core
{
    public abstract class TezItemSlot : TezSlot
    {
        public TezDataBaseItem myItem { get; set; } = null;

        public bool theSameAs(TezDataBaseItem item)
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