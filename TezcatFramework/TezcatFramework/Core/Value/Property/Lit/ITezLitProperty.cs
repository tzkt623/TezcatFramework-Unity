using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezLitProperty : ITezValueWrapper
    {
        event TezEventExtension.Action<ITezLitProperty> onValueChanged;
    }
}