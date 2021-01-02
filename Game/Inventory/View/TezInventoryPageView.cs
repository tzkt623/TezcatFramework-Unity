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
        public class ViewSlot
        {
            public int index { get; set; }
            public TezInventoryItemSlot bindSlot { get; set; }

            public void close()
            {
                this.bindSlot = null;
            }
        }

        /// <summary>
        /// 通单个知槽位刷新
        /// </summary>
        public event TezEventExtension.Action<ViewSlot> onRefresh;

        TezArray<ViewSlot> m_Slots = null;
        int m_BeginPos = 0;

        public int capacity
        {
            get { return m_Slots.capacity; }
        }

        public ViewSlot this[int index]
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

            if (m_InventoryRef.tryGet(out ITezInventory inventory))
            {
                inventory.filter.onItemChangedRefresh -= onItemChanged;
            }

            m_Slots = null;
            onRefresh = null;

            base.close();
        }

        public override void setInventory(ITezInventory inventory)
        {
            base.setInventory(inventory);
            m_InventoryRef.get().filter.onItemChangedRefresh += onItemChanged;
        }

        public void setPageCapacity(int capacity)
        {
            m_Slots = new TezArray<ViewSlot>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                m_Slots.add(new ViewSlot() { index = i });
            }
        }

        public void paging(int beginPos)
        {
            if (m_InventoryRef.tryGet(out ITezInventory inventory))
            {
                m_BeginPos = beginPos;
                for (int i = 0; i < m_Slots.capacity; i++)
                {
                    var index = m_BeginPos + i;
                    var view_slot = m_Slots[i];
                    if (index < inventory.count)
                    {
                        view_slot.bindSlot = inventory[index];
                    }
                    else
                    {
                        view_slot.bindSlot = null;
                    }

                    onRefresh?.Invoke(view_slot);
                }
            }
        }

        private void onItemChanged(TezInventoryItemSlot inventorySlot)
        {
            var slot_index = inventorySlot.index;
            if ((slot_index >= m_BeginPos) && (slot_index < m_BeginPos + m_Slots.capacity))
            {
                var view_slot = m_Slots[slot_index - m_BeginPos];
                view_slot.bindSlot = inventorySlot;
                onRefresh?.Invoke(view_slot);
            }
        }
    }
}