using System;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 
    /// Property
    /// 是一种通过加入Modifier改变当前值的数据类型
    /// 用来制作类似最大血量 力量 敏捷 智力 等这样的参考属性
    /// 
    /// </summary>
    public abstract class TezProperty<T>
        : TezValueWrapper<T>
        , ITezProperty<T>
    {
        public sealed override TezWrapperType wrapperType => TezWrapperType.Property;

        public event TezEventExtension.Action<ITezProperty> evtValueChanged;

        private bool mDirty = false;
        protected T mBaseValue = default(T);
        /// <summary>
        /// 基础数值
        /// 可以更改
        /// 更改后此属性被标记为脏
        /// 可以通过调用update来更新Value以及通知数据变化
        /// </summary>
        public virtual T baseValue
        {
            get
            {
                return mBaseValue;
            }
            set
            {
                mDirty = true;
                mBaseValue = value;
            }
        }

        private T mTrueValue = default(T);

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
                this.calculateValue();
                return mTrueValue;
            }
            set
            {
                throw new Exception("TezProperty Can not Set [value], Maybe you want to Set [baseValue]");
            }
        }

        protected TezBaseValueModifierCache<T> mModifierCache = null;

        /// <summary>
        /// 
        /// </summary>
        protected TezProperty(ITezValueDescriptor name, TezBaseValueModifierCache<T> cache) : base(name)
        {
            mModifierCache = cache;
        }

        /// <summary>
        /// 
        /// </summary>
        protected TezProperty(TezBaseValueModifierCache<T> cache) : base()
        {
            mModifierCache = cache;
        }

        public void addModifier(ITezValueModifier modifier)
        {
            mDirty = true;
            mModifierCache.addModifier(modifier);
            this.onAddModifier(modifier);
        }

        /// <summary>
        /// 加入Modifier之后
        /// </summary>
        protected virtual void onAddModifier(ITezValueModifier modifier) { }

        public bool removeModifier(ITezValueModifier modifier)
        {
            if (mModifierCache.removeModifier(modifier))
            {
                mDirty = true;
                this.onRemoveModifier(modifier);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 成功删除modifier之后
        /// </summary>
        protected virtual void onRemoveModifier(ITezValueModifier modifier) { }

        public bool removeAllModifiersFrom(object obj)
        {
            if (mModifierCache.removeAllModifiersFrom(obj))
            {
                mDirty = true;
                return true;
            }

            return false;
        }

        public void clearModifiers()
        {
            mDirty = true;
            mModifierCache.clearModifiers();
        }

        private void calculateValue()
        {
            if (mDirty)
            {
                mDirty = false;
                mTrueValue = mModifierCache.calculate(this);
            }
        }

        /// <summary>
        /// 只有数据需要重新计算时,才会重新计算
        /// 但一定会发出更新事件
        /// </summary>
        public void update()
        {
            this.calculateValue();
            this.evtValueChanged?.Invoke(this);
        }


        protected override void onClose()
        {
            ///这里不适用基类方法是因为
            ///在Property中
            ///Value不可以被Set
            mModifierCache.close();
            mTrueValue = default;
            mBaseValue = default;
            mDirty = false;

            mModifierCache = null;
            this.descriptor = null;
            this.evtValueChanged = null;
        }

        public virtual string baseValueToString()
        {
            return mBaseValue.ToString();
        }
    }
}