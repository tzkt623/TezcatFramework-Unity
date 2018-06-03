﻿using System;
using System.Collections.Generic;
using tezcat.Signal;
using tezcat.UI;
using tezcat.Wrapper;
using UnityEngine;

namespace tezcat.DataBase
{
    public interface ITezPrefab
    {
        Type GetType();
    }

    public abstract class TezPrefabDatabase : ScriptableObject
    {
        public class Prefab
        {
            public int ID { get; private set; } = -1;
            public ITezPrefab prefab { get; private set; } = null;

            /// <summary>
            /// 不要单独调用此构造函数
            /// </summary>
            public Prefab(int ID, ITezPrefab prefab)
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
        }

        #region Manager
        static Dictionary<Type, int> m_PrefabDic = new Dictionary<Type, int>();
        static List<Prefab> m_List = new List<Prefab>();

        public static void foreachPrefab(TezEventBus.Action<Prefab> action)
        {
            foreach (var prefab in m_List)
            {
                action(prefab);
            }
        }

        public static void load<Prefab>(List<Prefab> list) where Prefab : ITezPrefab
        {
            foreach (var widget in list)
            {
                if (widget != null)
                {
                    TezPrefabDatabase.register(widget);
                }
            }
        }

        public static void load(List<GameObject> list)
        {
            foreach (var go in list)
            {
                if (go)
                {
                    TezPrefabDatabase.register(go);
                }
            }
        }

        public static void register(GameObject go)
        {
            var prefab = go.GetComponent<ITezPrefab>();
            if (prefab != null)
            {
                register(prefab);
            }
            else
            {
                throw new ArgumentNullException(go.name + "`s Prefab Not Found");
            }
        }

        private static void register(ITezPrefab prefab)
        {
            int id = -1;
            if (!m_PrefabDic.TryGetValue(prefab.GetType(), out id))
            {
                id = m_List.Count;
                var p = new Prefab(id, prefab);
                m_List.Add(p);
                m_PrefabDic.Add(prefab.GetType(), id);
            }

            m_List[id].setPrefab(prefab);
        }

        public static Prefab register<T>() where T : ITezPrefab
        {
            int id = -1;
            if (!m_PrefabDic.TryGetValue(typeof(T), out id))
            {
                var prefab = new Prefab(m_List.Count, null);
                m_List.Add(prefab);
                m_PrefabDic.Add(typeof(T), prefab.ID);
            }

            return m_List[id];
        }

        public static T get<T>() where T : class, ITezPrefab
        {
            int id = -1;
            if (m_PrefabDic.TryGetValue(typeof(T), out id))
            {
                return m_List[id].convertTo<T>();
            }
            return null;
        }

        public static ITezPrefab get(int ID)
        {
            return m_List[ID].prefab;
        }

        public static ITezPrefab get(Type type)
        {
            int id = -1;
            if (m_PrefabDic.TryGetValue(type, out id))
            {
                return m_List[id].prefab;
            }

            return null;
        }

        public static T get<T>(int ID) where T : ITezPrefab
        {
            return m_List[ID].convertTo<T>();
        }
        #endregion

        [SerializeField]
        List<TezToolWindow> ToolWindow = new List<TezToolWindow>();
        [SerializeField]
        List<TezToolWidget> ToolWidget = new List<TezToolWidget>();

        [SerializeField]
        List<TezGameWindow> GameWindow = new List<TezGameWindow>();
        [SerializeField]
        List<TezGameWidget> GameWidget = new List<TezGameWidget>();

        [SerializeField]
        List<TezObjectMB> ObjectMB = new List<TezObjectMB>();

        public virtual void init()
        {
            load(ToolWindow);
            load(ToolWidget);
            load(GameWindow);
            load(GameWidget);
            load(ObjectMB);
        }
    }
}

