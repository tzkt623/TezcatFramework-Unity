using System;
using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 实体对象的唯一ID
    /// </summary>
    public class TezObjectUID
        : ITezCloseable
        , IEquatable<TezObjectUID>
    {
        #region 统计
        static Queue<uint> IDPool = new Queue<uint>();

        public static readonly TezBindable<uint> TotalUID = new TezBindable<uint>();
        public static readonly TezObjectUID Error = new TezObjectUID();

        /// <summary>
        /// 被回收的UID数量
        /// </summary>
        public static int freeCount
        {
            get { return IDPool.Count; }
        }

        public static uint generateID()
        {
            if (IDPool.Count > 0)
            {
                return IDPool.Dequeue();
            }

            return ++TotalUID.value;
        }

        public static void recycleID(uint uid)
        {
            if (uid != 0)
            {
                IDPool.Enqueue(uid);
            }
        }
        #endregion

        uint m_RTID;
        public uint RTID => m_RTID;

        public TezObjectUID()
        {
            m_RTID = generateID();
        }

        /// <summary>
        /// 比较两个实体的运行时ID是否一样
        /// </summary>
        public bool sameAs(TezObjectUID other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return m_RTID == other.m_RTID;
        }

        public void close()
        {
            IDPool.Enqueue(m_RTID);
        }

        public override int GetHashCode()
        {
            return m_RTID.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return this.sameAs((TezObjectUID)other);
        }

        public bool Equals(TezObjectUID other)
        {
            return this.sameAs(other);
        }

        /// <summary>
        /// 比较两个实体的运行时ID是否一样
        /// </summary>
        public static bool operator ==(TezObjectUID a, TezObjectUID b)
        {
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
            {
                return false;
            }

            return a.sameAs(b);
        }

        /// <summary>
        /// 比较两个实体的运行时ID是否一样
        /// </summary>
        public static bool operator !=(TezObjectUID a, TezObjectUID b)
        {
            return !(a == b);
        }
    }
}