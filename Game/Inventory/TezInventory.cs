using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.ECS;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// 背包系统接口
    /// </summary>
    public interface ITezInventory : ITezCloseable
    {
        event TezEventExtension.Action<TezInventorySlot> onItemAdded;
        event TezEventExtension.Action<TezInventorySlot> onItemRemoved;

        int slotCount { get; }

        TezInventorySlot this[int index] { get; }
        /// <summary>
        /// 用于过滤器的函数
        /// 没事不要搞他玩
        /// </summary>
        void swapSlots(List<TezInventorySlot> slots);

        /// <summary>
        /// 清空所有事件
        /// </summary>
        void clearEvents();

        /// <summary>
        /// 分页
        /// </summary>
        void paging(int begin, int end, TezEventExtension.Action<int, TezInventorySlot> action);
    }

    /// <summary>
    /// 背包系统
    /// </summary>
    public class TezInventory<Object>
        : ITezInventory
        where Object : TezGameObject
    {
        public event TezEventExtension.Action<TezInventorySlot> onItemAdded;
        public event TezEventExtension.Action<TezInventorySlot> onItemRemoved;

        /// <summary>
        /// 槽位数量
        /// </summary>
        public int slotCount
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
                if (m_Filter == null)
                {
                    m_Filter = new TezInventoryFilter(this, m_Slots);
                }
                return m_Filter;
            }
        }

        List<TezInventorySlot> m_Slots = new List<TezInventorySlot>();

        /// <summary>
        /// 获得一个Slot
        /// </summary>
        public TezInventorySlot this[int index]
        {
            get
            {
                return m_Slots[index];
            }
        }

        public void add(Object game_object, int count)
        {
            var stackable = game_object.templateItem.stackCount > 0;

            TezInventorySlot result = null;
            for (int i = 0; i < m_Slots.Count; i++)
            {
                var slot = m_Slots[i];

                if (stackable)
                {
                    ///找空格子
                    if (result == null && slot.item == null)
                    {
                        result = slot;
                    }

                    ///找相同格子
                    ///可堆叠的物品
                    ///他们的模板相同
                    if (slot.item != null && slot.item.templateAs(game_object))
                    {
                        result = slot;
                        break;
                    }
                }
                else
                {
                    ///找空格子
                    if (slot.item == null)
                    {
                        result = slot;
                        break;
                    }
                }
            }

            ///如果有可以放下的格子
            if (result != null)
            {
                if (result.item != null)
                {
                    result.count += count;
                    ///如果是可堆叠物品
                    ///并且已有存在的物品
                    ///计数+1并删除自己
                    if (stackable)
                    {
                        game_object.close();
                    }
                }
                else
                {
                    result.item = game_object;
                    result.count = count;
                }

                if (result.boundToUI)
                {
                    onItemAdded?.Invoke(result);
                }
            }
            else
            {
                result = new TezInventorySlot(this, m_Slots.Count);
                result.count = count;
                result.item = game_object;

                m_Slots.Add(result);
                onItemAdded?.Invoke(result);
            }
        }

        public void add(int slot_id, Object game_object, int count)
        {
            var slot = m_Slots[slot_id];
            slot.item = game_object;
            slot.count = count;
            if (slot.boundToUI)
            {
                onItemAdded?.Invoke(slot);
            }
        }

        public void remove(int slot_id, int count)
        {
            var slot = m_Slots[slot_id];
            slot.count -= count;
            if (slot.count == 0)
            {
                slot.item = null;
            }

            if (slot.boundToUI)
            {
                onItemRemoved?.Invoke(slot);
            }
        }

        public bool remove(Object game_object, int count)
        {
            var index = m_Slots.FindIndex((TezInventorySlot slot) =>
            {
                return slot.item != null && slot.item.sameAs(game_object);
            });

            if (index >= 0)
            {
                this.remove(index, count);
                return true;
            }

            return false;
        }

        public void paging(int begin, int end, TezEventExtension.Action<int, TezInventorySlot> action)
        {
            for (int i = begin; i < end; i++)
            {
                action(i, m_Slots[i]);
            }
        }

        public void clearEvents()
        {
            onItemAdded = null;
            onItemRemoved = null;
        }

        public virtual void close()
        {
            for (int i = 0; i < m_Slots.Count; i++)
            {
                m_Slots[i].close();
            }
            m_Slots.Clear();
            m_Slots = null;

            onItemAdded = null;
            onItemRemoved = null;

            m_Filter?.close();
            m_Filter = null;
        }

        void ITezInventory.swapSlots(List<TezInventorySlot> slots)
        {
            m_Slots = slots;
        }
    }
}