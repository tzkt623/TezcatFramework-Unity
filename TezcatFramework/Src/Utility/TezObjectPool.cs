using System;

namespace tezcat.Framework.Utility
{
    public interface ITezObjectPoolItem
    {
        /// <summary>
        /// 让对象池管理器知道此对象属于哪个对象池
        /// </summary>
        ITezObjectPool objectPool { get; set; }

        /// <summary>
        /// 尝试回收当前实例到对象池中。
        /// </summary>
        /// <remarks>
        /// 如果回收成功，返回 true；否则返回 false。
        /// </remarks>
        bool tryRecycleThis();

        /// <summary>
        /// 销毁当前实例。
        /// </summary>
        void onDestroyThis();
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

    public static class TezObjectPool
    {
        //public static void poolRecycle<T>(this ITezObjectPoolItem item) where T : ITezObjectPoolItem, new()
        //{
        //    if (item.recycleThis())
        //    {
        //        TezObjectPool.recycle<T>(item);
        //        //item.objectPool.recycle(item);
        //    }
        //}

        /// <summary>
        /// 回收当前实例到对象池中。
        /// </summary>
        /// <param name="item"></param>
        public static bool recycleToPool(this ITezObjectPoolItem item)
        {
            if (item.tryRecycleThis())
            {
                item.objectPool.recycle(item);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 销毁当前实例。
        /// </summary>
        /// <param name="item"></param>
        private static void destroyThisItem(this ITezObjectPoolItem item)
        {
            item.onDestroyThis();
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
                        item.destroyThisItem();
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
                        mList[index].destroyThisItem();
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

            public void recycle(ITezObjectPoolItem item)
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

        //不提供此方法是为了实现抽象对象也能顺利回收
        //public static void recycle<T>(ITezObjectPoolItem item) where T : ITezObjectPoolItem, new()
        //{
        //    if(item.recycleThis())
        //    {
        //        Pool<T>.instance.recycle(item);
        //    }
        //}

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

            void ITezObjectPoolItem.onDestroyThis()
            {
                this.Clear();
            }

            bool ITezObjectPoolItem.tryRecycleThis()
            {
                this.Clear();
                return true;
            }
        }

        public class Dictionary<Key, Value> : System.Collections.Generic.Dictionary<Key, Value>, ITezObjectPoolItem
        {
            ITezObjectPool ITezObjectPoolItem.objectPool { get; set; }

            void ITezObjectPoolItem.onDestroyThis()
            {
                this.Clear();
            }

            bool ITezObjectPoolItem.tryRecycleThis()
            {
                this.Clear();
                return true;
            }
        }

        public class HashSet<T> : System.Collections.Generic.HashSet<T>, ITezObjectPoolItem
        {
            ITezObjectPool ITezObjectPoolItem.objectPool { get; set; }

            void ITezObjectPoolItem.onDestroyThis()
            {
                this.Clear();
            }

            bool ITezObjectPoolItem.tryRecycleThis()
            {
                this.Clear();
                return true;
            }
        }
        #endregion
    }
}