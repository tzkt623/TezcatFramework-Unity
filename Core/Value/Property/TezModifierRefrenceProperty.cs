namespace tezcat.Framework.Core
{
    public interface ITezModifierRefrenceProperty : ITezProperty
    {
        TezValueModifierRefrence modifierRefrence { get; }
        void createRefrence(ITezValueDescriptor descriptor, TezValueModifierDefinition definition, object source);
    }

    public abstract class TezMRPropertyInt
        : TezPropertyInt
        , ITezModifierRefrenceProperty
    {
        public TezValueModifierRefrence modifierRefrence { get; protected set; }
        public override TezValueSubType valueSubType => TezValueSubType.MRProperty;


        protected TezMRPropertyInt(ITezValueDescriptor name, TezValueModifierBaseCache cache)
            : base(name, cache)
        {

        }

        public abstract void createRefrence(ITezValueDescriptor descriptor, TezValueModifierDefinition definition, object source);

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
        public TezValueModifierRefrence modifierRefrence { get; protected set; }
        public override TezValueSubType valueSubType => TezValueSubType.MRProperty;

        protected TezMRPropertyFloat(ITezValueDescriptor name, TezValueModifierBaseCache cache)
            : base(name, cache)
        {
        }

        public abstract void createRefrence(ITezValueDescriptor descriptor, TezValueModifierDefinition definition, object source);

        public override void close()
        {
            base.close();

            this.modifierRefrence.close();
            this.modifierRefrence = null;
        }
    }
}