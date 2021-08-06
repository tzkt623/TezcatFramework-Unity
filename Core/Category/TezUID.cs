using System;
using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 实体对象的唯一ID
    /// </summary>
    public class TezUID
        : ITezCloseable
        , IEquatable<TezUID>
    {
        #region 统计
        static Queue<uint> IDPool = new Queue<uint>();

        public static readonly TezBindable<uint> TotalUID = new TezBindable<uint>();
        public static readonly TezUID Error = new TezUID();

        /// <summary>
        /// 被回收的UID数量
        /// </summary>
        public static int freeCount
        {
            get { return IDPool.Count; }
        }

        static uint generateID()
        {
            if (IDPool.Count > 0)
            {
                return IDPool.Dequeue();
            }

            return TotalUID.value++;
        }

        #endregion

        uint m_RTID;
        public uint RTID => m_RTID;

        public TezDBID DBID { get; set; }

        public TezUID()
        {
            m_RTID = generateID();
        }

        /// <summary>
        /// 比较两个实体的运行时ID是否一样
        /// </summary>
        public bool sameAs(TezUID other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return m_RTID == other.m_RTID;
        }

        /// <summary>
        /// 比较两个实体的数据库ID是否一样
        /// </summary>
        public bool dbSameAs(TezUID other)
        {
            if (object.ReferenceEquals(this.DBID, null) || object.ReferenceEquals(other.DBID, null))
            {
                return false;
            }

            return this.DBID.Equals(other.DBID);
        }

        public void close()
        {
            this.DBID = null;
            IDPool.Enqueue(m_RTID);
        }

        public override int GetHashCode()
        {
            return m_RTID.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return this.sameAs((TezUID)other);
        }

        public bool Equals(TezUID other)
        {
            return this.sameAs(other);
        }

        /// <summary>
        /// 比较两个实体的运行时ID是否一样
        /// </summary>
        public static bool operator ==(TezUID a, TezUID b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.sameAs(b);
        }

        /// <summary>
        /// 比较两个实体的运行时ID是否一样
        /// </summary>
        public static bool operator !=(TezUID a, TezUID b)
        {
            return !(a == b);
        }
    }
}