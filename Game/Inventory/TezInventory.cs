using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.ECS;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// 背包系统
    /// </summary>
    public class TezInventory
        : TezRefObject
    //        , ITezInventory
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

        public void add(TezComData gameObject, int count)
        {
            var stackable = TezDatabaseItemConfig.getConfig(gameObject.category).stackCount > 0;

            TezInventoryItemSlot result_slot = null;
            if (stackable)
            {
                result_slot = this.findSlotStackable(gameObject);
            }
            else
            {
                result_slot = this.findSlotNoStackable();
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
                result_slot = new TezInventoryItemSlot(this)
                {
                    item = gameObject,
                    count = count,
                    index = m_Slots.Count
                };

                m_Slots.Add(result_slot);
            }

            onItemAdded?.Invoke(result_slot);
        }

        private TezInventoryItemSlot findSlotNoStackable()
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

        private TezInventoryItemSlot findSlotStackable(TezComData gameObject)
        {
            int result_index = 0;
            TezInventoryItemSlot result_slot = null;
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
                if (slot.item != null && slot.item.templateAs(gameObject))
                {
                    result_slot = slot;
                    break;
                }

                result_index++;
            }

            return result_slot;
        }

        public void remove(TezComData gameObject, int count)
        {
            var index = m_Slots.FindIndex((TezInventoryItemSlot slot) =>
            {
                return slot.item == gameObject;
            });

            if (index >= 0)
            {
                var resultSlot = m_Slots[index];
                resultSlot.count -= count;
                if (resultSlot.count == 0)
                {
                    resultSlot.item = null;
                }

                onItemRemoved?.Invoke(resultSlot);
            }
        }

        public void add(int slotIndex, TezComData gameObject, int count)
        {
            var slot = m_Slots[slotIndex];
            slot.item = gameObject;
            slot.count += count;

            onItemAdded?.Invoke(slot);
        }

        public void remove(int slotIndex, int count)
        {
            var slot = m_Slots[slotIndex];
            slot.count -= count;
            if (slot.count == 0)
            {
                slot.item = null;
            }

            onItemRemoved?.Invoke(slot);
        }

        //         public void add(TezComData gameObject, int count)
        //         {
        //             m_Filter.add(gameObject, count);
        //         }
        // 
        //         /// <summary>
        //         /// 向当前Slot中添加
        //         /// </summary>
        //         public void add(int slotIndex, TezComData gameObject, int count)
        //         {
        //             var slot = m_Slots[slotIndex];
        //             slot.item = gameObject;
        //             slot.count += count;
        // 
        //             m_Filter.notifyItemChanged(slot);
        //         }

        /// <summary>
        /// 从当前Slot中移除
        /// </summary>
        //         public void remove(int slotIndex, int count)
        //         {
        //             var slot = m_Slots[slotIndex];
        //             slot.count -= count;
        //             if (slot.count == 0)
        //             {
        //                 slot.item = null;
        //             }
        // 
        //             m_Filter.notifyItemChanged(slot);
        //         }
        // 
        //         public bool remove(TezComData gameObject, int count)
        //         {
        //             if (m_Filter.remove(gameObject, count))
        //             {
        //                 return true;
        //             }
        // 
        //             return false;
        //         }

        public virtual void close()
        {
            base.close();

            for (int i = 0; i < m_Slots.Count; i++)
            {
                m_Slots[i].close();
            }
            m_Slots.Clear();
            m_Slots = null;

            //             m_Filter.close();
            //             m_Filter = null;

            onItemAdded = null;
            onItemRemoved = null;
        }

        //         void ITezInventory.swapSlots(List<TezInventoryItemSlot> slots)
        //         {
        //             m_Slots = slots;
        //         }
    }
}