using tezcat.Framework.Definition;

namespace tezcat.Framework.Core
{
    public abstract class TezFunctionModifier : ITezFunctionModifer
    {
        public TezModifierType modifierType { get; } = TezModifierType.Function;
        public TezDefinition definition { get; set; } = null;
        public object source { get; set; }

        public abstract void close();
    }
}