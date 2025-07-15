using tezcat.Framework.Game;

namespace tezcat.Framework.Core
{
    public interface ITezSerializable
    {
        void serialize(TezSaveController.Writer writer);
        void deserialize(TezSaveController.Reader reader);
    }
}