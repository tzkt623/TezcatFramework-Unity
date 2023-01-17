using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.Test
{
    class MyItem : TezItemableObject
    {
        protected override TezItemableObject cloneThisObject(TezBaseItemInfo itemInfo)
        {
            var item = new MyItem();
            item.init(this);
            return item;
        }
    }

    class TestDatabase
    {
        public void run()
        {
            TezReader reader = null;
            var obj = new MyItem();
            obj.deserialize(reader);
            TezcatFramework.mainDB.registerItem(obj);
        }
    }
}