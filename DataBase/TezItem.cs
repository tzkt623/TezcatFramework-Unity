using System;
using System.Collections.Generic;
using tezcat.Core;
using tezcat.String;
using tezcat.Utility;

namespace tezcat.DataBase
{
    public class TezItemHelper
    {
        public static T create<T>(TezReader reader) where T : TezItem
        {
            reader.beginObject(TezReadOnlyString.Database.ID);
            var item = TezDatabaseItemFactory.create<T>(
                reader.readString(TezReadOnlyString.Database.GID),
                reader.readString(TezReadOnlyString.Database.CID));
            reader.endObject(TezReadOnlyString.Database.ID);
            return item;
        }
    }

    public abstract class TezItem
        : ITezSerializable
        , ITezPropertyOwner
        , ITezSelectable
        , IEquatable<TezItem>
    {
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

        TezPropertyManager m_PorpertyManager = null;
        public List<TezPropertyValue> properties
        {
            get { return m_PorpertyManager.properties; }
        }

        public abstract void clear();

        bool IEquatable<TezItem>.Equals(TezItem other)
        {
            return this.GUID == other.GUID;
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

        public virtual void serialize(TezWriter writer)
        {
            writer.beginObject(TezReadOnlyString.Database.ID);
            writer.write(TezReadOnlyString.Database.NID, this.NID);
            writer.write(TezReadOnlyString.Database.OID, OID >= 0 ? OID : -1);
            writer.write(TezReadOnlyString.Database.GUID, this.GUID);
            writer.endObject(TezReadOnlyString.Database.ID);
        }

        public virtual void deserialize(TezReader reader)
        {
            reader.beginObject(TezReadOnlyString.Database.ID);
            this.NID = reader.readString(TezReadOnlyString.Database.NID);
            this.OID = reader.readInt(TezReadOnlyString.Database.OID);
            this.GUID = reader.readInt(TezReadOnlyString.Database.GUID);
            this.onDeserializationGroupAndCategory(reader);
            reader.endObject(TezReadOnlyString.Database.ID);
        }

        protected abstract void onDeserializationGroupAndCategory(TezReader reader);


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