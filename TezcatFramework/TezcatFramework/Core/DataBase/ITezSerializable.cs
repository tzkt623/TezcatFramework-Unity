using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    public interface ITezSerializable
    {
 //       string NID { get; }
        void serialize(TezWriter writer);
        void deserialize(TezReader reader);
    }
}