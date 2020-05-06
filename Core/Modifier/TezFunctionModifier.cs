namespace tezcat.Framework.Core
{
    public abstract class TezFunctionModifier : ITezFunctionModifer
    {
        public TezModifierType modifierType { get; } = TezModifierType.Function;
        public TezModifierDefinition definition { get; set; } = null;
        public object source { get; set; }

        public abstract void close(bool self_close = true);
    }
}