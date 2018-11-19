using System;
using System.Reflection;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using tezcat.Framework.TypeTraits;
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
        int m_IDGiver = 0;
        class StaticContainer<Prefab> where Prefab : class, ITezPrefab
        {
            public static ITezPrefab prefab = null;
            public static int ID { get; private set; }
            public static void setID(int id)
            {
                ID = id;
            }
        }

        public void foreachPrefab(TezEventExtension.Action<TezPrefabID> action)
        {

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
                Debug.LogWarning(string.Format("{0}`s Prefab Not Found", go.name));
            }
        }

        private void register(ITezPrefab prefab)
        {
            Type type = typeof(StaticContainer<>);
            type = type.MakeGenericType(prefab.GetType());
            type.InvokeMember("prefab"
                , BindingFlags.Public | BindingFlags.Static | BindingFlags.SetField
                , null
                , null
                , new object[] { prefab });

            type.InvokeMember("setID"
                , BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod
                , null
                , null
                , new object[] { m_IDGiver++ });
        }

        public void register<T>(ITezPrefab prefab) where T : class, ITezPrefab
        {
            switch (StaticContainer<T>.ID)
            {
                case TezTypeInfo.ErrorID:
                    StaticContainer<T>.setID(m_IDGiver++);
                    break;
            }

            StaticContainer<T>.prefab = prefab;
        }

        public int getID<T>() where T : class, ITezPrefab
        {
            return StaticContainer<T>.ID;
        }

        public T get<T>() where T : class, ITezPrefab
        {
            return (T)StaticContainer<T>.prefab;
        }

        public void close()
        {

        }
    }
}

