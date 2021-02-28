using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.ECS;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game.Inventory
{
    public abstract class TezInventoryBaseSlot : ITezCloseable
    {
        public int index { get; set; }

        public virtual void close()
        {
            this.index = -1;
        }
    }

    public abstract class TezInventoryDataSlot : TezInventoryBaseSlot
    {
        public abstract TezInventoryItemSlot itemSlot { get; }
    }

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


        public override TezInventoryItemSlot itemSlot => this;

        /// <summary>
        /// 格子属于的Inventory
        /// </summary>
        public TezInventory inventory { get; private set; } = null;

        /// <summary>
        /// 装的Item
        /// </summary>
        public TezComData item { get; set; } = null;

        /// <summary>
        /// Item的数量
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
            this.inventory = inventory;
        }

        public override void close()
        {
            base.close();
            this.item = null;
            this.inventory = null;
        }

    }

    public class TezInventoryFilterSlot : TezInventoryDataSlot
    {
        TezInventoryItemSlot m_ItemSlot = null;
        public override TezInventoryItemSlot itemSlot => m_ItemSlot;

        public static TezInventoryFilterSlot create()
        {
            return TezSamplePool<TezInventoryFilterSlot>.instance.create();
        }

        public void bindItemSlot(TezInventoryItemSlot slot)
        {
            m_ItemSlot = slot;
        }

        public override void close()
        {
            base.close();
            m_ItemSlot = null;
            TezSamplePool<TezInventoryFilterSlot>.instance.recycle(this);
        }
    }

    public class TezInventoryViewSlot : TezInventoryBaseSlot
    {
        public TezInventoryItemSlot bindSlot { get; set; }

        public override void close()
        {
            base.close();
            this.bindSlot = null;
        }
    }
}