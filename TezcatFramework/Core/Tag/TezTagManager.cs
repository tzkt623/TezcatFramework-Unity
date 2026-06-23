using System;
using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public static class TezTagManager
    {
        public class Element
        {
            public int id;
            public string name;
        }

        static Dictionary<string, int> mDict = new Dictionary<string, int>();
        static List<Element> mList = new List<Element>();

        public static int register(string name)
        {
            if (mDict.ContainsKey(name))
            {
                throw new Exception("TezTag has already existed!!!");
            }
            else
            {
                int id = mList.Count;
                var tag = new Element()
                {
                    name = name,
                    id = id
                };
                mList.Add(tag);
                mDict.Add(name, id);

                return id;
            }
        }

        public static Element get(int id)
        {
            return mList[id];
        }

        public static Element get(string name)
        {
            if (mDict.TryGetValue(name, out int id))
            {
                return mList[id];
            }

            return null;
        }
    }
}