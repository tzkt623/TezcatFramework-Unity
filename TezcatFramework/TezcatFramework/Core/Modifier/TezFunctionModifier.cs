using tezcat.Framework.Attribute;

namespace tezcat.Framework.Core
{
    public abstract class TezFunctionModifier : ITezFunctionModifer
    {
        public TezModifierType modifierType { get; } = TezModifierType.Function;
        public TezAttributeDef definition { get; set; } = null;
        public object source { get; set; }

        public abstract void close();
    }
}