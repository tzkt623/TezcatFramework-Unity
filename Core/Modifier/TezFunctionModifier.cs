namespace tezcat.Framework.Core
{
    public abstract class TezFunctionModifier : TezModifier
    {
        public object function { get; protected set; }

        protected TezFunctionModifier(object function, ITezValueName value_name, ITezModifierOrder modifier_order, object source_object)
            : base(value_name, modifier_order, TezModifierType.Function, source_object)
        {
            this.function = function;
        }

        public override void close()
        {
            this.function = null;
        }
    }
}