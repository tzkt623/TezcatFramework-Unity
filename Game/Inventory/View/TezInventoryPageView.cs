using UnityEngine;
using System.Collections;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;
using System;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game.Inventory
{
    /// <summary>
    /// 卡尺式Inventory分页显示器
    /// </summary>
    public class TezInventoryPageView : TezInventoryBaseView
    {
        /// <summary>
        /// 通单个知槽位刷新
        /// </summary>
        public event TezEventExtension.Action<TezInventoryViewSlot> onSlotRefresh;

        TezArray<TezInventoryViewSlot> m_Slots = null;
        int m_BeginPos = 0;

        public int capacity
        {
            get { return m_Slots.capacity; }
        }

        public TezInventoryViewSlot this[int index]
        {
            get { return m_Slots[index]; }
        }

        public override void close()
        {
            foreach (var item in m_Slots)
            {
                item.close();
            }
            m_Slots.close();

            m_Slots = null;
            onSlotRefresh = null;

            base.close();
        }

        public override void setInventory(TezInventory inventory)
        {
            base.setInventory(inventory);
            this.filterManager.setInventory(inventory);
            this.filterManager.onItemChanged += this.onItemChanged;
        }

        public void setPageCapacity(int capacity)
        {
            m_Slots = new TezArray<TezInventoryViewSlot>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                m_Slots.add(new TezInventoryViewSlot() { index = i });
            }
        }

//         public void paging(int beginPos)
//         {
//             if (m_InventoryRef.tryGet(out var inventory))
//             {
//                 m_BeginPos = beginPos;
//                 for (int i = 0; i < m_Slots.capacity; i++)
//                 {
//                     var index = m_BeginPos + i;
//                     var view_slot = m_Slots[i];
//                     if (index < inventory.count)
//                     {
//                         view_slot.bindSlot = inventory[index];
//                     }
//                     else
//                     {
//                         view_slot.bindSlot = null;
//                     }
// 
//                     onSlotRefresh?.Invoke(view_slot);
//                 }
//             }
//         }

        public void paging(int beginPos)
        {
            m_BeginPos = beginPos;
            for (int i = 0; i < m_Slots.capacity; i++)
            {
                var index = m_BeginPos + i;
                var view_slot = m_Slots[i];
                if (index < this.filterManager.count)
                {
                    view_slot.bindSlot = this.filterManager[index].itemSlot;
                }
                else
                {
                    view_slot.bindSlot = null;
                }

                onSlotRefresh?.Invoke(view_slot);
            }
        }

        private void onItemChanged(TezInventoryItemSlot inventorySlot)
        {
            var slot_index = inventorySlot.index;
            if ((slot_index >= m_BeginPos) && (slot_index < m_BeginPos + m_Slots.capacity))
            {
                var view_slot = m_Slots[slot_index - m_BeginPos];
                view_slot.bindSlot = inventorySlot;
                onSlotRefresh?.Invoke(view_slot);
            }
        }

        private void onItemChanged(TezInventoryDataSlot dataSlot)
        {
            var slot_index = dataSlot.index;
            if ((slot_index >= m_BeginPos) && (slot_index < m_BeginPos + m_Slots.capacity))
            {
                var view_slot = m_Slots[slot_index - m_BeginPos];
                view_slot.bindSlot = dataSlot.itemSlot;
                onSlotRefresh?.Invoke(view_slot);
            }
        }
    }
}