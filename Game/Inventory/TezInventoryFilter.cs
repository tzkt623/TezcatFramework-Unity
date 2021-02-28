using UnityEngine;
using System.Collections;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using System.Collections.Generic;
using System;
using tezcat.Framework.ECS;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// 物品栏过滤器
    /// </summary>
    public class TezInventoryFilter : ITezCloseable
    {
        /// <summary>
        /// 当前Filter规则发生变化
        /// 通知整个物品栏刷新
        /// </summary>
	    public event TezEventExtension.Action<ITezInventory> onRuleChangedRefresh;

        /// <summary>
        /// 当前符合Filter规则的槽位发生变化
        /// 通知此槽位刷新
        /// </summary>
        public event TezEventExtension.Action<TezInventoryItemSlot> onItemChangedRefresh;

        List<TezInventoryItemSlot> m_CurrentSlots = null;
        List<TezInventoryItemSlot> m_BackupSlots = new List<TezInventoryItemSlot>();
        List<TezInventoryItemSlot> m_HiddenSlots = new List<TezInventoryItemSlot>();

        TezInventory m_Inventory = null;

        const byte Mask_Null = 0;
        const byte Mask_Contain = 1;
        const byte Mask_NotContain = 2;
        byte m_FilterMask = 0;
        TezEventExtension.Function<bool, TezComData> m_CurrentFilter = null;

        public TezInventoryFilter(TezInventory inventory, List<TezInventoryItemSlot> slots)
        {
            m_Inventory = inventory;
            m_CurrentSlots = slots;
        }

        public void add(TezComData gameObject, int count)
        {
            switch (m_FilterMask)
            {
                case Mask_Null:
                    {
                        var result_slot = TezInventoryHelper.add(gameObject, count, m_CurrentSlots, m_Inventory);
                        onItemChangedRefresh?.Invoke(result_slot);
                        break;
                    }
                case Mask_Contain:
                    ///成功的被选中
                    if (m_CurrentFilter(gameObject))
                    {
                        var result_slot = TezInventoryHelper.add(gameObject, count, m_CurrentSlots, m_Inventory);
                        onItemChangedRefresh?.Invoke(result_slot);
                    }
                    else
                    {
                        TezInventoryHelper.add(gameObject, count, m_HiddenSlots, m_Inventory);
                    }
                    break;
                case Mask_NotContain:
                    ///成功的被隐藏
                    if (m_CurrentFilter(gameObject))
                    {
                        TezInventoryHelper.add(gameObject, count, m_HiddenSlots, m_Inventory);
                    }
                    else
                    {
                        var result_slot = TezInventoryHelper.add(gameObject, count, m_CurrentSlots, m_Inventory);
                        onItemChangedRefresh?.Invoke(result_slot);
                    }
                    break;
                default:
                    throw new Exception("notifyItemChanged");
            }
        }

        public bool remove(TezComData gameObject, int count)
        {
            TezInventoryItemSlot result_slot = null;
            bool flag = false;
            switch (m_FilterMask)
            {
                case Mask_Null:
                    {
                        flag = TezInventoryHelper.remove(gameObject, count, m_CurrentSlots, out result_slot);
                        if (flag)
                        {
                            onItemChangedRefresh?.Invoke(result_slot);
                        }
                    }
                    break;
                case Mask_Contain:
                    ///成功的被选中
                    if (m_CurrentFilter(gameObject))
                    {
                        flag = TezInventoryHelper.remove(gameObject, count, m_CurrentSlots, out result_slot);
                        if (flag)
                        {
                            onItemChangedRefresh?.Invoke(result_slot);
                        }
                    }
                    else
                    {
                        flag = TezInventoryHelper.remove(gameObject, count, m_HiddenSlots, out result_slot);
                    }
                    break;
                case Mask_NotContain:
                    ///成功的被隐藏
                    if (m_CurrentFilter(gameObject))
                    {
                        flag = TezInventoryHelper.remove(gameObject, count, m_HiddenSlots, out result_slot);
                    }
                    else
                    {
                        flag = TezInventoryHelper.remove(gameObject, count, m_CurrentSlots, out result_slot);
                        if (flag)
                        {
                            onItemChangedRefresh?.Invoke(result_slot);
                        }
                    }
                    break;
                default:
                    throw new Exception("notifyItemChanged");
            }

            return flag;
        }

        /// <summary>
        /// 通知Item发生变化
        /// </summary>
        public void notifyItemChanged(TezInventoryItemSlot inventorySlot)
        {
            onItemChangedRefresh?.Invoke(inventorySlot);
        }

        /// <summary>
        /// 取消过滤
        /// </summary>
        public void cancelFilter()
        {
            m_FilterMask = Mask_Null;
            m_CurrentFilter = null;

            for (int i = 0; i < m_HiddenSlots.Count; i++)
            {
                var slot = m_HiddenSlots[i];
                slot.index = m_CurrentSlots.Count;
                m_CurrentSlots.Add(slot);
            }
            m_HiddenSlots.Clear();

//            onRuleChangedRefresh?.Invoke(m_Inventory);
        }

        /// <summary>
        /// 按照条件过滤物品
        /// 符合条件的被选中
        /// </summary>
        public void contain(TezEventExtension.Function<bool, TezComData> onFilter)
        {
            m_CurrentFilter = onFilter;
            m_FilterMask = Mask_Contain;
            this.reloadHide();

            for (int i = 0; i < m_CurrentSlots.Count; i++)
            {
                var slot = m_CurrentSlots[i];
                if (m_CurrentFilter(slot.item))
                {
                    slot.index = m_BackupSlots.Count;
                    m_BackupSlots.Add(slot);
                }
                else
                {
                    slot.index = -1;
                    m_HiddenSlots.Add(slot);
                }
            }

            m_CurrentSlots.Clear();
 //           m_Inventory.swapSlots(m_BackupSlots);
            this.swap();

//            onRuleChangedRefresh?.Invoke(m_Inventory);
        }

        /// <summary>
        /// 按条件过滤物品
        /// 符合条件的的被排除
        /// </summary>
        public void notContain(TezEventExtension.Function<bool, TezComData> onFilter)
        {
            m_CurrentFilter = onFilter;
            m_FilterMask = Mask_NotContain;
            this.reloadHide();

            for (int i = 0; i < m_CurrentSlots.Count; i++)
            {
                var slot = m_CurrentSlots[i];
                if (m_CurrentFilter(slot.item))
                {
                    slot.index = -1;
                    m_HiddenSlots.Add(slot);
                }
                else
                {
                    slot.index = m_BackupSlots.Count;
                    m_BackupSlots.Add(slot);
                }
            }

            m_CurrentSlots.Clear();
 //           m_Inventory.swapSlots(m_BackupSlots);
            this.swap();

//            onRuleChangedRefresh?.Invoke(m_Inventory);
        }

        /// <summary>
        /// 按自定义规则排序
        /// </summary>
        public void sortBy(Comparison<TezInventoryItemSlot> onSort)
        {
            this.reloadHide();

            m_CurrentSlots.Sort(onSort);
            for (int i = 0; i < m_CurrentSlots.Count; i++)
            {
                m_CurrentSlots[i].index = i;
            }
//            onRuleChangedRefresh?.Invoke(m_Inventory);
        }

        /// <summary>
        /// 整理空格子
        /// </summary>
        public void clearupEmptySlot()
        {
            for (int i = 0; i < m_CurrentSlots.Count; i++)
            {
                var slot = m_CurrentSlots[i];
                if (slot.item != null)
                {
                    slot.index = m_BackupSlots.Count;
                    m_BackupSlots.Add(slot);
                }
            }

            m_CurrentSlots.Clear();
//            m_Inventory.swapSlots(m_BackupSlots);
            this.swap();

//            onRuleChangedRefresh?.Invoke(m_Inventory);
        }

        /// <summary>
        /// 交换备用槽
        /// </summary>
        private void swap()
        {
            var temp = m_CurrentSlots;
            m_CurrentSlots = m_BackupSlots;
            m_BackupSlots = temp;
        }

        private void reloadHide()
        {
            if (m_HiddenSlots.Count > 0)
            {
                m_CurrentSlots.AddRange(m_HiddenSlots);
                m_HiddenSlots.Clear();
            }
        }

        public void clearEvent()
        {
            onRuleChangedRefresh = null;
        }

        public virtual void close()
        {
            foreach (var item in m_HiddenSlots)
            {
                item.close();
            }

            m_HiddenSlots.Clear();
            m_BackupSlots.Clear();

            m_HiddenSlots = null;
            m_BackupSlots = null;
            m_Inventory = null;
            m_CurrentSlots = null;
            onRuleChangedRefresh = null;
            onItemChangedRefresh = null;
        }
    }
}