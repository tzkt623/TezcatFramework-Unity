namespace tezcat.Framework.Core
{
    public interface ITezRVMContainer : ITezCloseable
    {
        bool dirty { get; set; }
        float refresh(float basic_value);

        void addModifier(ITezRealValueModifier modifier);
        bool removeModifier(ITezRealValueModifier modifier);
        bool removeAllModifierFrom(object source);
    }

    public abstract class TezRVMContainer : ITezRVMContainer
    {
        bool m_Dirty = false;
        float m_CurrentValue = float.MinValue;
        protected TezRVMList m_Modifiers = null;

        public virtual bool dirty
        {
            get
            {
                return m_Dirty || m_Modifiers.dirty;
            }
            set
            {
                m_Dirty = value;
            }
        }

        public TezRVMContainer(TezRVMList.RecordMode record_mode = TezRVMList.RecordMode.Normal)
        {
            m_Modifiers = new TezRVMList(record_mode);
        }

        public void addModifier(ITezRealValueModifier modifier)
        {
            m_Modifiers.add(modifier);
        }

        public bool removeModifier(ITezRealValueModifier modifier)
        {
            return m_Modifiers.remove(modifier);
        }

        public bool removeAllModifierFrom(object source)
        {
            return m_Modifiers.removeAll(source);
        }

        public float refresh(float basic_value)
        {
            if (this.dirty)
            {
                this.dirty = false;
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

        public virtual void close()
        {
            m_Modifiers.close();
            m_Modifiers = null;
        }

        protected abstract float recalculate(float basic_value);
    }
}