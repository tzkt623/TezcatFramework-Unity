using tezcat.Framework.Attribute;
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
        , ITezAttributeDefObject
    {
        object source { get; set; }
        TezModifierType modifierType { get; }
    }

    public interface ITezValueModifier
        : ITezModifier
        , ITezValueWrapper
    {
        event TezEventExtension.Action<ITezValueModifier, float> onValueChanged;
        TezValueModifierConfig modifierConfig { get; }
        float value { get; }
    }

    public interface ITezFunctionModifer : ITezModifier
    {

    }
}