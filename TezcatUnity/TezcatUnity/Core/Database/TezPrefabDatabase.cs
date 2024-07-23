using System;
using System.Collections.Generic;
using System.Reflection;
using tezcat.Framework.Core;
using tezcat.Framework.TypeTraits;
using UnityEngine;

namespace tezcat.Unity.Database
{
    public interface ITezPrefab
    {
        Type GetType();
    }

    public interface ITezMultiPrefab : ITezPrefab
    {

    }

    public interface ITezSinglePrefab : ITezPrefab
    {

    }

    public sealed class TezPrefabID : ITezCloseable
    {
        public ITezSinglePrefab prefab { get; private set; } = null;
        public int ID { get; }

        /// <summary>
        /// 不要单独调用此构造函数
        /// </summary>
        public TezPrefabID(int id, ITezSinglePrefab prefab)
        {
            this.ID = id;
            this.prefab = prefab;
        }

        public T convertTo<T>() where T : ITezSinglePrefab
        {
            return (T)prefab;
        }

        public void setPrefab(ITezSinglePrefab prefab)
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
        static Dictionary<string, ITezMultiPrefab> mMultiDic = new Dictionary<string, ITezMultiPrefab>();

        public static int count { get; private set; } = 0;

        class SinglePrefabContainer<Prefab> where Prefab : class, ITezSinglePrefab
        {
            public static ITezSinglePrefab prefab = null;
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
            if (prefab is ITezSinglePrefab sprefab)
            {
                register(sprefab);
                return;
            }

            if (prefab is ITezMultiPrefab mprefab)
            {
                mMultiDic.Add(go.name, mprefab);
                return;
            }

            Debug.LogWarning(string.Format("{0}`s Prefab Not Found", go.name));
        }

        private static void register(ITezSinglePrefab prefab)
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

        public static void register<T>(ITezSinglePrefab prefab) where T : class, ITezSinglePrefab
        {
            switch (SinglePrefabContainer<T>.ID)
            {
                case TezTypeInfo.ErrorID:
                    SinglePrefabContainer<T>.setID(count++);
                    break;
            }

            SinglePrefabContainer<T>.prefab = prefab;
        }

        public static int getID<T>() where T : class, ITezSinglePrefab
        {
            return SinglePrefabContainer<T>.ID;
        }

        public static T get<T>() where T : class, ITezSinglePrefab
        {
            return (T)SinglePrefabContainer<T>.prefab;
        }

        public static T get<T>(string name) where T : class, ITezMultiPrefab
        {
            return (T)mMultiDic[name];
        }
    }
}