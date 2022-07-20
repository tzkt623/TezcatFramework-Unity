using tezcat.Framework.BonusSystem;

namespace tezcat.Framework.Core
{
    /*
    public abstract class TezMRProperty<T>
        : TezProperty<T>
        , ITezModifierRefrenceProperty
    {
        public TezAgentValueModifier modifierRefrence { get; protected set; }
        public override TezWrapperType wrapperType => TezWrapperType.MRProperty;

        public bool allowCloseRef { get; private set; } = false;

        protected TezMRProperty(ITezValueDescriptor name, TezBaseValueModifierCache<T> cache)
            : base(name, cache)
        {

        }

        public override void close()
        {
            base.close();

            this.allowCloseRef = true;
            this.modifierRefrence.close();
            this.modifierRefrence = null;
        }

        public void createRefrence(TezBonusPath definition, TezValueModifierConfig modifierConfig, object source)
        {
            if (this.modifierRefrence == null)
            {
                this.allowCloseRef = false;
                this.modifierRefrence = new TezAgentValueModifier(this)
                {
                    modifierConfig = modifierConfig,
                    bonusPath = definition,
                    source = source
                };
            }
        }
    }
    */
}