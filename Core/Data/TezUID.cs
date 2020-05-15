using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public class TezUID : ITezCloseable
    {
        #region 统计

        public static readonly TezBindable<uint> TotalUID = new TezBindable<uint>();

        /// <summary>
        /// 错误ID
        /// </summary>
        public static readonly TezUID Error = new TezUID();

        static Queue<uint> IDPool = new Queue<uint>();

        static uint generateID()
        {
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

        public void close(bool self_close = true)
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
    }
}