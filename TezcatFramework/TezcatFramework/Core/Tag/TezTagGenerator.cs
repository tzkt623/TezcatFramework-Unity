using System;
using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public static class TezTagGenerator
    {
        public class Element
        {
            public int id;
            public string name;
        }

        #region Manager
        static Dictionary<string, int> m_Dict = new Dictionary<string, int>();
        static List<Element> m_List = new List<Element>();

        public static int register(string name)
        {
            if (m_Dict.ContainsKey(name))
            {
                throw new Exception("TezTag has already existed!!!");
            }
            else
            {
                int id = m_List.Count;
                var tag = new Element()
                {
                    name = name,
                    id = id
                };
                m_List.Add(tag);
                m_Dict.Add(name, id);

                return id;
            }
        }

        public static Element get(int id)
        {
            return m_List[id];
        }

        public static Element get(string name)
        {
            if (m_Dict.TryGetValue(name, out int id))
            {
                return m_List[id];
            }

            return null;
        }
        #endregion
    }
}