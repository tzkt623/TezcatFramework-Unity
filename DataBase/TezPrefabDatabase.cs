using System;
using System.Collections.Generic;
using tezcat.Core;
using tezcat.Extension;
using UnityEngine;

namespace tezcat.DataBase
{
    public interface ITezPrefab
    {
        Type GetType();
    }

    public sealed class TezPrefabID : ITezCloseable
    {
        public int ID { get; private set; } = -1;
        public ITezPrefab prefab { get; private set; } = null;

        /// <summary>
        /// 不要单独调用此构造函数
        /// </summary>
        public TezPrefabID(int ID, ITezPrefab prefab)
        {
            this.ID = ID;
            this.prefab = prefab;
        }

        public T convertTo<T>() where T : ITezPrefab
        {
            return (T)prefab;
        }

        public void setPrefab(ITezPrefab prefab)
        {
            this.prefab = prefab;
        }

        public void close()
        {
            this.prefab = null;
        }
    }

    public class TezPrefabDatabase : ITezService
    {
        Dictionary<Type, int> m_Dic = new Dictionary<Type, int>();
        List<TezPrefabID> m_List = new List<TezPrefabID>();

        public void foreachPrefab(TezEventExtension.Action<TezPrefabID> action)
        {
            foreach (var prefab in m_List)
            {
                action(prefab);
            }
        }

        public void register(GameObject go)
        {
            var prefab = go.GetComponent<ITezPrefab>();
            if (prefab != null)
            {
                register(prefab);
            }
            else
            {
                throw new ArgumentNullException(string.Format("{0}`s Prefab Not Found", go.name));
            }
        }

        private void register(ITezPrefab prefab)
        {
            int id = -1;
            if (!m_Dic.TryGetValue(prefab.GetType(), out id))
            {
                id = m_List.Count;
                var p = new TezPrefabID(id, prefab);
                m_List.Add(p);
                m_Dic.Add(prefab.GetType(), id);
            }

            m_List[id].setPrefab(prefab);
        }

        public TezPrefabID register<T>() where T : ITezPrefab
        {
            int id = -1;
            if (!m_Dic.TryGetValue(typeof(T), out id))
            {
                var prefab = new TezPrefabID(m_List.Count, null);
                m_List.Add(prefab);
                m_Dic.Add(typeof(T), prefab.ID);
            }

            return m_List[id];
        }

        public T get<T>() where T : class, ITezPrefab
        {
            int id = -1;
            if (m_Dic.TryGetValue(typeof(T), out id))
            {
                return m_List[id].convertTo<T>();
            }
            return null;
        }

        public ITezPrefab get(int ID)
        {
            return m_List[ID].prefab;
        }

        public ITezPrefab get(Type type)
        {
            int id = -1;
            if (m_Dic.TryGetValue(type, out id))
            {
                return m_List[id].prefab;
            }

            return null;
        }

        public T get<T>(int ID) where T : ITezPrefab
        {
            return m_List[ID].convertTo<T>();
        }

        public void close()
        {
            foreach (var item in m_List)
            {
                item.close();
            }

            m_List.Clear();
            m_Dic.Clear();

            m_List = null;
            m_Dic = null;
        }
    }
}

