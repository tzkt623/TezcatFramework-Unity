using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 实体对象的唯一ID
    /// </summary>
    public static class TezObjectUID
    //         : ITezCloseable
    //         , IEquatable<TezObjectUID>
    {
        /// <summary>
        /// 对象被创建时
        /// </summary>
        public static event TezEventExtension.Action<uint> evtObjectGenerated;
        /// <summary>
        /// 对象被回收时
        /// </summary>
        public static event TezEventExtension.Action<uint> evtObjectRecycled;

        /// <summary>
        /// 错误UID
        /// </summary>
        public const uint cErrorUID = 0;

        #region 统计
        static Queue<uint> IDPool = new Queue<uint>();
        static uint mIDGenerate = 0;

        //public static readonly TezBindable<uint> TotalUID = new TezBindable<uint>();
        //public static readonly TezObjectUID Error = new TezObjectUID();

        public static uint totalCount => mIDGenerate;

        /// <summary>
        /// 激活的
        /// </summary>
        public static uint activedCount
        {
            get { return mIDGenerate - (uint)IDPool.Count; }
        }

        /// <summary>
        /// 被回收的UID数量
        /// </summary>
        public static uint freeCount
        {
            get { return (uint)IDPool.Count; }
        }

        public static uint generateID()
        {
            if (IDPool.Count > 0)
            {
                return IDPool.Dequeue();
            }

            mIDGenerate++;
            evtObjectGenerated?.Invoke(mIDGenerate);
            return mIDGenerate;
        }

        public static void recycleID(uint uid)
        {
            if (uid != cErrorUID)
            {
                IDPool.Enqueue(uid);
                evtObjectRecycled?.Invoke(uid);
            }
        }
        #endregion

        /*
        uint mRTID;
        public uint RTID => mRTID;

        public TezObjectUID()
        {
            mRTID = generateID();
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

            return mRTID == other.mRTID;
        }

        public void close()
        {
            IDPool.Enqueue(mRTID);
        }

        public override init GetHashCode()
        {
            return mRTID.GetHashCode();
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
        */
    }
}