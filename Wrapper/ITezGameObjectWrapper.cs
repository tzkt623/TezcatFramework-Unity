using tezcat.Framework.Core;

namespace tezcat.Framework.Wrapper
{
    public interface ITezGameObjectWrapper : ITezWrapper
    {
        TezGameObject getObject();
    }
}