﻿using System;
using System.Collections.Generic;
using tezcat.Core;

namespace tezcat.DataBase
{
    public abstract class TezDataBaseItem
        : IEquatable<TezDataBaseItem>
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
            writer.write(TezReadOnlyString.Database.NID, this.NID);
        }

        public virtual void deserialize(TezReader reader)
        {
            this.NID = reader.readString(TezReadOnlyString.Database.NID);
        }

        public abstract void close();

        #region 重载
        public override bool Equals(object other)
        {
            return this.Equals((TezDataBaseItem)other);
        }

        public abstract bool Equals(TezDataBaseItem other);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(TezDataBaseItem a, TezDataBaseItem b)
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

        public static bool operator !=(TezDataBaseItem a, TezDataBaseItem b)
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

        public static bool operator true(TezDataBaseItem item)
        {
            return !object.ReferenceEquals(item, null);
        }

        public static bool operator false(TezDataBaseItem item)
        {
            return object.ReferenceEquals(item, null);
        }

        public static bool operator !(TezDataBaseItem item)
        {
            return object.ReferenceEquals(item, null);
        }
        #endregion
    }

    /// <summary>
    /// 图片文本Item
    /// </summary>
    public abstract class TezDataBaseAssetItem : TezDataBaseItem
    {
        public override Category itemCategory
        {
            get { return Category.AssetItem; }
        }

        public override bool Equals(TezDataBaseItem other)
        {
            return this.NID == other.NID;
        }
    }

    /// <summary>
    /// 游戏对象Item
    /// </summary>
    public abstract class TezDataBaseGameItem : TezDataBaseItem
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

        public abstract ITezSubGroup subgroup { get; }

        public int itemID
        {
            get { return this.RID.itemID; }
        }

        public TezRID RID { get; private set; } = null;

        /// <summary>
        /// 属性
        /// </summary>
        public ITezPropertyCollection properties { get; private set; } = new TezPropertyList();

        public List<string> TAGS { get; private set; } = new List<string>();

        public TezDataBaseGameItem()
        {
            this.registerProperty(this.properties);
        }

        public override void close()
        {
            this.CID = null;

            this.properties.close();
            this.properties = null;

            this.TAGS.Clear();
            this.TAGS = null;

            this.RID.close();
            this.RID = null;
        }

        public TezGameObject createObject()
        {
            var obj = this.onCreateObject();
            obj.initWithData(this);
            return obj;
        }

        public override void serialize(TezWriter writer)
        {
            base.serialize(writer);
            writer.write(TezReadOnlyString.Database.CID, this.CID);
        }

        protected void serializeTag(TezWriter writer)
        {
            writer.beginArray(TezReadOnlyString.Database.TAG);
            for (int i = 0; i < TAGS.Count; i++)
            {
                writer.write(TAGS[i]);
            }
            writer.endArray(TezReadOnlyString.Database.TAG);
        }

        public override void deserialize(TezReader reader)
        {
            base.deserialize(reader);
            this.CID = reader.readString(TezReadOnlyString.Database.CID);
        }

        protected void deserializeTag(TezReader reader)
        {
            reader.beginArray(TezReadOnlyString.Database.TAG);
            var count = reader.count;
            for (int i = 0; i < count; i++)
            {
                TAGS.Add(reader.readString(i));
            }
            reader.endArray(TezReadOnlyString.Database.TAG);
        }

        protected virtual TezGameObject onCreateObject()
        {
            return TezService.get<TezClassFactory>().create<TezGameObject>(this.CID);
        }

        public override bool Equals(TezDataBaseItem other)
        {
            var go = other as TezDataBaseGameItem;
            return go ? this.group.Equals(go.group) && this.subgroup.Equals(go.subgroup) : false;
        }

        protected abstract void registerProperty(ITezPropertyCollection collection);

        public abstract TezDataBaseGameItem clone();

        /// <summary>
        /// 数据库回调函数
        /// 不要手动调用
        /// </summary>
        /// <param name="db_id"></param>
        public void onAddToDB(int db_id)
        {
            if(this.RID != null)
            {
                throw new ArgumentException("RID");
            }

            this.RID = new TezRID(group, subgroup, db_id);
        }
    }
}