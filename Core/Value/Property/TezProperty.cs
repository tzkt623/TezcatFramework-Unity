using System;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezProperty : ITezValueWrapper
    {
        event TezEventExtension.Action<ITezProperty> onValueChanged;

        void addModifier(ITezValueModifier modifier);
        bool removeModifier(ITezValueModifier modifier);
        void update();
    }

    public abstract class TezProperty<T>
        : TezValueWrapper<T>
        , ITezProperty
    {
        public event TezEventExtension.Action<ITezProperty> onValueChanged;

        protected T m_BaseValue = default(T);
        public virtual T baseValue
        {
            get
            {
                return m_BaseValue;
            }
            set
            {
                m_BaseValue = value;
                this.nodifiyChanged();
            }
        }

        public T oldValue { get; protected set; }

        protected TezValueModifierBaseCache m_ModifierCache = null;
        public override TezValueSubType valueSubType => TezValueSubType.Property;

        protected TezProperty(ITezValueDescriptor name, TezValueModifierBaseCache cache) : base(name)
        {
            m_ModifierCache = cache;
        }

        public void addModifier(ITezValueModifier modifier)
        {
            m_ModifierCache.addModifier(modifier);
        }

        public bool removeModifier(ITezValueModifier modifier)
        {
            return m_ModifierCache.removeModifier(modifier);
        }

        protected virtual void nodifiyChanged()
        {
            this.onValueChanged?.Invoke(this);
        }

        public abstract void update();

        public override void close()
        {
            base.close();
            m_ModifierCache.close();

            this.onValueChanged = null;
            m_ModifierCache = null;
        }
    }

    public abstract class TezPropertyFloat : TezProperty<float>
    {
        protected float m_Value = 0;
        public override float value
        {
            get
            {
                if (m_ModifierCache.dirty)
                {
                    m_ModifierCache.dirty = false;
                    this.nodifiyChanged();
                }
                return m_Value;
            }
            set
            {
//                throw new Exception("TezProperty Can not Set [value], Maybe you want to Set [baseValue]");
            }
        }

        protected TezPropertyFloat(ITezValueDescriptor name, TezValueModifierBaseCache cache) : base(name, cache)
        {

        }

        protected override void nodifiyChanged()
        {
            this.oldValue = m_Value;
            m_Value = m_ModifierCache.calculate(this);
            base.nodifiyChanged();
        }

        public override void update()
        {
            if (m_ModifierCache.dirty)
            {
                m_ModifierCache.dirty = false;
                this.nodifiyChanged();
            }
        }
    }

    public abstract class TezPropertyInt : TezProperty<int>
    {
        protected int m_Value = 0;
        public override int value
        {
            get
            {
                if (m_ModifierCache.dirty)
                {
                    m_ModifierCache.dirty = false;
                    this.nodifiyChanged();
                }
                return m_Value;
            }
            set
            {
//                throw new Exception("TezProperty Can not Set [value], Maybe you want to Set [baseValue]");
            }
        }

        protected TezPropertyInt(ITezValueDescriptor name, TezValueModifierBaseCache cache) : base(name, cache)
        {

        }

        protected override void nodifiyChanged()
        {
            this.oldValue = m_Value;
            m_Value = m_ModifierCache.calculate(this);
            base.nodifiyChanged();
        }

        public override void update()
        {
            if (m_ModifierCache.dirty)
            {
                m_ModifierCache.dirty = false;
                this.nodifiyChanged();
            }
        }
    }
}