using System;
using System.Collections.Generic;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// InventorySlot
    /// </summary>
    public class TezInventoryItemSlot : TezInventoryDataSlot
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

        public override Category category => Category.Item;

        public override TezInventoryItemSlot itemSlot => this;


        TezInventory m_Inventory = null;
        /// <summary>
        /// 格子属于的Inventory
        /// </summary>
        public TezInventory inventory => m_Inventory;

        /// <summary>
        /// 装的Item
        /// </summary>
        public TezComData item { get; set; } = null;

        /// <summary>
        /// Item的数量
        /// 为-1表示不可堆叠
        /// </summary>
        public int count { get; set; } = -1;

        /// <summary>
        /// 转换Item
        /// 转换失败返回Null
        public T getItem<T>() where T : TezComData
        {
            return this.item as T;
        }

        public TezInventoryItemSlot(TezInventory inventory)
        {
            m_Inventory = inventory;
        }

        public override void close()
        {
            base.close();
            this.item = null;
            m_Inventory = null;
        }

        /// <summary>
        /// 拿出
        /// </summary>
        public TezComData take()
        {
            return m_Inventory.take(this.index);
        }
    }
}