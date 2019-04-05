using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public abstract class TezModifierCache : ITezCloseable
    {
        public bool dirty { get; set; } = false;
        List<ITezModifier> m_Modifiers = new List<ITezModifier>();

        public void addModifier(ITezModifier modifier)
        {
            this.onModifierAdded(modifier);
            m_Modifiers.Add(modifier);
            modifier.onValueChanged += changeModifier;
            this.dirty = true;
        }

        public void removeModifier(ITezModifier modifier)
        {
            this.onModifierRemoved(modifier);
            m_Modifiers.Remove(modifier);
            modifier.onValueChanged -= changeModifier;
            this.dirty = true;
        }

        public void removeAllModifiersFrom(object source)
        {
            for (int i = m_Modifiers.Count - 1; i >= 0; i--)
            {
                if (m_Modifiers[i].source == source)
                {
                    m_Modifiers[i].onValueChanged -= changeModifier;
                    m_Modifiers.RemoveAt(i);
                    this.dirty = true;
                }
            }
        }

        public void changeModifier(ITezModifier modifier, float old_value)
        {
            this.onModifierChanged(modifier, old_value);
            this.dirty = true;
        }

        public virtual void close()
        {
            foreach (var modifier in m_Modifiers)
            {
                modifier.onValueChanged -= changeModifier;
            }
            m_Modifiers.Clear();
            m_Modifiers = null;
        }

        protected abstract void onModifierAdded(ITezModifier modifier);
        protected abstract void onModifierRemoved(ITezModifier modifier);
        protected abstract void onModifierChanged(ITezModifier modifier, float old_value);

        public abstract float calculate(TezPropertyFloat property);
        public abstract int calculate(TezPropertyInt property);
    }
}