using System;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    class TestInventory : TezBaseTest
    {
        TezInventory mInventory = null;
        TezInventoryPageView mPageView = null;

        public TestInventory() : base("Inventory")
        {

        }

        public override void init()
        {
            TezInventoryFilter.createFilter("NameFilter", (ITezInventoryViewSlotData data) =>
            {
                return data.item.itemInfo.NID == "H355";
            });

            TezInventoryFilter.createFilter("TypeFilter", (ITezInventoryViewSlotData data) =>
            {
                return data.item.itemInfo.itemID.TID == TezcatFramework.protoDB.getTypeID("HealthPotion");
            });

//             TezInventoryFilter.createFilter("CategoryFilter", (ITezInventoryViewSlotData data) =>
//             {
//                 return data.item.itemInfo.category == TezCategorySystem.getCategory(MyCategory.Potion.HealthPotion);
//             });

            mPageView = new TezInventoryPageView();

            mPageView.setPageCapacity(5);
            mPageView.evtPageChanged += onPageChanged;
            mPageView.evtSlotRefresh += onSlotRefresh;

            mInventory = new TezInventory();
            mInventory.setView(mPageView);
        }

        protected override void onClose()
        {
            mInventory.close();
            mPageView.close();
        }

        public override void run()
        {
            //var proto = TezcatFramework.protoDB.createObject<HealthPotion>("SmallHealthPotion");
            var hpPotion = TezcatFramework.protoDB.createObject<HealthPotion>("SmallHealthPotion");
            mInventory.store(hpPotion, 25);

            //proto = TezcatFramework.protoDB.createObject<Breastplate>("H355");
            var armor = TezcatFramework.protoDB.createObject<Breastplate>("H355");
            mInventory.store(armor);
            mPageView.debug();

            mPageView.filterManager.setFilter("TypeFilter");
            mPageView.filterManager.setFilter("NameFilter");

            mInventory.take(armor);
            mInventory.take(hpPotion, 10);

            mPageView.filterManager.setFilter("Default");
        }

        private void onSlotRefresh(TezInventoryViewSlot slot, int index)
        {
            if (slot.data == null)
            {
                Console.WriteLine($"{slot.index}:[x]");
            }
            else
            {
                Console.WriteLine($"{slot.index}:{slot.data.item.itemInfo.NID}({slot.count})");
            }
        }

        private void onPageChanged(int current, int max)
        {
            Console.WriteLine($"{current}/{max}");
        }
    }
}