using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using UnityEngine;

namespace tezcat.Framework.DataBase
{
    public interface ITezPrefab
    {
        Type GetType();
    }

    public sealed class TezPrefabID : ITezCloseable
    {
        public ITezPrefab prefab { get; private set; } = null;
        public int ID { get; }

        /// <summary>
        /// 不要单独调用此构造函数
        /// </summary>
        public TezPrefabID(int id, ITezPrefab prefab)
        {
            this.ID = id;
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
        Dictionary<Type, TezPrefabID> m_Dic = new Dictionary<Type, TezPrefabID>();
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
            TezPrefabID temp = null;
            if (!m_Dic.TryGetValue(prefab.GetType(), out temp))
            {
                temp = new TezPrefabID(m_List.Count, prefab);
                m_List.Add(temp);
                m_Dic.Add(prefab.GetType(), temp);
            }

            temp.setPrefab(prefab);
        }

        public TezPrefabID register<T>(ITezPrefab prefab) where T : ITezPrefab
        {
            TezPrefabID temp = null;
            if (!m_Dic.TryGetValue(typeof(T), out temp))
            {
                temp = new TezPrefabID(m_List.Count, prefab);
                m_List.Add(temp);
                m_Dic.Add(typeof(T), temp);
            }

            return temp;
        }

        public int getID<T>() where T : ITezPrefab
        {
            TezPrefabID temp = null;
            if (m_Dic.TryGetValue(typeof(T), out temp))
            {
                return temp.ID;
            }
            else
            {
                throw new ArgumentNullException(string.Format("{0} is not registered!!", typeof(T).Name));
            }
        }

        public int getID(Type type)
        {
            TezPrefabID temp = null;
            if (m_Dic.TryGetValue(type, out temp))
            {
                return temp.ID;
            }
            else
            {
                throw new ArgumentNullException(string.Format("{0} is not registered!!", type.Name));
            }
        }

        public T get<T>() where T : class, ITezPrefab
        {
            TezPrefabID temp = null;
            if (m_Dic.TryGetValue(typeof(T), out temp))
            {
                return temp.convertTo<T>();
            }
            else
            {
                throw new ArgumentNullException(string.Format("{0} is not registered!!", typeof(T).Name));
            }
        }

        public ITezPrefab get(int ID)
        {
            return m_List[ID].prefab;
        }

        public ITezPrefab get(Type type)
        {
            TezPrefabID temp = null;
            if (m_Dic.TryGetValue(type, out temp))
            {
                return temp.prefab;
            }
            else
            {
                throw new ArgumentNullException(string.Format("{0} is not registered!!", type.Name));
            }
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

