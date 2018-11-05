namespace tezcat.Framework.Core
{
    public interface ITezRealValueModifier
        : ITezModifier
    {
        float value { get; }
        ITezRealValueModifierCombiner createCombiner();
    }

    public class TezRealValueModifier
        : TezModifier
        , ITezRealValueModifier
    {
        public virtual float value { get; protected set; }

        public TezRealValueModifier(float value, ITezValueName value_name, ITezModifierOrder modifier_order, object source_object)
            : base(value_name, modifier_order, TezModifierType.Value, source_object)
        {
            this.value = value;
        }

        public virtual ITezRealValueModifierCombiner createCombiner()
        {
            return new TezRealValueModifierCombiner(this.valueName, this.modifierOrder);
        }

        public override void close()
        {

        }
    }
}