using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Database
{
    public abstract class TezDatabaseItem
        : IEquatable<TezDatabaseItem>
        , ITezSerializableItem
        , ITezCloseable
    {
        public enum Category
        {
            AssetItem,
            GameItem
        }

        /// <summary>
        /// Name ID
        /// </summary>
        public string NID { get; set; } = null;

        /// <summary>
        /// 类型
        /// </summary>
        public abstract Category itemCategory { get; }

        public virtual void serialize(TezWriter writer)
        {
            writer.write(TezReadOnlyString.NID, this.NID);
        }

        public virtual void deserialize(TezReader reader)
        {
            this.NID = reader.readString(TezReadOnlyString.NID);
        }

        public abstract void close(bool self_close = true);

        #region 重载
        public override bool Equals(object other)
        {
            return this.Equals((TezDatabaseItem)other);
        }

        public abstract bool Equals(TezDatabaseItem other);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(TezDatabaseItem a, TezDatabaseItem b)
        {
            var flagA = object.ReferenceEquals(a, null);
            var flagB = object.ReferenceEquals(b, null);

            ///(true && true) || (!true && !true && a == b)
            ///true || (?)
            ///
            ///(true && false) || (!true && !false && a== b)
            ///false || (false && ?)
            ///
            ///(false && false) || (!false && !false && a== b)
            ///false || (true && a == b)
            ///
            return (flagA && flagB) || (!flagA && !flagB && a.Equals(b));
        }

        public static bool operator !=(TezDatabaseItem a, TezDatabaseItem b)
        {
            var flagA = object.ReferenceEquals(a, null);
            var flagB = object.ReferenceEquals(b, null);

            ///(!true || !true) && (true || true || a != b)
            ///false && (?)
            ///
            ///(!true || !false) && (true || false || 
            ///true && (true || ?)
            ///
            ///(!false || !false) && (false || false) || (a != b)
            ///true && (false || a != b)
            ///
            return (!flagA || !flagB) && (flagA || flagB || !a.Equals(b));
        }

        public static bool operator true(TezDatabaseItem item)
        {
            return !object.ReferenceEquals(item, null);
        }

        public static bool operator false(TezDatabaseItem item)
        {
            return object.ReferenceEquals(item, null);
        }

        public static bool operator !(TezDatabaseItem item)
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
        public override Category itemCategory
        {
            get { return Category.AssetItem; }
        }

        public override bool Equals(TezDatabaseItem other)
        {
            return this.NID == other.NID;
        }
    }

    /// <summary>
    /// 游戏对象Item
    /// </summary>
    public abstract class TezDatabaseGameItem : TezDatabaseItem
    {
        /// <summary>
        /// 类型
        /// </summary>
        public override Category itemCategory
        {
            get { return Category.GameItem; }
        }

        /// <summary>
        /// Class ID
        /// </summary>
        public string CID { get; private set; }

        public abstract ITezGroup group { get; }

        public abstract ITezSubgroup subgroup { get; }

        public ulong itemID
        {
            get { return this.RID.itemID; }
        }

        public TezRID RID { get; private set; } = null;

        /// <summary>
        /// 属性
        /// </summary>
        public TezPropertySortList properties { get; private set; } = new TezPropertySortList();

        public List<string> TAGS { get; private set; } = new List<string>();

        public TezDatabaseGameItem()
        {
            this.registerProperty(this.properties);
        }

        public override void close(bool self_close = true)
        {
            this.CID = null;

            this.properties.clear();
            this.properties = null;

            this.TAGS.Clear();
            this.TAGS = null;

            this.RID.close();
            this.RID = null;
        }

        public TezEntity createObject(bool copy = false)
        {
            var obj = this.onCreateObject();
            obj.initWithData(this, copy);

            var entity = TezEntity.create();
            entity.addComponent(obj);
            return entity;
        }

        public override void serialize(TezWriter writer)
        {
            base.serialize(writer);
            writer.write(TezReadOnlyString.CID, this.CID);
            writer.write(TezReadOnlyString.NID, this.NID);
        }

        protected void serializeTag(TezWriter writer)
        {
            writer.beginArray(TezReadOnlyString.TAG);
            for (int i = 0; i < TAGS.Count; i++)
            {
                writer.write(TAGS[i]);
            }
            writer.endArray(TezReadOnlyString.TAG);
        }

        public override void deserialize(TezReader reader)
        {
            base.deserialize(reader);
            this.CID = reader.readString(TezReadOnlyString.CID);
            this.NID = reader.readString(TezReadOnlyString.NID);
        }

        protected void deserializeTag(TezReader reader)
        {
            reader.beginArray(TezReadOnlyString.TAG);
            var count = reader.count;
            for (int i = 0; i < count; i++)
            {
                TAGS.Add(reader.readString(i));
            }
            reader.endArray(TezReadOnlyString.TAG);
        }

        protected virtual TezGameObject onCreateObject()
        {
            throw new Exception(string.Format("Please override this method for {0}", this.GetType().Name));
        }

        public override bool Equals(TezDatabaseItem other)
        {
            var go = other as TezDatabaseGameItem;
            return go ? this.group.Equals(go.group) && this.subgroup.Equals(go.subgroup) : false;
        }

        protected virtual void registerProperty(TezPropertySortList collection) { }

        /// <summary>
        /// 数据库回调函数
        /// 不要手动调用
        /// </summary>
        /// <param name="db_id"></param>
        public void onAddToDB(int db_id)
        {
            if (this.RID != null)
            {
                throw new ArgumentException("RID");
            }

            this.RID = new TezRID(group, subgroup, db_id);
        }
    }
}