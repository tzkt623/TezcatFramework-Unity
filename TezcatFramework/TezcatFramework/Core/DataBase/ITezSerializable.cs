namespace tezcat.Framework.Core
{
    public interface ITezSerializable
    {
        void serialize(TezWriter writer);
        void deserialize(TezReader reader);
    }
}