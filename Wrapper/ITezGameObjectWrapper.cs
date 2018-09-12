using tezcat.Core;

namespace tezcat.Wrapper
{
    public interface ITezGameObjectWrapper : ITezWrapper
    {
        TezGameObject myObject { get; }
    }
}