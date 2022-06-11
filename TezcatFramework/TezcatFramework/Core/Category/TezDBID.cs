using System;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 数据库ID
    /// </summary>
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

        public TezDBID(int dbUID, int itemID)
        {
            this.dbID = dbUID;
            this.itemID = itemID;
        }

        public void close()
        {

        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
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

        /// <summary>
        /// 比较两个Item的DBID是否一样
        /// </summary>
        public static bool operator ==(TezDBID a, TezDBID b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        /// <summary>
        /// 比较两个Item的DBID是否一样
        /// </summary>
        public static bool operator !=(TezDBID a, TezDBID b)
        {
            return !(a == b);
        }
    }
}
