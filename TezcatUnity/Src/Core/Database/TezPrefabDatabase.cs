using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using tezcat.Framework.Core;
using tezcat.Framework.TypeTraits;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

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
        GameObject gameObject { get; }
        string name { get; }
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

        public void close()
        {
            this.prefab = null;
        }
    }

    class OperationHandleComperer : IEqualityComparer<HashSet<string>>
    {
        public bool Equals(HashSet<string> x, HashSet<string> y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if (x == null || y == null)
            {
                return false;
            }

            return x.SetEquals(y);
        }

        public int GetHashCode(HashSet<string> obj)
        {
            return obj.GetHashCode();
        }
    }

    public class TezDBPrefab
    {
        private static HashSet<TezDBPrefab> allDB = new HashSet<TezDBPrefab>();
        public static void unloadAll()
        {
            foreach (var db in allDB)
            {
                db.clearAll();
            }
        }

        public TezDBPrefab()
        {
            allDB.Add(this);
        }

        Dictionary<string, GameObject> mGameObjectCache = new Dictionary<string, GameObject>();
        Dictionary<HashSet<string>, AsyncOperationHandle> mOperationHandleCache = new Dictionary<HashSet<string>, AsyncOperationHandle>(new OperationHandleComperer());

        public IEnumerator loadPrefabsByTagsAsync(IEnumerable keys)
        {
            HashSet<string> keys_cache = new HashSet<string>();

            foreach (var key in keys)
            {
                keys_cache.Add(key.ToString());
            }

            if (!mOperationHandleCache.TryGetValue(keys_cache, out var go))
            {
                var handle = Addressables.LoadAssetsAsync<GameObject>(keys, (asset) =>
                {
                    if (mGameObjectCache.ContainsKey(asset.name))
                    {
                        throw new Exception($"Prefab {asset.name} found in database.");
                    }
                    else
                    {
                        mGameObjectCache.Add(asset.name, asset);
                    }
                }
                , Addressables.MergeMode.Intersection);

                yield return handle;

                mOperationHandleCache.Add(keys_cache, handle);
            }
        }

        public T getPerfab<T>(string name) where T : MonoBehaviour
        {
            if (mGameObjectCache.TryGetValue(name, out var go))
            {
                return go.GetComponent<T>();
            }
            else
            {
                throw new Exception($"Prefab {name} not found in database.");
            }
        }

        public void clearAll()
        {
            foreach (var pair in mGameObjectCache)
            {
                UnityEngine.Object.Destroy(pair.Value);
            }
            mGameObjectCache.Clear();

            foreach (var handle in mOperationHandleCache)
            {
                Addressables.Release(handle);
            }
            mOperationHandleCache.Clear();
        }
    }

    public static class TezDBPrefab<T>
    {
        public readonly static TezDBPrefab instance = new TezDBPrefab();
    }

    public class TezPrefabDatabase
    {
        List<ITezPrefab> mPrefabList = new List<ITezPrefab>();
        List<Dictionary<string, ITezPrefab>> mMultiplePrefabCache = new List<Dictionary<string, ITezPrefab>>();

        Dictionary<HashSet<string>, AsyncOperationHandle> mHandleDict = new Dictionary<HashSet<string>, AsyncOperationHandle>(new OperationHandleComperer());

        class SinglePrefabContainer<Prefab> where Prefab : class, ITezPrefab
        {
            static int mID = -1;
            public static int ID => mID;
            public static void setID(int id)
            {
                mID = id;
            }
        }

        class MultiplePrefabContainer<Prefab> where Prefab : class, ITezPrefab
        {
            public static Dictionary<string, ITezPrefab> mDict = new Dictionary<string, ITezPrefab>();

            private static void add(string key, ITezPrefab prefab)
            {
                mDict.Add(key, prefab);
            }
        }

        public int count { get; private set; } = 0;

        public IEnumerator loadPrefabsAsync(IEnumerable keys, Addressables.MergeMode mode = Addressables.MergeMode.None)
        {
            HashSet<string> keys_cache = new HashSet<string>();
            foreach (var key in keys)
            {
                keys_cache.Add((string)key);
            }

            if (!mHandleDict.ContainsKey(keys_cache))
            {
                var handle = Addressables.LoadAssetsAsync<GameObject>(keys, null, mode, true);
                yield return handle;

                var list = handle.Result;
                int count = 0;
                while (count < list.Count)
                {
                    this.register(list[count]);
                    count++;
                    //yield return null;
                }

                mHandleDict.Add(keys_cache, handle);
            }

            yield return null;
        }

        public void register(GameObject go)
        {
            if (!go.TryGetComponent<ITezPrefab>(out var prefab))
            {
                return;
            }

            switch (prefab.prefabCount)
            {
                case TezPrefabCount.Multiple:
                    //mMultiDic.Add(go.name, prefab);
                    registerMultiple(prefab);
                    break;
                case TezPrefabCount.Single:
                    registerSingle(prefab);
                    break;
                default:
                    break;
            }

        }

        private void registerMultiple(ITezPrefab prefab)
        {
            Type type = typeof(MultiplePrefabContainer<>);
            Debug.Log(prefab.GetType().Name);
            type = type.MakeGenericType(prefab.GetType());
            //             type.InvokeMember("add"
            //                 , BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.InvokeMethod
            //                 , null
            //                 , null
            //                 , new object[] { prefab.name, prefab });

            var field = type.GetField("mDict", BindingFlags.Public | BindingFlags.Static);
            if (field != null)
            {
                object value = field.GetValue(null);
                if (value is Dictionary<string, ITezPrefab> dict)
                {
                    dict.Add(prefab.name, prefab);
                    mMultiplePrefabCache.Add(dict);
                }
                else
                {
                    throw new Exception();
                }
            }
            else
            {
                throw new Exception();
            }
        }

        private void registerSingle(ITezPrefab prefab)
        {
            Type type = typeof(SinglePrefabContainer<>);
            Debug.Log(prefab.GetType().Name);
            type = type.MakeGenericType(prefab.GetType());
            //             type.InvokeMember("prefab"
            //                 , BindingFlags.Public | BindingFlags.Static | BindingFlags.SetField
            //                 , null
            //                 , null
            //                 , new object[] { prefab });

            type.InvokeMember("mID"
                , BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.SetField
                , null
                , null
                , new object[] { mPrefabList.Count });

            mPrefabList.Add(prefab);
        }

        public void register<T>(ITezPrefab prefab) where T : class, ITezPrefab
        {
            if (SinglePrefabContainer<T>.ID == TezTypeInfo.ErrorID)
            {
                SinglePrefabContainer<T>.setID(mPrefabList.Count);
                mPrefabList.Add(prefab);
            }
            else { throw new Exception(); }
        }

        public int getID<T>() where T : class, ITezPrefab
        {
            return SinglePrefabContainer<T>.ID;
        }

        public T get<T>() where T : class, ITezPrefab
        {
            return (T)mPrefabList[SinglePrefabContainer<T>.ID];
        }

        public T get<T>(string name) where T : class, ITezPrefab
        {
            var dict = MultiplePrefabContainer<T>.mDict;
            dict.TryGetValue(name, out var prefab);
            return (T)prefab;
        }

        public bool tryGet<T>(string name, out T prefab) where T : class, ITezPrefab
        {
            var res = MultiplePrefabContainer<T>.mDict.TryGetValue(name, out var temp);
            prefab = (T)temp;
            return res;
        }

        public void clearAll()
        {
            foreach (var item in mPrefabList)
            {
                UnityEngine.Object.Destroy(item.gameObject);
            }
            mPrefabList.Clear();


            foreach (var dict in mMultiplePrefabCache)
            {
                foreach (var pair in dict)
                {
                    UnityEngine.Object.Destroy(pair.Value.gameObject);
                }
                dict.Clear();
            }

            mMultiplePrefabCache.Clear();
        }
    }
}