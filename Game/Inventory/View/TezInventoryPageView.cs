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

        public void setPageCapacity(int capacity)
        {
            m_Slots = new TezArray<TezInventoryViewSlot>(capacity);
            for (int i = 0; i < capacity; i++)
            {
                m_Slots.add(new TezInventoryViewSlot() { index = i });
            }
        }

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

        protected override void onItemChanged(TezInventoryDataSlot dataSlot)
        {
            var slot_index = dataSlot.index;
            if ((slot_index >= m_BeginPos) && (slot_index < m_BeginPos + m_Slots.capacity))
            {
                var view_slot = m_Slots[slot_index - m_BeginPos];
                view_slot.bindSlot = dataSlot.itemSlot;
                onSlotRefresh?.Invoke(view_slot);
            }
        }

        protected override void onFilterChanged(TezInventoryFilter filterManager)
        {
            this.paging(0);
        }
    }
}