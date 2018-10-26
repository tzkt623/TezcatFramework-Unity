using tezcat.Framework.Core;

namespace tezcat.Framework.DataBase
{
    public interface ITezSerializable
    {
        void serialize(TezSaveManager manager);
        void deserialize(TezSaveManager manager);
    }

    public interface ITezSerializableItem
    {
        void serialize(TezWriter writer);
        void deserialize(TezReader reader);
    }
}