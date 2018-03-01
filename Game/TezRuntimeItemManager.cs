using System;
using System.Collections.Generic;

namespace tezcat
{
    public class TezRuntimeItemManager
    {
        List<List<List<TezItem>>> m_Items = new List<List<List<TezItem>>>();

        public void addItem(TezItem item)
        {
            while(m_Items.Count <= item.groupID)
            {
                m_Items.Add(new List<List<TezItem>>());
            }

            var container = m_Items[item.groupID];
            while(container.Count <= item.typeID)
            {
                container.Add(new List<TezItem>());
            }

            container[item.typeID].Add(item);
        }

        public void removeItem(TezItem item)
        {
            m_Items[item.groupID][item.typeID].Remove(item);
        }
    }
}