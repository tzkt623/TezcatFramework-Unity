using System;
using System.Collections.Generic;
using System.Reflection;
using tezcat.Framework.Core;
using tezcat.Framework.TypeTraits;
using UnityEngine;

namespace tezcat.Unity.Database
{
    public enum TezPrefabCount
    {
        Invaild,
        Single,
        Multiple
    }

    public interface ITezPrefab
    {
        TezPrefabCount prefabCount { get; }
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

        void ITezCloseable.closeThis()
        {
            this.prefab = null;
        }
    }

    public static class TezPrefabDatabase
    {
        static Dictionary<string, ITezPrefab> mMultiDic = new Dictionary<string, ITezPrefab>();

        public static int count { get; private set; } = 0;

        class SinglePrefabContainer<Prefab> where Prefab : class, ITezPrefab
        {
            public static ITezPrefab prefab = null;
            static int m_ID = -1;
            public static int ID => m_ID;
            public static void setID(int id)
            {
                m_ID = id;
            }
        }

        public static void register(GameObject go)
        {
            var prefab = go.GetComponent<ITezPrefab>();
            if(prefab == null)
            {
                return;
            }

            switch (prefab.prefabCount)
            {
                case TezPrefabCount.Multiple:
                    mMultiDic.Add(go.name, prefab);
                    break;
                case TezPrefabCount.Single:
                    register(prefab);
                    break;
                default:
                    break;
            }

        }

        private static void register(ITezPrefab prefab)
        {
            Type type = typeof(SinglePrefabContainer<>);
            type = type.MakeGenericType(prefab.GetType());
            type.InvokeMember("prefab"
                , BindingFlags.Public | BindingFlags.Static | BindingFlags.SetField
                , null
                , null
                , new object[] { prefab });

            //            Debug.Log(string.Format("{0}/{1}", prefab.GetType().Name, count));

            type.InvokeMember("m_ID"
                , BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetField
                , null
                , null
                , new object[] { count++ });
        }

        public static void register<T>(ITezPrefab prefab) where T : class, ITezPrefab
        {
            switch (SinglePrefabContainer<T>.ID)
            {
                case TezTypeInfo.ErrorID:
                    SinglePrefabContainer<T>.setID(count++);
                    break;
            }

            SinglePrefabContainer<T>.prefab = prefab;
        }

        public static int getID<T>() where T : class, ITezPrefab
        {
            return SinglePrefabContainer<T>.ID;
        }

        public static T get<T>() where T : class, ITezPrefab
        {
            return (T)SinglePrefabContainer<T>.prefab;
        }

        public static T get<T>(string name) where T : class, ITezPrefab
        {
            return (T)mMultiDic[name];
        }
    }
}