using System;
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
        TezInventory inventory = null;
        TezInventoryPageView pageView = null;

        public TestInventory() : base("Inventory")
        {

        }

        private void onSlotRefresh(TezInventoryViewSlot slot, int index)
        {
            if (slot.data == null)
            {
                Console.WriteLine($"Slot Refresh[{index}]>> empty");
            }
            else
            {
                Console.WriteLine($"Slot Refresh[{index}]>> {slot.data.item.itemInfo.NID}({slot.count})");
            }
        }

        private void onPageChanged(int current, int max)
        {
            Console.WriteLine($"{current}/{max}");
        }

        public override void run()
        {
            var info = TezcatFramework.mainDB.getItem("SmallHealthPotion");
            var hpPotion = info.getObject<HealthPotion>();
            this.inventory.store(hpPotion, 25);

            info = TezcatFramework.mainDB.getItem("H355");
            var armor = info.getObject<Armor>();
            this.inventory.store(armor);
            this.pageView.debug();

            this.pageView.filterManager.setFilter("TypeFilter");
            this.pageView.filterManager.setFilter("NameFilter");

            this.inventory.take(armor);
            this.inventory.take(hpPotion, 10);

            this.pageView.filterManager.setFilter("Default");
        }

        public override void init()
        {
            TezInventoryFilter.createFilter("NameFilter", (ITezInventoryViewSlotData data) =>
            {
                return data.item.itemInfo.NID == "H355";
            });

            TezInventoryFilter.createFilter("TypeFilter", (ITezInventoryViewSlotData data) =>
            {
                return data.item.itemInfo.category == TezCategorySystem.getCategory(MyCategory.Potion.HealthPotion);
            });

            this.pageView = new TezInventoryPageView();

            pageView.setPageCapacity(5);
            pageView.onPageChanged += onPageChanged;
            pageView.onSlotRefresh += onSlotRefresh;

            this.inventory = new TezInventory();
            inventory.setView(pageView);
        }

        public override void close()
        {
            this.inventory.close();
            this.pageView.close();
        }
    }
}