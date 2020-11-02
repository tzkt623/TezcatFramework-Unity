using System;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezProperty : ITezValueWrapper
    {
        event TezEventExtension.Action<ITezProperty> onValueChanged;

        void addModifier(ITezValueModifier modifier);
        bool removeModifier(ITezValueModifier modifier);

        /// <summary>
        /// 只有数据需要重新计算时,才会重新计算
        /// 但一定会发出更新事件
        /// </summary>
        void update();
    }

    public interface ITezProperty<T> : ITezProperty
    {
        T baseValue { get; }
        T value { get; }
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
        , ITezProperty<T>
    {
        public event TezEventExtension.Action<ITezProperty> onValueChanged;

        protected T m_BaseValue = default(T);
        /// <summary>
        /// 基础数值
        /// 可以更改
        /// 更改后此属性被标记为脏
        /// 可以通过直接获得Value来获得最新数据
        /// 也可以通过调用update来更新以及通知数据变化
        /// </summary>
        public virtual T baseValue
        {
            get
            {
                return m_BaseValue;
            }
            set
            {
                m_ModifierCache.dirty = true;
                m_BaseValue = value;
            }
        }

        /// <summary>
        /// 当前数值被更改时所保存的旧值
        /// </summary>
        public T oldValue { get; protected set; }


        private T m_Value;
        /// <summary>
        /// 实际数值
        /// 根据basevalue和modifier所计算出的实际作用数值
        /// 他不能被手动更改
        /// 调用此函数不会通知更新
        /// </summary>
        public override T value
        {
            get
            {
                if (m_ModifierCache.dirty)
                {
                    m_ModifierCache.dirty = false;
                    this.calculateValue();
                }
                return m_Value;
            }
            set
            {
                throw new Exception("TezProperty Can not Set [value], Maybe you want to Set [baseValue]");
            }
        }

        protected TezValueModifierBaseCache<T> m_ModifierCache = null;
        public override TezWrapperType wrapperType => TezWrapperType.Property;

        protected TezProperty(ITezValueDescriptor name, TezValueModifierBaseCache<T> cache) : base(name)
        {
            m_ModifierCache = cache;
        }

        protected TezProperty(TezValueModifierBaseCache<T> cache) : base()
        {
            m_ModifierCache = cache;
        }

        public void addModifier(ITezValueModifier modifier)
        {
            m_ModifierCache.addModifier(modifier);
            this.afterAddModifier(modifier);
        }

        /// <summary>
        /// 加入Modifier之后
        /// </summary>
        protected virtual void afterAddModifier(ITezValueModifier modifier) { }

        public bool removeModifier(ITezValueModifier modifier)
        {
            var flag = m_ModifierCache.removeModifier(modifier);
            if (flag)
            {
                this.afterRemoveModifier(modifier);
            }
            return flag;
        }

        /// <summary>
        /// 成功删除modifier之后
        /// </summary>
        protected virtual void afterRemoveModifier(ITezValueModifier modifier) { }

        private void calculateValue()
        {
            this.oldValue = m_Value;
            m_Value = m_ModifierCache.calculate(this);
        }

        /// <summary>
        /// 只有数据需要重新计算时,才会重新计算
        /// 但一定会发出更新事件
        /// </summary>
        public void update()
        {
            if (m_ModifierCache.dirty)
            {
                m_ModifierCache.dirty = false;
                this.calculateValue();
            }
            this.onValueChanged?.Invoke(this);
        }


        public override void close(bool self_close)
        {
            ///这里不适用基类方法是因为
            ///在Property中
            ///Value不可以被Set
            this.descriptor = null;
            m_ModifierCache.close(!self_close);

            this.onValueChanged = null;
            m_ModifierCache = null;
        }
    }
}