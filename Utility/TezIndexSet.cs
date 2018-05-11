using System.Collections.Generic;


namespace tezcat.Utility
{
    public class TezIndexSet<Key, Value>
    {
        List<Value> m_List = new List<Value>();
        Dictionary<Key, int> m_Dic = new Dictionary<Key, int>();
    }
}