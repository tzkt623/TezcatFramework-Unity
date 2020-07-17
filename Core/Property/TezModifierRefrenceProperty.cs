﻿using tezcat.Framework.Definition;

namespace tezcat.Framework.Core
{
    public interface ITezModifierRefrenceProperty : ITezProperty
    {
        TezValueModifierRefrence modifierRefrence { get; }
        void createRefrence(ITezValueDescriptor descriptor, TezDefinition definition, TezValueModifierConfig modifierConfig, object source);
    }

    public abstract class TezMRPropertyInt
        : TezPropertyInt
        , ITezModifierRefrenceProperty
    {
        public TezValueModifierRefrence modifierRefrence { get; protected set; }
        public override TezWrapperType wrapperType => TezWrapperType.MRProperty;


        protected TezMRPropertyInt(ITezValueDescriptor name, TezValueModifierBaseCache cache)
            : base(name, cache)
        {

        }

        public override void close(bool self_close = true)
        {
            base.close(self_close);

            this.modifierRefrence.close(false);
            this.modifierRefrence = null;
        }

        public abstract void createRefrence(ITezValueDescriptor descriptor, TezDefinition definition, TezValueModifierConfig modifierConfig, object source);
    }

    public abstract class TezMRPropertyFloat
        : TezPropertyFloat
        , ITezModifierRefrenceProperty
    {
        public TezValueModifierRefrence modifierRefrence { get; protected set; }
        public override TezWrapperType wrapperType => TezWrapperType.MRProperty;

        protected TezMRPropertyFloat(ITezValueDescriptor name, TezValueModifierBaseCache cache)
            : base(name, cache)
        {
        }

        public override void close(bool self_close = true)
        {
            base.close(self_close);

            this.modifierRefrence.close(false);
            this.modifierRefrence = null;
        }

        public abstract void createRefrence(ITezValueDescriptor descriptor, TezDefinition definition, TezValueModifierConfig modifierConfig, object source);
    }
}