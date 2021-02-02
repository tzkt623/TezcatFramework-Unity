using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    /// <summary>
    /// 基础数据库Item类
    /// </summary>
    public abstract class TezDatabaseItem
        : IEquatable<TezDatabaseItem>
        , ITezSerializableItem
        , ITezCloseable
    {
        /// <summary>
        /// Name ID
        /// </summary>
        public string NID { get; set; } = null;

        public TezDBID DBID { get; private set; }

        public void onRegister(int db_id, int item_id)
        {
            this.DBID = new TezDBID(db_id, item_id);
        }

        public void serialize(TezWriter writer)
        {
            this.onSerialize(writer);
        }

        protected virtual void onSerialize(TezWriter writer)
        {
            writer.write(TezReadOnlyString.NID, this.NID);
        }

        public void deserialize(TezReader reader)
        {
            this.onDeserialize(reader);
        }

        protected virtual void onDeserialize(TezReader reader)
        {
            this.NID = reader.readString(TezReadOnlyString.NID);
        }

        public virtual void close()
        {
            DBID.close();
            DBID = null;
        }

        #region 重载
        public override bool Equals(object other)
        {
            return this.Equals((TezDatabaseItem)other);
        }

        public bool Equals(TezDatabaseItem other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return DBID.Equals(other.DBID);
        }

        public static bool operator ==(TezDatabaseItem a, TezDatabaseItem b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        public static bool operator !=(TezDatabaseItem a, TezDatabaseItem b)
        {
            return !(a == b);
        }

        public static bool operator true(TezDatabaseItem item)
        {
            return !object.ReferenceEquals(item, null);
        }

        public static bool operator false(TezDatabaseItem item)
        {
            return object.ReferenceEquals(item, null);
        }
        #endregion
    }

    /// <summary>
    /// 图片文本Item
    /// </summary>
    public abstract class TezDataBaseAssetItem : TezDatabaseItem
    {

    }
}