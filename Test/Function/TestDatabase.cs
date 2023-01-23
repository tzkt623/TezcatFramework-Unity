using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.Test
{
    class TestDatabase
    {
        public void run()
        {
            TezReader reader = null;
            var obj = new MyPotion();
            obj.deserialize(reader);
            TezcatFramework.mainDB.registerItem(obj);
        }
    }
}