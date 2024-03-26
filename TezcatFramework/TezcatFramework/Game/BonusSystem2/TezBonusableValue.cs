using System;
using tezcat.Framework.Core;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public interface ITezBonusableValue
        : ITezValueWrapper
        , ITezSerializable
    {
        TezBonusToken bonusToken { get; set; }
        ITezBonusModifierContainer modifierContainer { get; }
        ITezLitProperty baseProperty { get; }

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

        /// <summary>
        /// 当前包含的Litproperty
        /// </summary>
        ITezLitProperty<T> porperty { get; }
    }

    public abstract class TezBaseBonusaValue<T>
        : ITezBonusableValue<T>
    {
        protected TezLitProperty<T> mLitPoperty = new TezLitProperty<T>();
        public ITezLitProperty baseProperty => mLitPoperty;
        public ITezLitProperty<T> porperty => mLitPoperty;


        public Type systemType => mLitPoperty.systemType;
        public TezValueType valueType => mLitPoperty.valueType;
        public TezWrapperType wrapperType => TezWrapperType.Bonusable;
        public int ID => mLitPoperty.ID;
        public string name => mLitPoperty.name;

        public ITezValueDescriptor descriptor
        {
            get => mLitPoperty.descriptor;
            set => mLitPoperty.descriptor = value;
        }

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

        public T value
        {
            get
            {
                if (mDirty)
                {
                    mDirty = true;
                    mLitPoperty.innerValue = this.calculateValue();
                }
                return mLitPoperty.value;
            }
        }

        public TezBaseBonusaValue(TezBonusToken bonusToken, ITezValueDescriptor valueDescriptor)
        {
            mBonusToken = bonusToken;
            mLitPoperty.descriptor = valueDescriptor;
        }

        public TezBaseBonusaValue()
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

        public virtual void close()
        {
            mLitPoperty.close();
            mModifierContainer.close();

            mLitPoperty = null;
            mBonusToken = null;
            mModifierContainer = null;
        }

        public void manualUpdate()
        {
            if (mDirty)
            {
                mDirty = true;
                mLitPoperty.innerValue = this.calculateValue();
            }

            mLitPoperty.manualUpdate();
        }

        public abstract void serialize(TezWriter writer);
        public abstract void deserialize(TezReader reader);

        public string valueToString()
        {
            return mLitPoperty.valueToString();
        }
    }

    public class TezBonusableInt
        : TezBaseBonusaValue<int>
    {
        public TezBonusableInt()
        {
            mLitPoperty.value = 0;
        }

        public TezBonusableInt(TezBonusToken bonusToken, ITezValueDescriptor valueDescriptor) : base(bonusToken, valueDescriptor)
        {
            mLitPoperty.value = 0;
        }

        protected override int calculateValue()
        {
            return mModifierContainer.calculate(ref mBaseValue);
        }

        public override void serialize(TezWriter writer)
        {
            writer.write(mLitPoperty.descriptor.name, mBaseValue);
        }

        public override void deserialize(TezReader reader)
        {
            mBaseValue = reader.readInt(mLitPoperty.descriptor.name);
        }
    }

    public class TezBonusableFloat
        : TezBaseBonusaValue<float>
    {
        public TezBonusableFloat()
        {
            mLitPoperty.value = 0.0f;
        }

        public TezBonusableFloat(TezBonusToken bonusToken, ITezValueDescriptor valueDescriptor) : base(bonusToken, valueDescriptor)
        {
            mLitPoperty.value = 0.0f;
        }

        protected override float calculateValue()
        {
            return mModifierContainer.calculate(ref mBaseValue);
        }

        public override void serialize(TezWriter writer)
        {
            writer.write(mLitPoperty.descriptor.name, mBaseValue);
        }

        public override void deserialize(TezReader reader)
        {
            mBaseValue = reader.readFloat(mLitPoperty.descriptor.name);
        }
    }
}