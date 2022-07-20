using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// 
    /// 背包系统
    /// 
    /// 不可堆叠加入
    /// 说明每一个物品
    /// 在底层上都是不同的数据
    /// 一个单独的存在
    /// 就算他的ID是一样的
    /// 
    /// 可堆叠加入
    /// 说明物品在底层上都是同一个物品
    /// 即引用同一个数据
    /// 就算ID不同
    /// 
    /// #待解决问题
    /// 如何共享可堆叠物品的对象
    /// 
    /// </summary>
    public class TezInventory
        : TezRefObject
    {
        public event TezEventExtension.Action<TezInventoryItemSlot> onItemAdded;
        public event TezEventExtension.Action<TezInventoryItemSlot> onItemRemoved;

        /// <summary>
        /// 物品数量
        /// </summary>
        public int count
        {
            get { return m_Slots.Count; }
        }

        List<TezInventoryItemSlot> m_Slots = new List<TezInventoryItemSlot>();

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

        /// <summary>
        /// 可以自己修改Slot
        /// </summary>
        protected virtual TezInventoryItemSlot createItemSlot()
        {
            return new TezInventoryItemSlot(this);
        }

        /// <summary>
        /// 存入
        /// 不允许堆叠
        /// </summary>
        public void store(ITezInventoryItem item)
        {
            TezInventoryItemSlot result_slot = this.findSlotUnstackable();

            ///找到空格子
            if (result_slot != null)
            {
                result_slot.item = item;
                result_slot.count = -1;
            }
            ///没有找到空格子
            else
            {
                result_slot = this.createItemSlot();
                result_slot.item = item;
                result_slot.count = -1;
                result_slot.index = m_Slots.Count;

                m_Slots.Add(result_slot);
            }

            onItemAdded?.Invoke(result_slot);
        }

        /// <summary>
        /// 堆叠型加入
        /// </summary>
        public void store(ITezInventoryItem item, int count)
        {
            var stack_max_count = item.stackCount;
            TezInventoryItemSlot result_slot = this.findSlotStackable(item);

            ///找到格子
            if (result_slot != null)
            {
                if (result_slot.item != null)
                {
                    result_slot.count += count;
                    var remain = result_slot.count - stack_max_count;
                    if (remain > 0)
                    {
                        result_slot.count = stack_max_count;
                        this.store(item, remain);
                    }
                    else
                    {
                        item.recycle();
                    }
                }
                else
                {
                    result_slot.item = item;
                    if (count > stack_max_count)
                    {
                        result_slot.count = stack_max_count;
                        this.store(item, count - stack_max_count);
                    }
                    else
                    {
                        result_slot.count = count;
                    }
                }
            }
            ///没有找到格子
            else
            {
                result_slot = this.createItemSlot();
                result_slot.item = item;
                result_slot.count = count;
                result_slot.index = m_Slots.Count;

                m_Slots.Add(result_slot);
            }

            onItemAdded?.Invoke(result_slot);
        }

        public void store(int slotIndex, ITezInventoryItem item, int count)
        {
            var slot = m_Slots[slotIndex];
            slot.item = item;
            slot.count += count;

            onItemAdded?.Invoke(slot);
        }

        /// <summary>
        /// 从包裹里取出
        /// 理论上
        /// 你应该知道精确的位置
        /// 才能从包裹里面取出物品
        /// </summary>
        public ITezInventoryItem take(int slotIndex, int count)
        {
            var slot = m_Slots[slotIndex];
            var item = slot.item;

            slot.count -= count;
            if (slot.count == 0)
            {
                slot.item = null;
            }

            onItemRemoved?.Invoke(slot);

            return item;
        }

        /// <summary>
        /// 从包裹里取出
        /// 理论上
        /// 你应该知道精确的位置
        /// 才能从包裹里面取出物品
        /// </summary>
        public ITezInventoryItem take(int slotIndex)
        {
            var slot = m_Slots[slotIndex];
            var item = slot.item;

            slot.item = null;
            slot.count = -1;

            onItemRemoved?.Invoke(slot);

            return item;
        }

        private TezInventoryItemSlot findSlotUnstackable()
        {
            int result_index = 0;

            while (result_index < m_Slots.Count)
            {
                var slot = m_Slots[result_index];
                ///找空格子
                if (slot.item == null)
                {
                    return slot;
                }
                result_index++;
            }

            return null;
        }

        private TezInventoryItemSlot findSlotStackable(ITezInventoryItem item)
        {
            TezInventoryItemSlot result_slot = null;

            int result_index = 0;
            while (result_index < m_Slots.Count)
            {
                var slot = m_Slots[result_index];
                ///找空格子
                if (result_slot == null && slot.item == null)
                {
                    result_slot = slot;
                }

                ///找相同格子
                ///可堆叠的物品
                ///他们的模板相同
                if (slot.item != null && slot.item.templateAs(item))
                {
                    result_slot = slot;
                    break;
                }

                result_index++;
            }

            return result_slot;
        }

        /// <summary>
        /// 从包裹里面查询是否有某个物品
        /// 返回-1表示没有
        /// </summary>
        public int find(ITezInventoryItem gameObject)
        {
            return m_Slots.FindIndex((TezInventoryItemSlot slot) =>
            {
                return slot.item == gameObject;
            });
        }

        public override void close()
        {
            base.close();

            for (int i = 0; i < m_Slots.Count; i++)
            {
                m_Slots[i].close();
            }
            m_Slots.Clear();
            m_Slots = null;

            onItemAdded = null;
            onItemRemoved = null;
        }
    }
}