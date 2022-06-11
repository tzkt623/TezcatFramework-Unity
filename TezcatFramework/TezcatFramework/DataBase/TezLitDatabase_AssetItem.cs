using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    public class TezLitDatabase_AssetItem : ITezCloseable
    {
        public class Table
        {
            List<TezDatabaseAssetItem> m_List = new List<TezDatabaseAssetItem>();
            Dictionary<string, int> m_NameSearchDic = new Dictionary<string, int>();

            public int count => m_List.Count;

            public void register(TezDatabaseAssetItem item)
            {
                var index = m_List.Count;
                m_List.Add(item);
                m_NameSearchDic.Add(item.NID, index);
                this.onAdd(index);
            }

            protected virtual void onAdd(int index)
            {

            }

            public TezDatabaseAssetItem get(string name)
            {
                if (m_NameSearchDic.TryGetValue(name, out int index))
                {
                    return m_List[index];
                }

                throw new Exception();
            }

            /// <summary>
            /// 使用当前Container的Index间接获得Database中的Item
            /// </summary>
            public TezDatabaseAssetItem get(int index)
            {
                return m_List[index];
            }
        }

        List<Table> m_TableList = new List<Table>();

        public Table getOrCreateTable(int tableID)
        {
            while (m_TableList.Count >= tableID)
            {
                m_TableList.Add(new Table());
            }

            return m_TableList[tableID];
        }

        public Table getTable(int tableID)
        {
            return m_TableList[tableID];
        }

        public virtual void close()
        {

        }
    }
}