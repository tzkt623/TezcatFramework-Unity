using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    public class TezHashCache
    {
        private Dictionary<int, string> m_Hashes = new Dictionary<int, string>();

        private static TezHashCache m_Instance;
        public static TezHashCache instance
        {
            get
            {
                if(m_Instance == null)
                {
                    m_Instance = new TezHashCache();
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