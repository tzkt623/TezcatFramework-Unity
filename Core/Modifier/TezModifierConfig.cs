using tezcat.Framework.Definition;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public enum TezModifierType
    {
        Value,
        Function
    }

    public interface ITezModifier
        : ITezCloseable
        , ITezDefinitionObject
    {
        TezModifierType modifierType { get; }
        object source { get; set; }
    }

    public interface ITezValueModifier
        : ITezModifier
        , ITezValueWrapper
    {
        event TezEventExtension.Action<ITezValueModifier, float> onValueChanged;
        float value { get; }
    }

    public interface ITezFunctionModifer : ITezModifier
    {

    }
}