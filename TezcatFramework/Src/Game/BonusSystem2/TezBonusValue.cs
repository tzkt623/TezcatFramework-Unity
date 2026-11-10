using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// BonusValue不能主动删除modifier
    /// 但是可以选中自己的modifier让他自己删除
    /// </summary>
    public interface ITezBonusValue
        : ITezValueWrapper
        , ITezSerializable
    {
        ITezBonusModifierContainer modifierContainer { get; }
        void manualUpdate();

        void setDirty();
        void addModifier(ITezBonusModifier bonusModifier);
        //void removeModifier(ITezBonusModifier bonusModifier);
    }

    public interface ITezBonusValue<T> : ITezBonusValue
    {
        event System.Action<T> evtValueChanged;

        /// <summary>
        /// 实际数值
        /// </summary>
        T value { get; }

        /// <summary>
        /// 基础数值
        /// </summary>
        T baseValue { get; set; }
    }

    public abstract class TezBonusValue<T>
        : TezValueWrapper<T>
        , ITezBonusValue<T>
    {
        public event System.Action<T> evtValueChanged;
        public sealed override TezWrapperType wrapperType => TezWrapperType.Bonusable;

        protected ITezBonusModifierContainer mModifierContainer = null;
        public ITezBonusModifierContainer modifierContainer => mModifierContainer;

        protected T mBaseValue = default;
        public T baseValue
        {
            get { return mBaseValue; }
            set
            {
                mModifierContainer.setDirty();
                mBaseValue = value;
            }
        }

        protected T mValue = default;
        public override T value
        {
            get
            {
                if (mModifierContainer.isDirty)
                {
                    mValue = this.calculateValue();
                    this.evtValueChanged?.Invoke(mValue);
                }
                return mValue;
            }

            set
            {
                throw new Exception();
            }
        }

        public TezBonusValue(ITezBonusModifierContainer container, ITezValueDescriptor valueDescriptor) : base(valueDescriptor)
        {
            mModifierContainer = container;
        }

        public TezBonusValue(ITezBonusModifierContainer container)
        {
            mModifierContainer = container;
        }

        protected abstract T calculateValue();

        public void setDirty()
        {
            mModifierContainer.setDirty();
        }

        public void addModifier(ITezBonusModifier bonusModifier)
        {
            mModifierContainer.addModifier(bonusModifier);
        }

        /// <summary>
        /// 一个Value删除Modifier的情况有
        /// 
        /// 1.自己关闭了,需要通知modifier关闭自己
        /// 2.modifier关闭了,通知value删除自己
        /// 
        /// </summary>
//          public void removeModifier(ITezBonusModifier bonusModifier)
//          {
//              mModifierContainer.removeModifier(bonusModifier);
//          }

        protected override void onClose()
        {
            base.onClose();
            mModifierContainer.close();
            mModifierContainer = null;
            mDescriptor = null;

            this.evtValueChanged = null;
        }

        public void manualUpdate()
        {
            if (mModifierContainer.isDirty)
            {
                mValue = this.calculateValue();
            }

            this.evtValueChanged?.Invoke(mValue);
        }

        public abstract void serialize(TezSaveController.Writer writer);
        public abstract void deserialize(TezSaveController.Reader reader);

        public override string valueToString()
        {
            return this.value.ToString();
        }
    }

    public class TezBonusInt
        : TezBonusValue<int>
    {
        public TezBonusInt(ITezBonusModifierContainer container) : base(container)
        {

        }

        public TezBonusInt(ITezBonusModifierContainer container, ITezValueDescriptor valueDescriptor) : base(container, valueDescriptor)
        {

        }

        protected override int calculateValue()
        {
            return mModifierContainer.calculate(ref mBaseValue);
        }

        public override void serialize(TezSaveController.Writer writer)
        {
            writer.write(this.name, mBaseValue);
        }

        public override void deserialize(TezSaveController.Reader reader)
        {
            this.baseValue = reader.readInt(this.name);
        }
    }

    public class TezBonusFloat
        : TezBonusValue<float>
    {
        public TezBonusFloat(ITezBonusModifierContainer container) : base(container)
        {

        }

        public TezBonusFloat(ITezBonusModifierContainer container, ITezValueDescriptor valueDescriptor) : base(container, valueDescriptor)
        {

        }

        protected override float calculateValue()
        {
            return mModifierContainer.calculate(ref mBaseValue);
        }

        public override void serialize(TezSaveController.Writer writer)
        {
            writer.write(this.name, mBaseValue);
        }

        public override void deserialize(TezSaveController.Reader reader)
        {
            this.baseValue = reader.readFloat(this.name);
        }
    }
}