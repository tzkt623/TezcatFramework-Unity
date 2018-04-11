using tezcat.Utility;
namespace tezcat
{
    public interface ITezItem
    {
        string name { get; }
        int groupID { get; }
        int typeID { get; }
        int objectID { get; set; }
        int GUID { get; set; }
        int refrence { get; }
    }

    public abstract class TezItem
        : ITezItem
        , ITezSerializable
    {
        public abstract int groupID { get; }
        public abstract int typeID { get; }

        public virtual string name { get; }
        public int objectID { get; set; } = -1;
        public int GUID { get; set; } = -1;
        public int refrence { get; private set; } = 0;

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

        public bool isTheSameItem(TezItem item)
        {
            return item == null ? false : this.GUID == item.GUID;
        }

        public virtual void serialization(TezJsonWriter writer)
        {
            writer.beginObject(TezReadOnlyString.id);
            this.onSerializationID(writer);
            writer.endObject();
        }

        protected virtual void onSerializationID(TezJsonWriter writer)
        {
            var gid = TezItemFactory.convertToGroup(groupID);
            writer.pushValue(TezReadOnlyString.group_id, gid.name);
            writer.pushValue(TezReadOnlyString.type_id, gid.convertToType(typeID).name);
            writer.pushValue(TezReadOnlyString.object_id, objectID > 0 ? objectID : -1);
            writer.pushValue(TezReadOnlyString.GUID, this.GUID);
        }

        public virtual void deserialization(TezJsonReader reader)
        {
            reader.enter(TezReadOnlyString.id);
            this.onDeserializationID(reader);
            reader.exit();
        }

        protected virtual void onDeserializationID(TezJsonReader reader)
        {
            this.objectID = reader.tryGetInt(TezReadOnlyString.object_id, -1);
            this.GUID = reader.tryGetInt(TezReadOnlyString.GUID, -1);
        }

        protected abstract void onRefInit();
        protected abstract void onRefZero();
        public abstract void clear();
    }
}