using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 简易的对象池
    /// 无法继承此类
    /// 直接用就行了
    /// </summary>
    public sealed class TezSamplePool<MyObject>
        : ITezSamplePool
        where MyObject : new()
    {
        static TezSamplePool<MyObject> sInstance = new TezSamplePool<MyObject>();
        public static TezSamplePool<MyObject> instance => sInstance;

        TezEventExtension.Action<MyObject> mClearFunction;
        TezEventExtension.Function<MyObject> mCreateFunction;
        TezEventExtension.Action<MyObject> mCloseFunction;

        /// <summary>
        /// 设置清理方法
        /// 可以不设置
        /// </summary>
        public TezEventExtension.Action<MyObject> clearFunction
        {
            set { mClearFunction = value; }
        }

        /// <summary>
        /// 设置自定义生成方法
        /// 可以不设置
        /// </summary>
        public TezEventExtension.Function<MyObject> createFunction
        {
            set { mCreateFunction = value; }
        }

        /// <summary>
        /// 关闭此Pool时需要执行的清理操作
        /// </summary>
        public TezEventExtension.Action<MyObject> closeFunction
        {
            set { mCloseFunction = value; }
        }

        int mCreateCount = 0;

        string mName = string.Empty;
        /// <summary>
        /// 池名称
        /// (包含类型的名称)
        /// </summary>
        public string poolName
        {
            get
            {
                if (string.IsNullOrEmpty(mName))
                {
                    mName = $"TezSamplePool<{typeof(MyObject).Name}>";
                }
                return mName;
            }
        }
        /// <summary>
        /// 以创建的个数
        /// </summary>
        public int createCount => mCreateCount;
        /// <summary>
        /// 回收待用的个数
        /// </summary>
        public int recycleCount => mPool.Count;

        Queue<MyObject> mPool = new Queue<MyObject>();

        private TezSamplePool()
        {
            TezSamplePoolManager.register(this);
        }

        /// <summary>
        /// 清理内存
        /// 腾出空间
        /// </summary>
        public void destroyObjects()
        {
            if (mCloseFunction != null)
            {
                foreach (var item in mPool)
                {
                    mCloseFunction(item);
                }
            }
            mPool.Clear();
        }

        /// <summary>
        /// 关闭此Pool
        /// 此Pool将永远无法再次使用
        /// </summary>
        public void destroy()
        {
            this.destroyObjects();

            mPool = null;

            mCloseFunction = null;
            mClearFunction = null;
            mCreateFunction = null;
            sInstance = null;
        }

        /// <summary>
        /// 使用构造函数创建对象
        /// </summary>
        public MyObject create()
        {
            if (mPool.Count > 0)
            {
                return mPool.Dequeue();
            }

            mCreateCount++;
            return new MyObject();
        }

        /// <summary>
        /// 使用自定义方发创建对象
        /// </summary>
        public MyObject customCreate()
        {
            if (mPool.Count > 0)
            {
                return mPool.Dequeue();
            }

            mCreateCount++;
            return mCreateFunction();
        }

        /// <summary>
        /// 直接回收
        /// </summary>
        public void recycle(MyObject myObject)
        {
            mPool.Enqueue(myObject);
        }

        /// <summary>
        /// 回收并使用自定义方法清理
        /// </summary>
        public void recycleWithClear(MyObject myObject)
        {
            mClearFunction(myObject);
            mPool.Enqueue(myObject);
        }

        object ITezSamplePool.create()
        {
            return this.create();
        }

        object ITezSamplePool.customCreate()
        {
            return this.customCreate();
        }

        void ITezSamplePool.recycle(object obj)
        {
            this.recycle((MyObject)obj);
        }

        void ITezSamplePool.recycleWithClear(object obj)
        {
            this.recycleWithClear((MyObject)obj);
        }
    }
}