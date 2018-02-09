using System;
using System.Collections.Generic;

namespace tezcat
{
    public class HashCache
    {
        private Dictionary<int, string> m_Hashes = new Dictionary<int, string>();

        private static HashCache m_Instance;
        public static HashCache instance
        {
            get
            {
                if(m_Instance == null)
                {
                    m_Instance = new HashCache();
                }
                return m_Instance;
            }
        }

        public string get(int hash)
        {
            string empty = string.Empty;
            this.m_Hashes.TryGetValue(hash, out empty);
            return empty;
        }

        public void add(int hash, string text)
        {
            this.m_Hashes[hash] = text;
        }
    }

}