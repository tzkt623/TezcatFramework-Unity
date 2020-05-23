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

    /// <summary>
    /// 
    /// Property
    /// 是一种通过加入Modifier改变当前值的数据类型
    /// 用来制作类似最大血量 力量值这样的参考属性
    /// 
    /// </summary>
    public abstract class TezProperty<T>
        : TezValueWrapper<T>
        , ITezProperty
    {
        public event TezEventExtension.Action<ITezProperty> onValueChanged;

        protected T m_BaseValue = default(T);
        /// <summary>
        /// 基础数值
        /// 可以更改 更改完毕后当前实际数值(value)会重新计算
        /// </summary>
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

        /// <summary>
        /// 当前数值被更改时所保存的旧值
        /// </summary>
        public T oldValue { get; protected set; }

        protected TezValueModifierBaseCache m_ModifierCache = null;
        public override TezWrapperType wrapperType => TezWrapperType.Property;

        protected TezProperty(ITezValueDescriptor name, TezValueModifierBaseCache cache) : base(name)
        {
            m_ModifierCache = cache;
        }

        protected TezProperty(TezValueModifierBaseCache cache) : base()
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

        public override void close(bool self_close)
        {
            base.close(self_close);
            m_ModifierCache.close(!self_close);

            this.onValueChanged = null;
            m_ModifierCache = null;
        }
    }

    public abstract class TezPropertyFloat : TezProperty<float>
    {
        protected float m_Value = 0;

        /// <summary>
        /// 实际数值
        /// 根据basevalue和modifier所计算出的实际作用数值
        /// 他不能被手动更改
        /// </summary>
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

        protected TezPropertyFloat(TezValueModifierBaseCache cache) : base(cache)
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

        /// <summary>
        /// 实际数值
        /// 根据basevalue和modifier所计算出的实际作用数值
        /// 他不能被手动更改
        /// </summary>
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

        protected TezPropertyInt(TezValueModifierBaseCache cache) : base(cache)
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