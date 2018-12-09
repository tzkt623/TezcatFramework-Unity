using System;
using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 加成合成器
    /// </summary>
    public abstract class TezRealModifierCombiner : ITezCloseable
    {
        public event TezEventExtension.Action onDirty;
        public event TezEventExtension.Action<TezRealModifierCombiner> onRemoveMe;

        List<ITezRealModifier> m_Modifiers = new List<ITezRealModifier>();

        public object onwer { get; set; }
        public ITezValueDescriptor valueName { get; set; }

        /// <summary>
        /// TODO:Remove还没有设计完
        /// </summary>
        int m_Ref = 0;
        public void retain()
        {
            m_Ref += 1;
        }

        public void release()
        {
            m_Ref -= 1;
            if (m_Ref <= 0)
            {
                m_Ref = 0;
                if (m_Modifiers.Count == 0)
                {
                    onRemoveMe(this);
                    onRemoveMe = null;
                }
            }
        }

        public void add(ITezRealModifier modifier)
        {
            m_Modifiers.Add(modifier);
            this.onAdd(modifier);
            onDirty?.Invoke();
        }

        public bool remove(ITezRealModifier modifier)
        {
            if (m_Modifiers.Remove(modifier))
            {
                this.onRemove(modifier);
                onDirty?.Invoke();
                return true;
            }

            return false;
        }

        protected virtual void onAdd(ITezRealModifier modifier)
        {

        }

        protected virtual void onRemove(ITezRealModifier modifier)
        {

        }

        public void close()
        {
            m_Modifiers.Clear();
            onDirty = null;
        }
    }

    public abstract class TezRealModifierContainer : ITezCloseable
    {
        float m_Current = 0;
        protected List<TezRealModifierCombiner> m_Combiners = new List<TezRealModifierCombiner>();

        public bool dirty { get; set; } = true;

        private void onDirty()
        {
            this.dirty = true;
        }

        public void add(TezRealModifierCombiner combiner)
        {
            m_Combiners.Add(combiner);
            combiner.retain();
            combiner.onDirty += onDirty;
            this.dirty = true;
        }

        public bool remove(TezRealModifierCombiner combiner)
        {
            if (m_Combiners.Remove(combiner))
            {
                combiner.onDirty -= onDirty;
                combiner.release();
                this.dirty = true;
                return true;
            }

            return false;
        }

        public float refresh(float basic_value)
        {
            if (this.dirty)
            {
                m_Current = this.calculate(basic_value);
                this.dirty = false;
            }

            return m_Current;
        }

        public virtual void close()
        {
            foreach (var item in m_Combiners)
            {
                item.onDirty -= onDirty;
            }
            m_Combiners.Clear();
            m_Combiners = null;
        }

        protected abstract float calculate(float basic_value);
    }

    public class TezRealModifierList
    {
        public enum RecordMode
        {
            Normal,
            Combine
        }
        RecordMode m_RecordMode = RecordMode.Normal;
        List<ITezRealModifier> m_Modifiers = new List<ITezRealModifier>();
        public bool dirty { get; private set; } = false;

        public TezRealModifierList(RecordMode record_mode)
        {
            m_RecordMode = record_mode;
        }

        public virtual void close()
        {
            m_Modifiers.Clear();
            m_Modifiers = null;
        }

        public void foreachModifier(TezEventExtension.Action<int, ITezRealModifier> function)
        {
            for (int i = 0; i < m_Modifiers.Count; i++)
            {
                function(i, m_Modifiers[i]);
            }
        }

        public void add(ITezRealModifier modifier)
        {
            switch (m_RecordMode)
            {
                case RecordMode.Normal:
                    m_Modifiers.Add(modifier);
                    break;
                case RecordMode.Combine:
                    var combiner = (ITezRealModifierCombiner)m_Modifiers.Find((ITezRealModifier combine_modifier) =>
                    {
                        return combine_modifier.source == null
                            && combine_modifier.operation == modifier.operation;
                    });

                    if (combiner == null)
                    {
                        m_Modifiers.Add(combiner);
                    }

                    combiner.combine(modifier);
                    break;
            }

            this.dirty = true;
        }

        public bool remove(ITezRealModifier modifier)
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
                    var index = m_Modifiers.FindIndex((ITezRealModifier modifier_combiner) =>
                    {
                        return modifier_combiner.source == null
                            && modifier_combiner.operation == modifier.operation;
                    });

                    if (index >= 0)
                    {
                        var combiner = (ITezRealModifierCombiner)m_Modifiers[index];
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
                        if (m_Modifiers[i].source == source)
                        {
                            removed = true;
                            m_Modifiers.RemoveAt(i);
                        }
                    }
                    break;
                case RecordMode.Combine:
                    for (int i = m_Modifiers.Count - 1; i >= 0; i--)
                    {
                        var combiner = (ITezRealModifierCombiner)m_Modifiers[i];
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
            if (a.operationID < b.operationID)
            {
                return -1;
            }
            else if (a.operationID > b.operationID)
            {
                return 1;
            }

            return 0;
        }
    }
}
