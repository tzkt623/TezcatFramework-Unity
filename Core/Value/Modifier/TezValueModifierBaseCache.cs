﻿using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public interface ITezModifierCache
    {

    }

    public abstract class TezValueModifierBaseCache : ITezCloseable
    {
        public bool dirty { get; set; } = false;
        List<ITezValueModifier> m_Modifiers = new List<ITezValueModifier>();

        public void addModifier(ITezValueModifier modifier)
        {
            this.onModifierAdded(modifier);
            m_Modifiers.Add(modifier);
            modifier.onValueChanged += changeModifier;
            this.dirty = true;
        }

        public bool removeModifier(ITezValueModifier modifier)
        {
            if (m_Modifiers.Remove(modifier))
            {
                this.onModifierRemoved(modifier);
                modifier.onValueChanged -= changeModifier;
                this.dirty = true;
                return true;
            }
            return false;
        }

        private void onRemoveModifier(ITezValueModifier modifier)
        {
            this.removeModifier(modifier);
        }

        public bool removeAllModifiersFrom(object source)
        {
            bool flag = false;
            for (int i = m_Modifiers.Count - 1; i >= 0; i--)
            {
                if (m_Modifiers[i].source == source)
                {
                    m_Modifiers[i].onValueChanged -= changeModifier;
                    m_Modifiers.RemoveAt(i);
                    flag = true;
                    this.dirty = true;
                }
            }

            return flag;
        }

        public void changeModifier(ITezValueModifier modifier, float old_value)
        {
            this.onModifierChanged(modifier, old_value);
            this.dirty = true;
        }

        public virtual void clear()
        {
            foreach (var modifier in m_Modifiers)
            {
                modifier.onValueChanged -= changeModifier;
            }
            m_Modifiers.Clear();
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

        protected abstract void onModifierAdded(ITezValueModifier modifier);
        protected abstract void onModifierRemoved(ITezValueModifier modifier);
        protected abstract void onModifierChanged(ITezValueModifier modifier, float old_value);

        public abstract float calculate(TezPropertyFloat property);
        public abstract int calculate(TezPropertyInt property);
    }
}