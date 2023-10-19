namespace tezcat.Framework.Database
{
    public interface ITezSerializable
    {
        void serialize(TezWriter writer);
        void deserialize(TezReader reader);
    }
}