using System;
using System.Collections.Generic;
using tezcat.Core;
using tezcat.Serialization;
using tezcat.String;
using tezcat.Utility;

namespace tezcat.DataBase
{
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
        public TezSelectType selectType
        {
            get { return TezSelectType.Item; }
        }

        /// <summary>
        /// Item资源
        /// </summary>
        public TezAsset asset { get; protected set; } = new TezAsset();

        public virtual TezStaticString NID { get; set; }
        public int objectID { get; set; } = -1;
        public int GUID { get; set; } = -1;
        public bool unregistered
        {
            get { return objectID == -1 || GUID == -1; }
        }


        int refrence { get; set; } = 0;

        TezPropertyManager m_PorpertyManager = null;
        public List<TezPropertyValue> properties
        {
            get { return m_PorpertyManager.properties; }
        }

        public void addRef()
        {
            if (refrence == 0)
            {
                this.onRefInit();
            }
            refrence += 1;
        }

        public void subRef()
        {
            refrence -= 1;
            if (refrence == 0)
            {
                this.onRefZero();
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
            this.onSerializationID(writer);
            writer.endObject(TezReadOnlyString.Database.id);
        }

        protected virtual void onSerializationID(TezWriter writer)
        {
            writer.write(TezReadOnlyString.Database.group_id, groupType.name);
            writer.write(TezReadOnlyString.Database.type_id, categoryType.name);
            writer.write(TezReadOnlyString.Database.object_id, objectID >= 0 ? objectID : -1);
            writer.write(TezReadOnlyString.Database.GUID, this.GUID);
        }

        public virtual void deserialization(TezReader reader)
        {
            reader.beginObject(TezReadOnlyString.Database.id);
            this.onDeserializationID(reader);
            reader.endObject(TezReadOnlyString.Database.id);
        }

        protected virtual void onDeserializationID(TezReader reader)
        {
            this.objectID = reader.readInt(TezReadOnlyString.Database.object_id);
            this.GUID = reader.readInt(TezReadOnlyString.Database.GUID);
        }

        protected abstract void onRefInit();
        protected abstract void onRefZero();

        public abstract void clear();

        public static T readItem<T>(TezReader reader) where T : TezItem
        {
            reader.beginObject(TezReadOnlyString.Database.id);
            var item = TezDatabaseItemFactory.create<T>(
                reader.readString(TezReadOnlyString.Database.group_id),
                reader.readString(TezReadOnlyString.Database.type_id));
            reader.endObject(TezReadOnlyString.Database.id);

            item.deserialization(reader);
            return item;
        }

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