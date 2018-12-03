namespace tezcat.Framework.Core
{
    public interface ITezRealModifier
        : ITezModifier
    {
        float value { get; }
    }

    public class TezRealModifier
        : TezModifier
        , ITezRealModifier
    {
        public virtual float value { get; protected set; }

        public TezRealModifier(float value, ITezValueDescriptor descriptor, ITezModifierOperation operation)
            : base(descriptor, operation)
        {
            this.value = value;
        }

        public override void close()
        {

        }
    }
}