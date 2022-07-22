using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.Game.Inventory
{
    public interface ITezInventoryObject
        : ITezCategoryObject
        , ITezCloseable
    {
        TezCategory category { get; }
        TezInventoryEntry inventoryEntry { get; }
    }
}