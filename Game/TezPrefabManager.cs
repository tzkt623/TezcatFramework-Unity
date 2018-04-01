using System;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public interface IPrefab
    {
        System.Type GetType();
    }

    public class TezPrefabManager : TezSingleton<TezPrefabManager>
    {
        static Dictionary<System.Type, IPrefab> m_PrefabDic = new Dictionary<System.Type, IPrefab>();
        public static void register(GameObject go)
        {
            var prefab = go.GetComponent<IPrefab>();
            if (prefab != null)
            {
                m_PrefabDic.Add(prefab.GetType(), prefab);
            }
            else
            {
                throw new ArgumentNullException("Prefab Not Found");
            }
        }

        public static void register(List<GameObject> list)
        {
            foreach (var go in list)
            {
                TezPrefabManager.register(go);
            }
        }

        public static T get<T>() where T : class, IPrefab
        {
            IPrefab result = null;
            m_PrefabDic.TryGetValue(typeof(T), out result);
            return (T)result;
        }

        public static IPrefab get(System.Type type)
        {
            IPrefab result = null;
            m_PrefabDic.TryGetValue(type, out result);
            return result;
        }
    }
}