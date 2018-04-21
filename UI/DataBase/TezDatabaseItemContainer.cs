using System.Collections.Generic;
using tezcat.DataBase;
using tezcat.TypeTraits;
using UnityEngine;

using tezcat.Wrapper;
namespace tezcat.UI
{
    public class TezDatabaseItemContainer
        : TezArea
    {
        [SerializeField]
        TezDatabaseSlot m_Prefab = null;

        [SerializeField]
        RectTransform m_Content = null;

        List<TezDatabaseSlot> m_SlotList = new List<TezDatabaseSlot>();

        protected override void onRefresh()
        {

        }

        public void loadItems(TezType group, TezType type)
        {
            List<TezDatabase.ContainerSlot> items = null;

            if (TezDatabase.instance.tryGetItems(group.ID, type.ID, out items))
            {
                for (int i = 0; i < items.Count; i++)
                {
                    var slot = Instantiate(m_Prefab, m_Content, false);
                    slot.open();
                    m_SlotList.Add(slot);
                }

                for (int i = 0; i < items.Count; i++)
                {
                    m_SlotList[i].bind(new TezDatabaseItemWrapper(items[i].item.GUID));
                }
            }
        }

        public void reset()
        {
            foreach (var slot in m_SlotList)
            {
                slot.close();
            }

            m_SlotList.Clear();
        }
    }
}
