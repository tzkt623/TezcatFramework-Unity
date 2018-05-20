using System;
using System.Collections.Generic;
using tezcat.Core;
using tezcat.Serialization;
using tezcat.String;
using tezcat.Utility;

namespace tezcat.DataBase
{
    public class TezItemHelper
    {
        public static T create<T>(TezReader reader) where T : TezItem
        {
            reader.beginObject(TezReadOnlyString.Database.id);
            var item = TezDatabaseItemFactory.create<T>(
                reader.readString(TezReadOnlyString.Database.GID),
                reader.readString(TezReadOnlyString.Database.CID));
            reader.endObject(TezReadOnlyString.Database.id);
            return item;
        }
    }

    public abstract class TezItem
        : ITezData
        , ITezSerializable
        , ITezPropertyOwner
        , ITezSelectable
        , IEquatable<TezItem>
    {
        /// <summary>
        /// 数据库定义的分组信息
        /// </summary>
        public abstract TezDatabase.GroupType groupType { get; }

        /// <summary>
        /// 数据库定义的类型信息
        /// </summary>
        public abstract TezDatabase.CategoryType categoryType { get; }

        /// <summary>
        /// 可被选择器选中的Item类数据
        /// </summary>
        public TezSelectorType selectorType
        {
            get { return TezSelectorType.Item; }
        }

        /// <summary>
        /// Item资源
        /// </summary>
        public TezAsset asset { get; protected set; } = new TezAsset();

        #region IDs
        /// <summary>
        /// Name ID
        /// </summary>
        public TezStaticString NID { get; set; } = new TezStaticString();

        /// <summary>
        /// Object ID
        /// </summary>
        public int OID { get; set; } = -1;

        /// <summary>
        /// Global ID
        /// </summary>
        public int GUID { get; set; } = -1;
        #endregion

        public bool unregistered
        {
            get { return OID == -1 || GUID == -1; }
        }

        public bool isSubItem { get; set; } = false;

        int refrence { get; set; } = 0;

        TezPropertyManager m_PorpertyManager = null;
        public List<TezPropertyValue> properties
        {
            get { return m_PorpertyManager.properties; }
        }

//         protected abstract void onRefInit();
//         protected abstract void onRefZero();

        public abstract void clear();

        public void addRef()
        {
            if (refrence == 0)
            {
//                this.onRefInit();
            }
            refrence += 1;
        }

        public void subRef()
        {
            refrence -= 1;
            if (refrence == 0)
            {
//                this.onRefZero();
            }
        }

        /// <summary>
        /// 注册当前Item的所有属性
        /// </summary>
        public void registerProperty()
        {
            m_PorpertyManager = new TezPropertyManager(this);
            this.onRegisterProperty(m_PorpertyManager);
            m_PorpertyManager.sortProperties();
        }

        /// <summary>
        /// 在这里注册你的Item属性
        /// </summary>
        /// <param name="manager"></param>
        protected abstract void onRegisterProperty(TezPropertyManager manager);

        public bool isTheSameItem(TezItem item)
        {
            return item == null ? false : this.GUID == item.GUID;
        }

        public virtual void serialization(TezWriter writer)
        {
            writer.beginObject(TezReadOnlyString.Database.id);
            writer.write(TezReadOnlyString.Database.NID, this.NID);
            writer.write(TezReadOnlyString.Database.OID, OID >= 0 ? OID : -1);
            writer.write(TezReadOnlyString.Database.GUID, this.GUID);
            writer.write(TezReadOnlyString.Database.GID, groupType.name);
            writer.write(TezReadOnlyString.Database.CID, categoryType.name);
            writer.endObject(TezReadOnlyString.Database.id);
        }

        public virtual void deserialization(TezReader reader)
        {
            reader.beginObject(TezReadOnlyString.Database.id);
            this.NID = reader.readString(TezReadOnlyString.Database.NID);
            this.OID = reader.readInt(TezReadOnlyString.Database.OID);
            this.GUID = reader.readInt(TezReadOnlyString.Database.GUID);
            this.onDeserializationGroupAndCategory(reader);
            reader.endObject(TezReadOnlyString.Database.id);
        }

        protected abstract void onDeserializationGroupAndCategory(TezReader reader);

        bool IEquatable<TezItem>.Equals(TezItem other)
        {
            return this.GUID == other.GUID;
        }


        public static bool operator true(TezItem item)
        {
            return !object.ReferenceEquals(item, null);
        }

        public static bool operator false(TezItem item)
        {
            return object.ReferenceEquals(item, null);
        }

        public static bool operator !(TezItem item)
        {
            return object.ReferenceEquals(item, null);
        }
    }
}