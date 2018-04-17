using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using tezcat.Wrapper;
using tezcat.Utility;
using tezcat.DataBase;

namespace tezcat.UI
{
    public class TezDatabaseItemContainer
        : TezArea
    {
        [SerializeField]
        TezDatabaseSlot m_Prefab = null;

        [SerializeField]
        RectTransform m_Content = null;

        List<TezStorageSlot> m_SlotList = new List<TezStorageSlot>();

        protected override void onRefresh()
        {

        }

        public void removeAllItem()
        {
            foreach (var slot in m_SlotList)
            {
                slot.close();
            }

            m_SlotList.Clear();
        }

        public void addItem(ITezItem item)
        {
            var slot = Instantiate(m_Prefab, m_Content, false);
        }

        internal void loadItems(TezRTTI group, TezRTTI type)
        {
            List<DataBase.TezSlot> items = null;
            if (TezDatabase.instance.tryGetItems(group.ID, type.ID, out items))
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var slot = (TezDatabase.DatabaseSlot)items[i];
                    this.addItem(slot.item);
                }
            }
        }
    }
}
