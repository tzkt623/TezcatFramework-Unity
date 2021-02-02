using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// 背包系统接口
    /// </summary>
    public interface ITezInventory : ITezRefObject
    {
        /// <summary>
        /// 过滤器
        /// </summary>
        TezInventoryFilter filter { get; }

        /// <summary>
        /// 物品数量
        /// </summary>
        int count { get; }


        TezInventoryItemSlot this[int index] { get; }

        /// <summary>
        /// 用于过滤器的函数
        /// 没事不要搞他玩
        /// </summary>
        void swapSlots(List<TezInventoryItemSlot> slots);
    }
}