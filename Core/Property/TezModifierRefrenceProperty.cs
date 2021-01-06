using tezcat.Framework.Definition;

namespace tezcat.Framework.Core
{
    public interface ITezModifierRefrenceProperty : ITezProperty
    {
        TezValueModifierRefrence modifierRefrence { get; }
        void createRefrence(TezDefinition definition, TezValueModifierConfig modifierConfig, object source);
    }

    public abstract class TezMRProperty<T>
        : TezProperty<T>
        , ITezModifierRefrenceProperty
    {
        public TezValueModifierRefrence modifierRefrence { get; protected set; }
        public override TezWrapperType wrapperType => TezWrapperType.MRProperty;


        protected TezMRProperty(ITezValueDescriptor name, TezValueModifierBaseCache<T> cache)
            : base(name, cache)
        {

        }

        public override void close()
        {
            base.close();

            this.modifierRefrence.close();
            this.modifierRefrence = null;
        }

        public void createRefrence(TezDefinition definition, TezValueModifierConfig modifierConfig, object source)
        {
            if(this.modifierRefrence == null)
            {
                this.modifierRefrence = new TezValueModifierRefrence(this)
                {
                    modifierConfig = modifierConfig,
                    definition = definition,
                    source = source
                };
            }
        }
    }
}