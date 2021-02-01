using System;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    public interface ITezAStarDataWrapper
        : ITezCloseable
        , ITezBinaryHeapItem
    {
        ITezAStarDataWrapper parent { get; set; }

        object data { get; }

        int gCost { get; set; }

        int hCost { get; set; }

        int fCost { get; }

        bool isBlocked();
    }

    public interface ITezAStarDataWrapper<Self, BlockData>
        : ITezAStarDataWrapper
        , ITezBinaryHeapItem<Self>
        , IEquatable<Self>
        where Self : ITezAStarDataWrapper
    {
        BlockData blockData { get; set; }
    }
}

