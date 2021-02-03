using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    public static class TezSamplePoolManager
    {
        static Dictionary<string, ITezSamplePool> m_Dic = new Dictionary<string, ITezSamplePool>();

        public static void register<T>(T pool) where T : ITezSamplePool
        {
            if (!m_Dic.ContainsKey(pool.name))
            {
                m_Dic.Add(pool.name, pool);
            }
            else
            {
                throw new System.Exception(string.Format("TezSamplePool<{0}> be new twice!!", pool.name));
            }
        }

        public static ITezSamplePool getPool(string name)
        {
            return m_Dic[name];
        }

        public static void closeAll()
        {
            foreach (var item in m_Dic)
            {
                item.Value.close();
            }
            m_Dic.Clear();
        }
    }
}