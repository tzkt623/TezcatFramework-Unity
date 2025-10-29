using System;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game
{
    public interface ITezBonusableValue
        : ITezValueWrapper
        , ITezSerializable
    {
        //TezBonusToken bonusToken { get; set; }
        ITezBonusModifierContainer modifierContainer { get; }

        void createContainer<Container>() where Container : ITezBonusModifierContainer, new();
        void manualUpdate();

        void markDirty();
        void addModifier(ITezBonusModifier bonusModifier);
        void removeModifier(ITezBonusModifier bonusModifier);
    }

    public interface ITezBonusableValue<T> : ITezBonusableValue
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

    public abstract class TezBaseBonusableValue<T>
        : TezValueWrapper<T>
        , ITezBonusableValue<T>
    {
        public event TezEventExtension.Action<T> evtValueChanged;
        public sealed override TezWrapperType wrapperType => TezWrapperType.Bonusable;

//         protected TezBonusToken mBonusToken = null;
//         public TezBonusToken bonusToken
//         {
//             get => mBonusToken;
//             set => mBonusToken = value;
//         }

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
                    mDirty = false;
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

        public TezBaseBonusableValue(ITezValueDescriptor valueDescriptor) : base(valueDescriptor)
        {
            //mBonusToken = bonusToken;
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

            //mBonusToken = null;
            mModifierContainer = null;
            mDescriptor = null;

            this.evtValueChanged = null;
        }

        public void manualUpdate()
        {
            if (mDirty)
            {
                mDirty = true;
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

    public class TezBonusableInt
        : TezBaseBonusableValue<int>
    {
        public TezBonusableInt()
        {

        }

        public TezBonusableInt(ITezValueDescriptor valueDescriptor) : base(valueDescriptor)
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

    public class TezBonusableFloat
        : TezBaseBonusableValue<float>
    {
        public TezBonusableFloat()
        {

        }

        public TezBonusableFloat(ITezValueDescriptor valueDescriptor) : base(valueDescriptor)
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