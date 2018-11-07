namespace tezcat.Framework.Core
{
    public enum TezModifierType
    {
        Value,
        Function
    }

    public interface ITezModifier
        : ITezCloseable
    {
        int order { get; }
        object sourceObject { get; }
        ITezModifierOrder modifierOrder { get; }
        TezModifierType modifierType { get; }
        ITezValueName valueName { get; }      
    }

    public abstract class TezModifier : ITezModifier
    {
        public int order
        {
            get { return this.modifierOrder.toID; }
        }

        public object sourceObject { get; }
        public ITezModifierOrder modifierOrder { get; }
        public TezModifierType modifierType { get; }
        public ITezValueName valueName { get; }

        protected TezModifier(ITezValueName value_name, ITezModifierOrder modifier_order, TezModifierType modifier_type, object source_object)
        {
            this.modifierOrder = modifier_order;
            this.sourceObject = source_object;
            this.modifierType = modifier_type;
            this.valueName = value_name;
        }

        public abstract void close();
    }
}