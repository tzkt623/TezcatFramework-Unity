using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public abstract class TezModifierBaseCache : ITezCloseable
    {
        public bool dirty { get; set; } = false;
        List<ITezModifier> m_Modifiers = new List<ITezModifier>();

        public void addModifier(ITezModifier modifier)
        {
            this.dirty = true;
            this.onModifierAdded(modifier);
            m_Modifiers.Add(modifier);
        }

        public bool removeModifier(ITezModifier modifier)
        {
            if (m_Modifiers.Remove(modifier))
            {
                this.onModifierRemoved(modifier);
                this.dirty = true;
                return true;
            }

            return false;
        }

        public bool removeAllModifiersFrom(object source)
        {
            bool flag = false;
            for (int i = m_Modifiers.Count - 1; i >= 0; i--)
            {
                if (m_Modifiers[i].source == source)
                {
                    this.onModifierRemoved(m_Modifiers[i]);
                    m_Modifiers.RemoveAt(i);
                    flag = true;
                }
            }

            if (flag)
            {
                this.dirty = flag;
            }

            return flag;
        }

        public virtual void clear()
        {
            foreach (var modifier in m_Modifiers)
            {
                this.onModifierRemoved(modifier);
            }
            m_Modifiers.Clear();
            this.dirty = true;
        }

        public virtual void close()
        {
            foreach (var modifier in m_Modifiers)
            {
                this.onModifierRemoved(modifier);
            }
            m_Modifiers.Clear();
            m_Modifiers = null;
        }

        protected abstract void onModifierAdded(ITezModifier modifier);
        protected abstract void onModifierRemoved(ITezModifier modifier);
    }

    public abstract class TezFunctionModifierBaseCache : TezModifierBaseCache
    {

    }

    public abstract class TezValueModifierBaseCache<T> : TezModifierBaseCache
    {
        protected sealed override void onModifierAdded(ITezModifier modifier)
        {
            ITezValueModifier vm = (ITezValueModifier)modifier;
            this.onModifierAdded(vm);
            vm.onValueChanged += changeModifier;
        }

        protected sealed override void onModifierRemoved(ITezModifier modifier)
        {
            ITezValueModifier vm = (ITezValueModifier)modifier;
            this.onModifierRemoved(vm);
            vm.onValueChanged -= changeModifier;
        }

        public void changeModifier(ITezValueModifier modifier, float old_value)
        {
            this.onModifierChanged(modifier, old_value);
            this.dirty = true;
        }

        protected abstract void onModifierAdded(ITezValueModifier modifier);
        protected abstract void onModifierRemoved(ITezValueModifier modifier);
        protected abstract void onModifierChanged(ITezValueModifier modifier, float old_value);

        public abstract T calculate(ITezProperty<T> property);
    }
}