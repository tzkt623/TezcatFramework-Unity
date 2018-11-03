using System;
using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public interface ITezRealValueModifierCombiner : ITezRealValueModifier
    {
        bool empty { get; }
        void combine(ITezRealValueModifier modifier);
        bool separate(ITezRealValueModifier modifier);
        bool separate(object source);
    }

    public class TezRealValueModifierCombiner
        : TezRealValueModifier
        , ITezRealValueModifierCombiner
    {
        List<ITezRealValueModifier> m_CombineOwner = new List<ITezRealValueModifier>();

        public bool empty
        {
            get { return m_CombineOwner.Count == 0; }
        }

        public TezRealValueModifierCombiner(ITezValueName value_name, ITezModifierOrder modifier_order)
            : base(0, value_name, modifier_order, null)
        {

        }

        public override void close()
        {
            base.close();
            m_CombineOwner.Clear();
            m_CombineOwner = null;
        }

        public void combine(ITezRealValueModifier modifier)
        {
            m_CombineOwner.Add(modifier);
            this.onCombine(modifier);
        }

        protected virtual void onCombine(ITezRealValueModifier modifier)
        {
            this.value += modifier.value;
        }

        public bool separate(ITezRealValueModifier modifier)
        {
            var index = m_CombineOwner.FindIndex((ITezRealValueModifier inner_modifier) =>
            {
                return inner_modifier == modifier;
            });

            if (index >= 0)
            {
                m_CombineOwner.RemoveAt(index);
                this.onSeparate(modifier);
                return true;
            }

            return false;
        }

        protected virtual void onSeparate(ITezRealValueModifier modifier)
        {
            this.value -= modifier.value;
        }

        public bool separate(object source)
        {
            var index = m_CombineOwner.FindIndex((ITezRealValueModifier inner_modifier) =>
            {
                return inner_modifier.sourceObject == source;
            });

            if (index >= 0)
            {
                var modifier = m_CombineOwner[index];
                m_CombineOwner.RemoveAt(index);
                this.onSeparate(modifier);
                return true;
            }

            return false;
        }

        public override ITezRealValueModifierCombiner createCombiner()
        {
            throw new Exception("ModifierCombiner Can not invoke this method!!");
        }
    }

}