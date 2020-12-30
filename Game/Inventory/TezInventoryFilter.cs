using UnityEngine;
using System.Collections;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using System.Collections.Generic;
using System;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// 物品栏过滤器
    /// </summary>
    public class TezInventoryFilter : ITezCloseable
    {
        /// <summary>
        /// 刷新整个物品栏通知
        /// </summary>
	    public event TezEventExtension.Action<ITezInventory> onRefresh;

        List<TezInventorySlot> m_BackupSlots = new List<TezInventorySlot>();
        List<TezInventorySlot> m_HideSlots = new List<TezInventorySlot>();

        ITezInventory m_Inventory = null;
        List<TezInventorySlot> m_InventorySlots = null;

        public TezInventoryFilter(ITezInventory inventory, List<TezInventorySlot> slots)
        {
            m_Inventory = inventory;
            m_InventorySlots = slots;
        }

        /// <summary>
        /// 取消过滤
        /// </summary>
        public void cancelFilter()
        {
            for (int i = 0; i < m_HideSlots.Count; i++)
            {
                var slot = m_HideSlots[i];
                slot.index = m_InventorySlots.Count;
                m_InventorySlots.Add(slot);
            }

            m_HideSlots.Clear();

            onRefresh?.Invoke(m_Inventory);
        }

        /// <summary>
        /// 按照条件过滤物品
        /// 符合条件的被选中
        /// </summary>
        public void contain(TezEventExtension.Function<bool, TezInventorySlot> onFilter)
        {
            this.reloadHide();

            for (int i = 0; i < m_InventorySlots.Count; i++)
            {
                var slot = m_InventorySlots[i];
                if (onFilter(slot))
                {
                    slot.index = m_BackupSlots.Count;
                    m_BackupSlots.Add(slot);
                }
                else
                {
                    m_HideSlots.Add(slot);
                }
            }

            m_InventorySlots.Clear();
            m_Inventory.swapSlots(m_BackupSlots);

            this.swap();

            onRefresh?.Invoke(m_Inventory);
        }

        /// <summary>
        /// 按条件过滤物品
        /// 符合条件的的被排除
        /// </summary>
        public void notContain(TezEventExtension.Function<bool, TezInventorySlot> onFilter)
        {
            this.reloadHide();


            for (int i = 0; i < m_InventorySlots.Count; i++)
            {
                var slot = m_InventorySlots[i];
                if (onFilter(slot))
                {
                    m_HideSlots.Add(slot);
                }
                else
                {
                    slot.index = m_BackupSlots.Count;
                    m_BackupSlots.Add(slot);
                }
            }

            m_InventorySlots.Clear();
            m_Inventory.swapSlots(m_BackupSlots);

            this.swap();

            onRefresh?.Invoke(m_Inventory);
        }

        /// <summary>
        /// 按自定义规则排序
        /// </summary>
        public void sortBy(Comparison<TezInventorySlot> onSort)
        {
            this.reloadHide();

            m_InventorySlots.Sort(onSort);
            for (int i = 0; i < m_InventorySlots.Count; i++)
            {
                m_InventorySlots[i].index = i;
            }

            onRefresh?.Invoke(m_Inventory);
        }

        /// <summary>
        /// 整理空格子
        /// </summary>
        public void clearupEmptySlot()
        {
            for (int i = 0; i < m_InventorySlots.Count; i++)
            {
                var slot = m_InventorySlots[i];
                if (slot.item != null)
                {
                    slot.index = m_BackupSlots.Count;
                    m_BackupSlots.Add(slot);
                }
            }

            m_InventorySlots.Clear();
            m_Inventory.swapSlots(m_BackupSlots);
            this.swap();
            onRefresh?.Invoke(m_Inventory);
        }

        /// <summary>
        /// 交换备用槽
        /// </summary>
        private void swap()
        {
            var temp = m_InventorySlots;
            m_InventorySlots = m_BackupSlots;
            m_BackupSlots = temp;
        }

        private void reloadHide()
        {
            if (m_HideSlots.Count > 0)
            {
                m_InventorySlots.AddRange(m_HideSlots);
                m_HideSlots.Clear();
            }
        }

        public void clearEvent()
        {
            onRefresh = null;
        }

        public virtual void close()
        {
            foreach (var item in m_HideSlots)
            {
                item.close();
            }
            m_HideSlots.Clear();
            m_HideSlots = null;

            m_BackupSlots.Clear();
            m_BackupSlots = null;
            m_Inventory = null;
            m_InventorySlots = null;
            onRefresh = null;
        }
    }
}