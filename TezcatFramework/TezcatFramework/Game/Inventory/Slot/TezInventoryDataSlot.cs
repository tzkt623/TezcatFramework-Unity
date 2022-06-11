namespace tezcat.Framework.Game.Inventory
{
    public abstract class TezInventoryDataSlot : TezInventoryBaseSlot
    {
        public enum Category
        {
            Item,
            Filter
        }

        public abstract Category category { get; }
        public abstract TezInventoryItemSlot itemSlot { get; }
    }
}