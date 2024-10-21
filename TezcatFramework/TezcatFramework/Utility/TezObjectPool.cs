using System;

namespace tezcat.Framework.Utility
{
    public interface ITezObjectPoolItem
    {
        ITezObjectPool objectPool { get; set; }
        bool recycleThis();
        void destroyThis();
    }

    public interface ITezObjectPool
    {
        string poolName { get; }
        System.Type poolType { get; }
        void recycle(ITezObjectPoolItem item);
        void destroy(int percent);
    }

    public interface ITezObjectPool<T> : ITezObjectPool
    {

    }

    public static class TezObjectPoolHelper
    {
        public static void poolRecycle(this ITezObjectPoolItem item)
        {
            if(item.recycleThis())
            {
                item.objectPool.recycle(item);
            }
        }
    }

    public static class TezObjectPool
    {
        private static void poolDestroy(this ITezObjectPoolItem item)
        {
            item.destroyThis();
            item.objectPool = null;
        }

        #region Manager
        public static event System.Action<ITezObjectPool> evtPoolCreated;
        public static event System.Action<ITezObjectPool, int> evtPoolDestroyed;

        static Dictionary<Type, ITezObjectPool> sDict = new Dictionary<Type, ITezObjectPool>();

        public static void register(ITezObjectPool pool)
        {
            var type = pool.poolType;
            if (!sDict.ContainsKey(type))
            {
                sDict.Add(type, pool);
                evtPoolCreated?.Invoke(pool);
            }
            else
            {
                throw new Exception($"TezSamplePool<{pool.poolName}> be new twice!!");
            }
        }

        public static void destroyAllPool(int percent = 100)
        {
            if(percent <= 0)
            {
                return;
            }

            percent = TezMath.min(percent, 100);

            foreach (var item in sDict)
            {
                evtPoolDestroyed?.Invoke(item.Value, percent);
                item.Value.destroy(percent);
            }
        }
        #endregion

        #region Pool
        /// <summary>
        /// 简易的对象池
        /// 无法继承此类
        /// 直接用就行了
        /// </summary>
        private class Pool<MyObject> : ITezObjectPool<MyObject>
            where MyObject : ITezObjectPoolItem, new()
        {
            public static readonly Pool<MyObject> instance = new Pool<MyObject>();

            string mName = string.Empty;
            /// <summary>
            /// 池名称
            /// (包含类型的名称)
            /// </summary>
            string ITezObjectPool.poolName
            {
                get
                {
                    if (string.IsNullOrEmpty(mName))
                    {
                        mName = $"<{typeof(MyObject).Name}>";
                    }
                    return mName;
                }
            }

            System.Type ITezObjectPool.poolType => typeof(MyObject);

            /// <summary>
            /// 回收待用的个数
            /// </summary>
            public int freeCount => mList.Count;

            List<MyObject> mList = new List<MyObject>();

            Pool()
            {
                register(this);
            }

            /// <summary>
            /// 清理内存
            /// 腾出空间
            /// </summary>
            public void destroy(int percent)
            {
                if (percent <= 0 || mList.Count == 0)
                {
                    return;
                }

                if(percent >= 100)
                {
                    foreach (var item in mList)
                    {
                        item.poolDestroy();
                    }

                    mList.Clear();
                }
                else
                {
                    percent = TezMath.min(percent, 100);
                    float temp = mList.Count * (percent / 100.0f/** 0.01f*/);
                    int count = (int)temp;
                    count = TezMath.max(count, 1);
                    int index;
                    while (count > 0)
                    {
                        index = mList.Count - 1;
                        mList[index].poolDestroy();
                        mList.RemoveAt(index);
                        count--;
                    }
                }

                mList.TrimExcess();
            }

            /// <summary>
            /// 使用构造函数创建对象
            /// </summary>
            public MyObject create()
            {
                if (mList.Count > 0)
                {
                    var obj = mList[mList.Count - 1];
                    mList.RemoveAt(mList.Count - 1);
                    return obj;
                }

                return new MyObject() { objectPool = this };
            }

            void ITezObjectPool.recycle(ITezObjectPoolItem item)
            {
                mList.Add((MyObject)item);
            }
        }


        /// <summary>
        /// 创建对象
        /// </summary>
        public static T create<T>() where T : ITezObjectPoolItem, new()
        {
            return Pool<T>.instance.create();
        }

        /// <summary>
        /// 按百分比删除一定的数量,腾出内存
        /// 0-100
        /// </summary>
        public static void destroy<T>(int percent = 100) where T : ITezObjectPoolItem, new()
        {
            Pool<T>.instance.destroy(percent);
        }

        public static int freeCount<T>() where T : ITezObjectPoolItem, new()
        {
            return Pool<T>.instance.freeCount;
        }


        #endregion

        #region Container
        public class List<T> : System.Collections.Generic.List<T>, ITezObjectPoolItem
        {
            ITezObjectPool ITezObjectPoolItem.objectPool { get; set; }

            void ITezObjectPoolItem.destroyThis()
            {
                this.Clear();
            }

            bool ITezObjectPoolItem.recycleThis()
            {
                this.Clear();
                return true;
            }
        }

        public class Dictionary<Key, Value> : System.Collections.Generic.Dictionary<Key, Value>, ITezObjectPoolItem
        {
            ITezObjectPool ITezObjectPoolItem.objectPool { get; set; }

            void ITezObjectPoolItem.destroyThis()
            {
                this.Clear();
            }

            bool ITezObjectPoolItem.recycleThis()
            {
                this.Clear();
                return true;
            }
        }

        public class HashSet<T> : System.Collections.Generic.HashSet<T>, ITezObjectPoolItem
        {
            ITezObjectPool ITezObjectPoolItem.objectPool { get; set; }

            void ITezObjectPoolItem.destroyThis()
            {
                this.Clear();
            }

            bool ITezObjectPoolItem.recycleThis()
            {
                this.Clear();
                return true;
            }
        }
        #endregion
    }
}