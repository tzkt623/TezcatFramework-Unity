using System;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game
{
    public interface ITezBonusableValue
        : ITezValueWrapper
        , ITezSerializable
    {
        event TezEventExtension.Action<ITezBonusableValue> onValueChanged;

        TezBonusToken bonusToken { get; set; }
        ITezBonusModifierContainer modifierContainer { get; }

        void createContainer<Container>() where Container : ITezBonusModifierContainer, new();
        void manualUpdate();

        void markDirty();
        void addModifier(ITezBonusModifier bonusModifier);
        void removeModifier(ITezBonusModifier bonusModifier);
    }

    public interface ITezBonusableValue<T> : ITezBonusableValue
    {
        /// <summary>
        /// 实际数值
        /// </summary>
        T value { get; }

        /// <summary>
        /// 基础数值
        /// </summary>
        T baseValue { get; set; }
    }

    public abstract class TezBaseBonusableValue<T>
        : TezValueWrapper<T>
        , ITezBonusableValue<T>
    {
        public event TezEventExtension.Action<ITezBonusableValue> onValueChanged;
        public sealed override TezWrapperType wrapperType => TezWrapperType.Bonusable;

        protected TezBonusToken mBonusToken = null;
        public TezBonusToken bonusToken
        {
            get => mBonusToken;
            set => mBonusToken = value;
        }

        protected ITezBonusModifierContainer mModifierContainer = null;
        public ITezBonusModifierContainer modifierContainer => mModifierContainer;

        protected bool mDirty = true;

        protected T mBaseValue = default;
        public T baseValue
        {
            get { return mBaseValue; }
            set
            {
                mDirty = true;
                mBaseValue = value;
            }
        }

        protected T mValue = default;
        public override T value
        {
            get
            {
                if (mDirty)
                {
                    mDirty = true;
                    mValue = this.calculateValue();
                    this.onValueChanged?.Invoke(this);
                }
                return mValue;
            }

            set
            {
                throw new Exception();
            }
        }

        public TezBaseBonusableValue(TezBonusToken bonusToken, ITezValueDescriptor valueDescriptor) : base(valueDescriptor)
        {
            mBonusToken = bonusToken;
        }

        public TezBaseBonusableValue()
        {

        }

        protected abstract T calculateValue();

        public void createContainer<Container>() where Container : ITezBonusModifierContainer, new()
        {
            mModifierContainer = new Container();
        }

        public void markDirty()
        {
            mDirty = true;
        }

        public void addModifier(ITezBonusModifier bonusModifier)
        {
            mDirty = true;
            mModifierContainer.add(bonusModifier);
        }

        public void removeModifier(ITezBonusModifier bonusModifier)
        {
            if (mModifierContainer.remove(bonusModifier))
            {
                mDirty = true;
            }
        }

        protected override void onClose()
        {
            base.onClose();
            mModifierContainer.close();

            mBonusToken = null;
            mModifierContainer = null;
            mDescriptor = null;

            this.onValueChanged = null;
        }

        public void manualUpdate()
        {
            if (mDirty)
            {
                mDirty = true;
                mValue = this.calculateValue();
            }

            this.onValueChanged?.Invoke(this);
        }

        public abstract void serialize(TezWriter writer);
        public abstract void deserialize(TezReader reader);

        public string valueToString()
        {
            return mValue.ToString();
        }
    }

    public class TezBonusableInt
        : TezBaseBonusableValue<int>
    {
        public TezBonusableInt()
        {

        }

        public TezBonusableInt(TezBonusToken bonusToken, ITezValueDescriptor valueDescriptor) : base(bonusToken, valueDescriptor)
        {

        }

        protected override int calculateValue()
        {
            return mModifierContainer.calculate(ref mBaseValue);
        }

        public override void serialize(TezWriter writer)
        {
            writer.write(this.name, mBaseValue);
        }

        public override void deserialize(TezReader reader)
        {
            mBaseValue = reader.readInt(this.name);
        }
    }

    public class TezBonusableFloat
        : TezBaseBonusableValue<float>
    {
        public TezBonusableFloat()
        {

        }

        public TezBonusableFloat(TezBonusToken bonusToken, ITezValueDescriptor valueDescriptor) : base(bonusToken, valueDescriptor)
        {

        }

        protected override float calculateValue()
        {
            return mModifierContainer.calculate(ref mBaseValue);
        }

        public override void serialize(TezWriter writer)
        {
            writer.write(this.name, mBaseValue);
        }

        public override void deserialize(TezReader reader)
        {
            mBaseValue = reader.readFloat(this.name);
        }
    }
}