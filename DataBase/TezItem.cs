using System.Collections.Generic;
using tezcat.Serialization;
using tezcat.Utility;

namespace tezcat.DataBase
{
    public interface ITezItem
    {
        string nameID { get; }

        TezDatabase.GroupType groupType { get; }
        TezDatabase.CategoryType categoryType { get; }

        int objectID { get; set; }
        int GUID { get; set; }
        int refrence { get; }
    }

    public abstract class TezItem
        : ITezItem
        , ITezSerializable
        , ITezPropertyOwner
    {
        public abstract TezDatabase.GroupType groupType { get; }
        public abstract TezDatabase.CategoryType categoryType { get; }

        public virtual string nameID { get; set; }
        public int objectID { get; set; } = -1;
        public int GUID { get; set; } = -1;
        public int refrence { get; private set; } = 0;

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
            m_PorpertyManager.sortFunctions();
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
    }
}