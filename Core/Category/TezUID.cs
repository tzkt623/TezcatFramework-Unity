using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public class TezUID : ITezCloseable
    {
        #region 统计
        static Queue<uint> IDPool = new Queue<uint>();

        public static readonly TezBindable<uint> TotalUID = new TezBindable<uint>();
        public static readonly TezUID Error = new TezUID();

        static uint generateID()
        {
            if (IDPool.Count > 0)
            {
                return IDPool.Dequeue();
            }

            return TotalUID.value++;
        }

        /// <summary>
        /// 被回收的UID数量
        /// </summary>
        public static int freeCount
        {
            get { return IDPool.Count; }
        }
        #endregion

        public uint RTID { get; set; }
        public TezDBID DBID { get; set; }

        public TezUID()
        {
            this.RTID = generateID();
        }

        public void close()
        {
            this.DBID = null;
            IDPool.Enqueue(this.RTID);
        }

        /// <summary>
        /// 比较两个ID是否一样
        /// </summary>
        public bool sameAs(TezUID other)
        {
            return this.RTID == other.RTID;
        }

        public bool dbSameAs(TezUID other)
        {
            if (object.ReferenceEquals(this.DBID, null) || object.ReferenceEquals(other.DBID, null))
            {
                return false;
            }

            return this.DBID.Equals(other.DBID);
        }
    }
}