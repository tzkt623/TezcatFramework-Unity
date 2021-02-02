using System;

namespace tezcat.Framework.Core
{
    public class TezDBID
        : ITezCloseable
        , IEquatable<TezDBID>
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

        public void close()
        {

        }

        public override bool Equals(object obj)
        {
            return this.Equals((TezDBID)obj);
        }

        public bool Equals(TezDBID other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return this.dbID == other.dbID && this.itemID == other.itemID;
        }
    }
}
