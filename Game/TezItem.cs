namespace tezcat
{
    public abstract class TezItem
        : ITezSerializable
    {
        public TezAsset asset { get; protected set; }

        public bool runtime { get; set; }
        public abstract int groupID { get; }
        public abstract int typeID { get; }
        public int objectID { get; private set; }

        public void setObjectID(int id)
        {
            this.objectID = id;
        }

        public TezItem()
        {
            this.objectID = -1;
            asset = new TezAsset();
        }

        public void serialization(TezJsonWriter writer)
        {
            writer.beginObject("id");
            this.onSerializationID(writer);
            writer.endObject();

            writer.beginObject("asset");
            this.onSerializationAsset(writer);
            writer.endObject();

            writer.beginObject("gamedata");
            this.onSerializationData(writer);
            writer.endObject();
        }

        protected virtual void onSerializationID(TezJsonWriter writer)
        {
            writer.pushValue("runtime", runtime);
            writer.pushValue("group_id", groupID);
            writer.pushValue("type_id", typeID);
            writer.pushValue("object_id", objectID > 0 ? objectID : -1);
        }

        protected virtual void onSerializationAsset(TezJsonWriter writer)
        {
            writer.beginObject("icon");
            writer.pushValue("normal", asset.iconNormal.convertToString());
            writer.pushValue("small", asset.iconSamll.convertToString());
            writer.pushValue("middle", asset.iconMiddle.convertToString());
            writer.pushValue("large", asset.iconLarge.convertToString());
            writer.endObject();
        }

        protected abstract void onSerializationData(TezJsonWriter writer);

        public void deserialization(TezJsonReader reader)
        {
            reader.enter("id");
            this.onDeserializationID(reader);
            reader.exit();

            if (reader.tryEnter("asset"))
            {
                if (reader.tryEnter("icon"))
                {
                    this.onDeserializationAsset(reader);
                    reader.exit();
                }

                reader.exit();
            }

            reader.enter("gamedata");
            this.onDeserializationData(reader);
            reader.exit();
        }

        protected virtual void onDeserializationID(TezJsonReader reader)
        {
            runtime = reader.tryGetBool("runtime", false);
            objectID = reader.tryGetInt("object_id", -1);
        }

        protected virtual void onDeserializationAsset(TezJsonReader reader)
        {
            asset.iconNormal = reader.getString("normal");
            asset.iconSamll = reader.getString("small");
            asset.iconMiddle = reader.getString("middle");
            asset.iconLarge = reader.getString("large");
        }

        protected abstract void onDeserializationData(TezJsonReader reader);


        public virtual void clear()
        {
            asset.clear();
            asset = null;
        }
    }
}