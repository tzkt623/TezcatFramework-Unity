namespace tezcat.Framework.Core
{
    public interface ITezProtoLoader
    {
        void loadProtoData(TezSaveController.Reader reader);
    }

    public interface ITezSerializable
    {
        void serialize(TezSaveController.Writer writer);
        void deserialize(TezSaveController.Reader reader);
    }
}