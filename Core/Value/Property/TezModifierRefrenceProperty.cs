namespace tezcat.Framework.Core
{
    public interface ITezModifierRefrenceProperty : ITezProperty
    {
        TezModifierRefrence modifierRefrence { get; }
        void createRefrence(TezModifierDefinition definition);
    }

    public abstract class TezMRPropertyInt
        : TezPropertyInt
        , ITezModifierRefrenceProperty
    {
        public TezModifierRefrence modifierRefrence { get; protected set; }

        protected TezMRPropertyInt(ITezValueDescriptor name, TezModifierCache cache)
            : base(name, cache)
        {

        }

        public abstract void createRefrence(TezModifierDefinition definition);

        public override void close()
        {
            base.close();

            this.modifierRefrence.close();
            this.modifierRefrence = null;
        }
    }

    public abstract class TezMRPropertyFloat
        : TezPropertyFloat
        , ITezModifierRefrenceProperty
    {
        public TezModifierRefrence modifierRefrence { get; protected set; }

        protected TezMRPropertyFloat(ITezValueDescriptor name, TezModifierCache cache)
            : base(name, cache)
        {
        }

        public abstract void createRefrence(TezModifierDefinition definition);

        public override void close()
        {
            base.close();

            this.modifierRefrence.close();
            this.modifierRefrence = null;
        }
    }
}