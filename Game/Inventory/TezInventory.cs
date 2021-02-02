using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.ECS;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game.Inventory
{
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
}