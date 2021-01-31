using System;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    public interface ITezAStarNode
        : ITezCloseable
    {
        ITezAStarNode parent { get; set; }

        int gCost { get; set; }

        int hCost { get; set; }

        int fCost { get; }

        bool isBlocked();
    }

    public interface ITezAStarNode<T>
        : ITezAStarNode
        , ITezBinaryHeapItem<T>
        , IEquatable<T>
        where T : ITezAStarNode
    {

    }
}

