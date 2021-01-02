﻿using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.ECS;
using tezcat.Framework.Extension;

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

    /// <summary>
    /// 背包系统
    /// </summary>
    public class TezInventory<Object>
        : TezRefObject
        , ITezInventory
        where Object : TezGameObject
    {
        /// <summary>
        /// 物品数量
        /// </summary>
        public int count
        {
            get { return m_Slots.Count; }
        }

        TezInventoryFilter m_Filter = null;
        /// <summary>
        /// 获得一个Filter
        /// 用于设置物品过滤规则
        /// </summary>
        public TezInventoryFilter filter
        {
            get
            {
                return m_Filter;
            }
        }

        List<TezInventoryItemSlot> m_Slots = new List<TezInventoryItemSlot>();

        public TezInventory()
        {
            m_Filter = new TezInventoryFilter(this, m_Slots);
        }

        /// <summary>
        /// 获得一个Slot
        /// </summary>
        public TezInventoryItemSlot this[int index]
        {
            get
            {
                return m_Slots[index];
            }
        }

        public void add(Object gameObject, int count)
        {
            m_Filter.add(gameObject, count);
        }

        /// <summary>
        /// 向当前Slot中添加
        /// </summary>
        public void add(int slotIndex, Object gameObject, int count)
        {
            var slot = m_Slots[slotIndex];
            slot.item = gameObject;
            slot.count += count;

            m_Filter.notifyItemChanged(slot);
        }

        /// <summary>
        /// 从当前Slot中移除
        /// </summary>
        public void remove(int slotIndex, int count)
        {
            var slot = m_Slots[slotIndex];
            slot.count -= count;
            if (slot.count == 0)
            {
                slot.item = null;
            }

            m_Filter.notifyItemChanged(slot);
        }

        public bool remove(Object gameObject, int count)
        {
            if(m_Filter.remove(gameObject, count))
            {
                return true;
            }

            return false;
        }

        public virtual void close()
        {
            base.close();

            for (int i = 0; i < m_Slots.Count; i++)
            {
                m_Slots[i].close();
            }
            m_Slots.Clear();
            m_Slots = null;

            m_Filter.close();
            m_Filter = null;
        }

        void ITezInventory.swapSlots(List<TezInventoryItemSlot> slots)
        {
            m_Slots = slots;
        }
    }

    public static class TezInventoryHelper
    {
        public static TezInventoryItemSlot add(TezGameObject gameObject, int count, List<TezInventoryItemSlot> slotList, ITezInventory inventory)
        {
            var stackable = gameObject.templateItem.stackCount > 0;

            int result_index = 0;
            TezInventoryItemSlot result_slot = null;
            while (result_index < slotList.Count)
            {
                var slot = slotList[result_index];
                if (stackable)
                {
                    ///找空格子
                    if (result_slot == null && slot.item == null)
                    {
                        result_slot = slot;
                    }

                    ///找相同格子
                    ///可堆叠的物品
                    ///他们的模板相同
                    if (slot.item != null && slot.item.templateAs(gameObject))
                    {
                        result_slot = slot;
                        break;
                    }
                }
                else
                {
                    ///找空格子
                    if (slot.item == null)
                    {
                        result_slot = slot;
                        break;
                    }
                }
                result_index++;
            }

            ///如果有可以放下的格子
            ///记录数据
            ///并回收临时格子
            if (result_slot != null)
            {
                if (result_slot.item != null)
                {
                    result_slot.count += count;
                    ///如果是可堆叠物品
                    ///并且已有存在的物品
                    ///计数+1并删除自己
                    if (stackable)
                    {
                        gameObject.close();
                    }
                }
                else
                {
                    result_slot.item = gameObject;
                    result_slot.count = count;
                }
            }
            ///如果没有格子用
            ///把当前格子变成现有格子
            ///不回收
            else
            {
                result_slot = new TezInventoryItemSlot(inventory)
                {
                    item = gameObject,
                    count = count,
                    index = slotList.Count
                };

                slotList.Add(result_slot);
            }

            return result_slot;
        }

        public static bool remove(TezGameObject gameObject, int count, List<TezInventoryItemSlot> slotList, out TezInventoryItemSlot resultSlot)
        {
            var index = slotList.FindIndex((TezInventoryItemSlot slot) =>
            {
                return slot.item != null && slot.item.sameAs(gameObject);
            });

            if (index >= 0)
            {
                resultSlot = slotList[index];
                resultSlot.count -= count;
                if (resultSlot.count == 0)
                {
                    resultSlot.item = null;
                    if (resultSlot.index == TezInventoryItemSlot.HideIndex)
                    {
                        slotList.RemoveAt(index);
                    }
                }

                return true;
            }

            resultSlot = null;
            return false;
        }
    }
}