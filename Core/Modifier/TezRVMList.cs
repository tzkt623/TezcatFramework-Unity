using System;
using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public class TezRVMList
    {
        public enum RecordMode
        {
            Normal,
            Combine
        }
        RecordMode m_RecordMode = RecordMode.Normal;
        List<ITezRealValueModifier> m_Modifiers = new List<ITezRealValueModifier>();
        public bool dirty { get; private set; } = false;

        public TezRVMList(RecordMode record_mode)
        {
            m_RecordMode = record_mode;
        }

        public virtual void close()
        {
            m_Modifiers.Clear();
            m_Modifiers = null;
        }

        public void foreachModifier(TezEventExtension.Action<int, ITezRealValueModifier> function)
        {
            for (int i = 0; i < m_Modifiers.Count; i++)
            {
                function(i, m_Modifiers[i]);
            }
        }

        public void add(ITezRealValueModifier modifier)
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

            this.dirty = true;
        }

        public bool remove(ITezRealValueModifier modifier)
        {
            bool removed = false;
            switch (m_RecordMode)
            {
                case RecordMode.Normal:
                    if (m_Modifiers.Remove(modifier))
                    {
                        removed = true;
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
                            removed = true;

                            if (combiner.empty)
                            {
                                m_Modifiers.RemoveAt(index);
                            }
                        }
                    }
                    break;
            }

            this.dirty = removed;
            return removed;
        }

        public bool removeAll(object source)
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

            this.dirty = removed;
            return removed;
        }

        public void sort()
        {
            if (this.dirty)
            {
                this.dirty = false;
                m_Modifiers.Sort(this.sort);
            }
        }

        public void sort(Comparison<ITezModifier> comparison)
        {
            if (this.dirty)
            {
                this.dirty = false;
                m_Modifiers.Sort(comparison);
            }
        }

        public void clear()
        {
            m_Modifiers.Clear();
        }

        private int sort(ITezModifier a, ITezModifier b)
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
    }
}
