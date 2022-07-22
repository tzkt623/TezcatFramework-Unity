using tezcat.Framework.Database;
using tezcat.Framework.Game.Inventory;

namespace tezcat.Framework.Test
{
    class MyItemableObject
        : ITezInventoryObject
        , ITezDBItemObject
    {
        public TezInventoryEntry inventoryEntry { get; set; }
        public TezCategory category { get; set; }
        public int dbUID { get; private set; } = -1;

        public void close()
        {
            this.inventoryEntry.close();

            this.category = null;
            this.inventoryEntry = null;
        }

        public bool compare(ITezCategoryObject other)
        {
            return this.category == other.category;
        }

        public bool compare(ITezDBItemObject other)
        {
            if (this.dbUID < 0 || other.dbUID < 0)
            {
                return false;
            }

            return this.dbUID == other.dbUID;
        }

        public void initWithData(ITezSerializableItem item)
        {
            var game_item = (TezDatabaseGameItem)item;
            this.dbUID = game_item.dbUID;
            this.category = game_item.category;
            this.inventoryEntry = new TezInventoryEntry(this, game_item.stackCount);
        }
    }


    class TestInventory
    {
        TezInventory inventory = new TezInventory();

        public void test()
        {
            var obj1 = new MyItemableObject();
            this.inventory.store(obj1);
        }

    }
}
