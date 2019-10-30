using tezcat.Framework.Core;

namespace tezcat.Framework.Game.Inventory
{
    public interface ITezInventoryItem
        : ITezCloseable
    {
        TezInventoryItemConfig itemConfig { get; }
    }
}