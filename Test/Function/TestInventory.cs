using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Game.Inventory;

namespace tezcat.Framework.Test
{
    class HealthPotion : TezSharedObject
    {
        public int healthAdd;

        public override void deserialize(TezReader reader)
        {
            base.deserialize(reader);

            healthAdd = reader.readInt("HealthAdd");
        }
    }

    class Armor : TezUniqueObject
    {
        public int armorAdd;

        protected override void initWithTemplate(TezItemableObject template)
        {
            base.initWithTemplate(template);
            var armor = (Armor)template;
            armorAdd = armor.armorAdd;
        }

        protected override TezItemableObject copy()
        {
            return new Armor();
        }

        public override void deserialize(TezReader reader)
        {
            base.deserialize(reader);

            armorAdd = reader.readInt("ArmorAdd");
        }
    }

    class Character : TezGameObject
    {
        public int health;
        public int armor;
    }

    class TestInventory : TezBaseTest
    {
        TezInventory inventory = new TezInventory();

        public TestInventory() : base("Inventory")
        {
            var page_view = new TezInventoryPageView();
            page_view.setPageCapacity(50);
            inventory.setView(page_view);
        }

        public override void run()
        {
            var info = TezcatFramework.mainDB.getItem("SmallHealthPotion");
            var hpPotion = info.getObject<HealthPotion>();
            this.inventory.store(hpPotion, 25);

            info = TezcatFramework.mainDB.getItem("H355");
            var armor = info.getObject<Armor>();
            this.inventory.store(armor);

            this.inventory.take(armor);
            this.inventory.take(hpPotion, 15);
        }
    }
}
