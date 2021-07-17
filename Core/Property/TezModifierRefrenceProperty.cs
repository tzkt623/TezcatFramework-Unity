using tezcat.Framework.Definition;

namespace tezcat.Framework.Core
{
    public interface ITezModifierRefrenceProperty : ITezProperty
    {
        /// <summary>
        /// 只能由MR来释放Ref
        /// 其他地方不能释放
        /// </summary>
        bool allowCloseRef { get; }
        TezValueModifierRefrence modifierRefrence { get; }
        void createRefrence(TezDefinition definition, TezValueModifierConfig modifierConfig, object source);
    }

    public abstract class TezMRProperty<T>
        : TezProperty<T>
        , ITezModifierRefrenceProperty
    {
        public TezValueModifierRefrence modifierRefrence { get; protected set; }
        public override TezWrapperType wrapperType => TezWrapperType.MRProperty;

        public bool allowCloseRef { get; private set; } = false;

        protected TezMRProperty(ITezValueDescriptor name, TezValueModifierBaseCache<T> cache)
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

        public void createRefrence(TezDefinition definition, TezValueModifierConfig modifierConfig, object source)
        {
            if (this.modifierRefrence == null)
            {
                this.allowCloseRef = false;
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