using tezcat.Core;

namespace tezcat.Wrapper
{
    public interface ITezWrapper : ITezCloseable
    {
        string myName { get; }
        string myDescription { get; }
    }
}