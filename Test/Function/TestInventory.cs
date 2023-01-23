using tezcat.Framework.Core;
using tezcat.Framework.Game.Inventory;

namespace tezcat.Framework.Test
{
    class MyPotion : TezReadOnlyObject
    {
        public int healthAdd;

        protected override TezItemableObject copyThisObject()
        {
            var copy = new MyPotion();
            copy.healthAdd = this.healthAdd;
            return copy;
        }
    }

    class MyShip : TezUniqueObject
    {
        protected override TezItemableObject copyThisObject()
        {
            throw new System.NotImplementedException();
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
            var info = TezcatFramework.fileDB.getItemInfo("Frigate001");
            var obj = info.getObject<MyShip>();
            this.inventory.store(obj);
        }
    }
}
