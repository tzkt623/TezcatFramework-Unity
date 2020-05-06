using System;
using System.Collections.Generic;
using System.Reflection;
using tezcat.Framework.Core;
using tezcat.Framework.TypeTraits;
using UnityEngine;

namespace tezcat.Framework.Database
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

        public void close(bool self_close = true)
        {
            this.prefab = null;
        }
    }

    public class TezPrefabDatabase : ITezService
    {
        Dictionary<string, ITezMultiPrefab> m_MultiDic = new Dictionary<string, ITezMultiPrefab>();

        public int count { get; private set; } = 0;

        class SinglePrefabContainer<Prefab> where Prefab : class, ITezSinglePrefab
        {
            public static ITezSinglePrefab prefab = null;
            public static int ID { get; private set; } = -1;
            public static void setID(int id)
            {
                ID = id;
            }
        }

        public void register(GameObject go)
        {
            var prefab = go.GetComponent<ITezPrefab>();
            if (prefab is ITezSinglePrefab)
            {
                this.register((ITezSinglePrefab)prefab);
                return;
            }

            if (prefab is ITezMultiPrefab)
            {
                m_MultiDic.Add(go.name, (ITezMultiPrefab)prefab);
                return;
            }

            Debug.LogWarning(string.Format("{0}`s Prefab Not Found", go.name));
        }

        private void register(ITezSinglePrefab prefab)
        {
            Type type = typeof(SinglePrefabContainer<>);
            type = type.MakeGenericType(prefab.GetType());
            type.InvokeMember("prefab"
                , BindingFlags.Public | BindingFlags.Static | BindingFlags.SetField
                , null
                , null
                , new object[] { prefab });

            //            Debug.Log(string.Format("{0}/{1}", prefab.GetType().Name, count));

            type.InvokeMember("setID"
                , BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod
                , null
                , null
                , new object[] { count++ });

        }

        public void register<T>(ITezSinglePrefab prefab) where T : class, ITezSinglePrefab
        {
            switch (SinglePrefabContainer<T>.ID)
            {
                case TezTypeInfo.ErrorID:
                    SinglePrefabContainer<T>.setID(count++);
                    break;
            }

            SinglePrefabContainer<T>.prefab = prefab;
        }

        public int getID<T>() where T : class, ITezSinglePrefab
        {
            return SinglePrefabContainer<T>.ID;
        }

        public T get<T>() where T : class, ITezSinglePrefab
        {
            return (T)SinglePrefabContainer<T>.prefab;
        }

        public T get<T>(string multi_name) where T : class, ITezMultiPrefab
        {
            return (T)m_MultiDic[multi_name];
        }

        public void close(bool self_close = true)
        {

        }
    }
}

