using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// InventorySlot
    /// </summary>
    public class TezInventoryItemSlot : ITezCloseable
    {
        /// <summary>
        /// 初始IndexID
        /// </summary>
        public const int InitIndex = -1;
        /// <summary>
        /// 被隐藏后IndexID
        /// </summary>
        public const int HideIndex = -2;
        /// <summary>
        /// 临时槽位ID
        /// </summary>
        public const int TempIndex = -3;

        /// <summary>
        /// 格子属于的Inventory
        /// </summary>
        public ITezInventory inventory { get; private set; } = null;

        /// <summary>
        /// 格子Index
        /// </summary>
        public int index { get; set; } = InitIndex;

        /// <summary>
        /// 装的Item
        /// </summary>
        public TezGameObject item { get; set; } = null;

        /// <summary>
        /// Item的数量
        /// </summary>
        public int count { get; set; } = -1;

        /// <summary>
        /// 转换Item
        /// 转换失败返回Null
        public T getItem<T>() where T : TezGameObject
        {
            return this.item as T;
        }

        public TezInventoryItemSlot(ITezInventory inventory)
        {
            this.inventory = inventory;
        }

        public virtual void close()
        {
            this.item = null;
            this.inventory = null;
        }
    }
}