namespace tezcat.Serialization
{
    public interface ITezSerializable
    {
        void serialization(TezWriter writer);
        void deserialization(TezReader reader);
    }
}