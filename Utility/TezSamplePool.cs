using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 简易的对象池
    /// 无法继承此类
    /// 直接用就行了
    /// </summary>
    public sealed class TezSamplePool<MyObject> where MyObject : new()
    {
        static TezSamplePool<MyObject> m_Instance = new TezSamplePool<MyObject>();
        public static TezSamplePool<MyObject> instance => m_Instance;

        TezEventExtension.Action<MyObject> m_ClearFunction;
        TezEventExtension.Function<MyObject> m_CreateFunction;
        TezEventExtension.Action<MyObject> m_CloseFunction;

        /// <summary>
        /// 设置清理方法
        /// 可以不设置
        /// </summary>
        public TezEventExtension.Action<MyObject> clearFunction
        {
            set { m_ClearFunction = value; }
        }

        /// <summary>
        /// 设置自定义生成方法
        /// 可以不设置
        /// </summary>
        public TezEventExtension.Function<MyObject> createFunction
        {
            set { m_CreateFunction = value; }
        }

        /// <summary>
        /// 关闭此Pool时需要执行的清理操作
        /// </summary>
        public TezEventExtension.Action<MyObject> closeFunction
        {
            set { m_CloseFunction = value; }
        }

        int m_CreateCount = 0;
        /// <summary>
        /// 以创建的个数
        /// </summary>
        public int createCount => m_CreateCount;
        /// <summary>
        /// 回收待用的个数
        /// </summary>
        public int recycleCount => m_Pool.Count;

        Queue<MyObject> m_Pool = new Queue<MyObject>();

        private TezSamplePool()
        {

        }

        /// <summary>
        /// 清理内存
        /// 腾出空间
        /// </summary>
        public void clear()
        {
            if (m_CloseFunction != null)
            {
                foreach (var item in m_Pool)
                {
                    m_CloseFunction(item);
                }
            }
            m_Pool.Clear();
        }

        /// <summary>
        /// 关闭此Pool
        /// 此Pool将永远无法再次使用
        /// </summary>
        public void close()
        {
            this.clear();

            m_Pool = null;

            m_CloseFunction = null;
            m_ClearFunction = null;
            m_CreateFunction = null;
            m_Instance = null;
        }

        /// <summary>
        /// 使用构造函数创建对象
        /// </summary>
        public MyObject create()
        {
            if (m_Pool.Count > 0)
            {
                return m_Pool.Dequeue();
            }

            m_CreateCount++;
            return new MyObject();
        }

        /// <summary>
        /// 使用自定义方发创建对象
        /// </summary>
        public MyObject customCreate()
        {
            if (m_Pool.Count > 0)
            {
                return m_Pool.Dequeue();
            }

            m_CreateCount++;
            return m_CreateFunction();
        }

        /// <summary>
        /// 直接回收
        /// </summary>
        public void recycle(MyObject myObject)
        {
            m_Pool.Enqueue(myObject);
        }

        /// <summary>
        /// 回收并使用自定义方法清理
        /// </summary>
        public void recycleWithClear(MyObject myObject)
        {
            m_ClearFunction(myObject);
            m_Pool.Enqueue(myObject);
        }
    }
}