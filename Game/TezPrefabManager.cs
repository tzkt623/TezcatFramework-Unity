using System;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat.Core
{
    public interface ITezPrefab
    {
        System.Type GetType();
    }

    public class TezPrefabManager : TezSingleton<TezPrefabManager>
    {
        static Dictionary<System.Type, ITezPrefab> m_PrefabDic = new Dictionary<System.Type, ITezPrefab>();
        public static void register(GameObject go)
        {
            var prefab = go.GetComponent<ITezPrefab>();
            if (prefab != null)
            {
                m_PrefabDic.Add(prefab.GetType(), prefab);
            }
            else
            {
                throw new ArgumentNullException(go.name + "`s Prefab Not Found");
            }
        }

        public static void register(List<GameObject> list)
        {
            foreach (var go in list)
            {
                TezPrefabManager.register(go);
            }
        }

        public static T get<T>() where T : class, ITezPrefab
        {
            ITezPrefab result = null;
            m_PrefabDic.TryGetValue(typeof(T), out result);
            return (T)result;
        }

        public static ITezPrefab get(System.Type type)
        {
            ITezPrefab result = null;
            m_PrefabDic.TryGetValue(type, out result);
            return result;
        }
    }
}