using tezcat.Framework.Core;

namespace tezcat.Framework.Wrapper
{
    public interface ITezWrapper : ITezCloseable
    {
        string myName { get; }
        string myDescription { get; }
    }
}