using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace tezcat
{
    public class TezDataBase
    {
        List<TezDataBaseTable> m_TableList = new List<TezDataBaseTable>();
        Dictionary<string, int> m_TableDic = new Dictionary<string, int>();

        public TezDataBaseTable createTable(string name)
        {
            int id = -1;
            if(m_TableDic.TryGetValue(name, out id))
            {
                return m_TableList[id];
            }
            else
            {
                var table = new TezDataBaseTable(m_TableList.Count);
                m_TableList.Add(table);
                return table;
            }
        }
    }
}