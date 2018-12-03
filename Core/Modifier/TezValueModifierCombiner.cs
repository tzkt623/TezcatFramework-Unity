using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public interface ITezRealModifierCombiner : ITezRealModifier
    {
        bool empty { get; }
        void combine(ITezRealModifier modifier);
        bool separate(ITezRealModifier modifier);
        bool separate(object source);
    }

    public class TezRealValueModifierCombiner
        : TezRealModifier
        , ITezRealModifierCombiner
    {
        List<ITezRealModifier> m_CombineOwner = new List<ITezRealModifier>();

        public bool empty
        {
            get { return m_CombineOwner.Count == 0; }
        }

        public TezRealValueModifierCombiner(ITezValueDescriptor value_name, ITezModifierOperation modifier_order)
            : base(0, value_name, modifier_order)
        {

        }

        public override void close()
        {
            base.close();
            m_CombineOwner.Clear();
            m_CombineOwner = null;
        }

        public void combine(ITezRealModifier modifier)
        {
            m_CombineOwner.Add(modifier);
            this.onCombine(modifier);
        }

        protected virtual void onCombine(ITezRealModifier modifier)
        {
            this.value += modifier.value;
        }

        public bool separate(ITezRealModifier modifier)
        {
            var index = m_CombineOwner.FindIndex((ITezRealModifier inner_modifier) =>
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

        protected virtual void onSeparate(ITezRealModifier modifier)
        {
            this.value -= modifier.value;
        }

        public bool separate(object source)
        {
            var index = m_CombineOwner.FindIndex((ITezRealModifier inner_modifier) =>
            {
                return inner_modifier.source == source;
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
    }

}