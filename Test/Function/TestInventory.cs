using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Game.Inventory;

namespace tezcat.Framework.Test
{
    class MyGameItem : TezDatabaseGameItem
    {

    }

    class MyPotion
        : TezNonCopyableObject
    {
        protected override TezItemableObject cloneThisObject(TezBaseItemInfo itemInfo)
        {
            var temp = new MyShip();
            temp.init(this);
            return temp;
        }
    }

    class MyShip : TezCopyableObject
    {
        protected override TezItemableObject cloneThisObject(TezBaseItemInfo itemInfo)
        {
            var temp = new MyShip();
            temp.init(this);
            return temp;
        }
    }


    class TestInventory : TezBaseTest
    {
        TezInventory inventory = new TezInventory();

        public TestInventory() : base("Inventory")
        {

        }

        public override void run()
        {
            var obj1 = new MyShip();
            obj1.deserialize(TezcatFramework.fileDB.getItemData("Frigate001"));
            this.inventory.store(obj1);
        }
    }
}
