using System;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat.Framework.Utility
{
    public static class TezSamplePoolManager
    {
        static Dictionary<Type, ITezSamplePool> m_Dic = new Dictionary<Type, ITezSamplePool>();

        public static void register<T>(T pool) where T : ITezSamplePool
        {
            var type = typeof(T);
            if (!m_Dic.ContainsKey(type))
            {
                Debug.Log("TezSamplePoolManager : " + type.FullName);
                m_Dic.Add(type, pool);
            }
            else
            {
                throw new System.Exception(string.Format("TezSamplePool<{0}> be new twice!!", pool.poolName));
            }
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