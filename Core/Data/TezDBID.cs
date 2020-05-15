namespace tezcat.Framework.Core
{
    public class TezDBID : ITezCloseable
    {
        /// <summary>
        /// 数据库ID
        /// </summary>
        public int dbID { get; }

        /// <summary>
        /// 物品ID
        /// </summary>
        public int itemID { get; }

        public TezDBID(int db_id, int item_id)
        {
            this.dbID = db_id;
            this.itemID = item_id;
        }

        /// <summary>
        /// 比较是否相同
        /// </summary>
        public bool sameAs(TezDBID other)
        {
            return this.dbID == other.dbID && this.itemID == other.itemID;
        }

        public void close(bool self_close = true)
        {

        }
    }
}
