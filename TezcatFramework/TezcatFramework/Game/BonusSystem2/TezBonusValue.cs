using System;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game
{
    public interface ITezBonusValue
        : ITezValueWrapper
        , ITezSerializable
    {
        void manualUpdate();

        void setDirty();
        void addModifier(ITezBonusModifier bonusModifier);
        void removeModifier(ITezBonusModifier bonusModifier);
    }

    public interface ITezBonusValue<T> : ITezBonusValue
    {
        event TezEventExtension.Action<T> evtValueChanged;

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
        public event TezEventExtension.Action<T> evtValueChanged;
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
            mModifierContainer.add(bonusModifier);
        }

        public void removeModifier(ITezBonusModifier bonusModifier)
        {
            mModifierContainer.remove(bonusModifier);
        }

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

        public string valueToString()
        {
            return mValue.ToString();
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
            mBaseValue = reader.readInt(this.name);
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
            mBaseValue = reader.readFloat(this.name);
        }
    }
}