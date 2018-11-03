using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public interface ITezRealValueModifierCollection : ITezCloseable
    {
        float refresh(float basic_value);
        void setDirty();
        void addModifier(ITezRealValueModifier modifier);
        bool removeModifier(ITezRealValueModifier modifier);
        bool removeAllModifierFrom(object source);
    }

    public abstract class TezRealValueModifierCollection : ITezRealValueModifierCollection
    {
        public enum RecordMode
        {
            Normal,
            Combine
        }

        bool m_Dirty = false;
        float m_CurrentValue = 0;
        RecordMode m_RecordMode = RecordMode.Normal;
        protected List<ITezRealValueModifier> m_Modifiers = new List<ITezRealValueModifier>();

        public TezRealValueModifierCollection(RecordMode record_mode = RecordMode.Combine)
        {
            m_RecordMode = record_mode;
        }

        public virtual void close()
        {
            m_Modifiers.Clear();
            m_Modifiers = null;
        }

        public void setDirty()
        {
            m_Dirty = true;
        }

        public void addModifier(ITezRealValueModifier modifier)
        {
            switch (m_RecordMode)
            {
                case RecordMode.Normal:
                    m_Modifiers.Add(modifier);
                    break;
                case RecordMode.Combine:
                    var combiner = (ITezRealValueModifierCombiner)m_Modifiers.Find((ITezRealValueModifier combine_modifier) =>
                    {
                        return combine_modifier.sourceObject == null
                            && combine_modifier.modifierType == modifier.modifierType
                            && combine_modifier.modifierOrder == modifier.modifierOrder;
                    });

                    if (combiner == null)
                    {
                        combiner = modifier.createCombiner();
                        m_Modifiers.Add(combiner);
                    }

                    combiner.combine(modifier);
                    break;
            }

            m_Dirty = true;
        }

        public bool removeModifier(ITezRealValueModifier modifier)
        {
            bool result = false;
            switch (m_RecordMode)
            {
                case RecordMode.Normal:
                    if (m_Modifiers.Remove(modifier))
                    {
                        m_Dirty = true;
                        result = true;
                    }
                    break;
                case RecordMode.Combine:
                    var index = m_Modifiers.FindIndex((ITezRealValueModifier modifier_combiner) =>
                    {
                        return modifier_combiner.sourceObject == null
                            && modifier_combiner.modifierType == modifier.modifierType
                            && modifier_combiner.modifierOrder == modifier.modifierOrder;
                    });

                    if (index >= 0)
                    {
                        var combiner = (ITezRealValueModifierCombiner)m_Modifiers[index];
                        if (combiner.separate(modifier))
                        {
                            m_Dirty = true;
                            result = true;
                            if (combiner.empty)
                            {
                                m_Modifiers.RemoveAt(index);
                            }
                        }
                    }
                    break;
            }

            return result;
        }

        public bool removeAllModifierFrom(object source)
        {
            bool removed = false;

            switch (m_RecordMode)
            {
                case RecordMode.Normal:
                    for (int i = m_Modifiers.Count - 1; i >= 0; i--)
                    {
                        if (m_Modifiers[i].sourceObject == source)
                        {
                            removed = true;
                            m_Modifiers.RemoveAt(i);
                        }
                    }
                    break;
                case RecordMode.Combine:
                    for (int i = m_Modifiers.Count - 1; i >= 0; i--)
                    {
                        var combiner = (ITezRealValueModifierCombiner)m_Modifiers[i];
                        if (combiner.separate(source))
                        {
                            removed = true;
                            if (combiner.empty)
                            {
                                m_Modifiers.RemoveAt(i);
                            }
                        }
                    }
                    break;
            }

            m_Dirty = removed;
            return removed;
        }

        public void sort()
        {
            m_Modifiers.Sort();
        }

        public float refresh(float basic_value)
        {
            if (m_Dirty)
            {
                m_Dirty = false;
                m_Modifiers.Sort(this.sortModifiers);
                m_CurrentValue = this.recalculate(basic_value);
            }

            return m_CurrentValue;
        }

        protected virtual int sortModifiers(ITezModifier a, ITezModifier b)
        {
            if (a.order < b.order)
            {
                return -1;
            }
            else if (a.order > b.order)
            {
                return 1;
            }

            return 0;
        }

        protected abstract float recalculate(float basic_value);
    }
}