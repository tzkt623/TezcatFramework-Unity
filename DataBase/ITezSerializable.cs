namespace tezcat.DataBase
{
    public interface ITezSerializable
    {
        void serialize(TezWriter writer);
        void deserialize(TezReader reader);
    }
}