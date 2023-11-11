using System;
using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    public static class TezSamplePoolManager
    {
        static Dictionary<Type, ITezSamplePool> mDict = new Dictionary<Type, ITezSamplePool>();

        public static void register<T>(T pool) where T : ITezSamplePool
        {
            var type = typeof(T);
            if (!mDict.ContainsKey(type))
            {
                //Console.WriteLine("TezSamplePoolManager : " + type.FullName);
                mDict.Add(type, pool);
            }
            else
            {
                throw new Exception($"TezSamplePool<{pool.poolName}> be new twice!!");
            }
        }

        public static void closeAll()
        {
            foreach (var item in mDict)
            {
                item.Value.destroy();
            }
            mDict.Clear();
        }
    }
}