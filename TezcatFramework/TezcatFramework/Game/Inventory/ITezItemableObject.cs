using System;
using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// 可储存对象
    /// </summary>
    [Obsolete("此接口没用")]
    public interface ITezItemableObject
        : ITezCategoryObject
        , ITezCloseable
    {
        /// <summary>
        /// 物品ID
        /// </summary>
        TezItemID itemID { get; }

        /// <summary>
        /// 物品信息
        /// </summary>
        TezItemInfo itemInfo { get; }
    }
}