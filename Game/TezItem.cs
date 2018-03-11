namespace tezcat
{
    public interface ITezItem
    {
        int groupID { get; }
        int typeID { get; }
        int objectID { get; set; }
        int GUID { get; set; }
    }

    public abstract class TezItem
        : ITezItem
        , ITezSerializable
    {
        public abstract int groupID { get; }
        public abstract int typeID { get; }
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

        public virtual void serialization(TezJsonWriter writer)
        {
            writer.beginObject("id");
            this.onSerializationID(writer);
            writer.endObject();
        }

        protected virtual void onSerializationID(TezJsonWriter writer)
        {
            var gid = TezItemFactory.convertToGroup(groupID);
            writer.pushValue("group_id", gid.name);
            writer.pushValue("type_id", gid.convertToType(typeID).name);
            writer.pushValue("object_id", objectID > 0 ? objectID : -1);
            writer.pushValue("GUID", this.GUID);
        }

        public virtual void deserialization(TezJsonReader reader)
        {
            reader.enter("id");
            this.onDeserializationID(reader);
            reader.exit();
        }

        protected virtual void onDeserializationID(TezJsonReader reader)
        {
            this.objectID = reader.tryGetInt("object_id", -1);
            this.GUID = reader.tryGetInt("GUID", -1);
        }

        protected abstract void onRefInit();
        protected abstract void onRefZero();
        public abstract void clear();
    }
}