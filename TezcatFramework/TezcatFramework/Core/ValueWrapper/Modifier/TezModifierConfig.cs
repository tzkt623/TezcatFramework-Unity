using tezcat.Framework.BonusSystem;
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
        , ITezBonusCarrier
    {
        object source { get; set; }
        TezModifierType modifierType { get; }
    }

    public interface ITezValueModifier
        : ITezModifier
        , ITezValueWrapper
    {
        event TezEventExtension.Action<ITezValueModifier, float> onChanged;
        TezValueModifierConfig modifierConfig { get; }
        float value { get; }
    }

    public interface ITezFunctionModifer : ITezModifier
    {

    }
}