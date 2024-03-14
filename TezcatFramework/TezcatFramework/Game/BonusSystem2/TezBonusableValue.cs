using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public interface ITezBonusableValue : ITezCloseable
    {
        TezBonusBaseModifierContainer modifierContainer { get; }
        ITezLitProperty baseProperty { get; }
        void setContainer(TezBonusBaseModifierContainer modifierContainer);
        void manualUpdate();
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

    public class TezBonusableInt
        : ITezBonusableValue<int>
        , ITezBonusReadModifierEntry
    {
        TezBonusBaseModifierContainer mModifierContainer = null;
        public TezBonusBaseModifierContainer modifierContainer => mModifierContainer;

        TezLitProperty<int> mValue = new TezLitProperty<int>();
        public ITezLitProperty baseProperty => mValue;
        public ITezLitProperty<int> porperty => mValue;

        int mBaseValue = 0;
        public int baseValue
        {
            get { return mBaseValue; }
            set
            {
                mBaseValue = value;
                mModifierContainer?.markDirty();
            }
        }

        public int value
        {
            get
            {
                if (mModifierContainer.isDirty)
                {
                    mValue.innerValue = mModifierContainer.calculate(ref mBaseValue);
                }

                return mValue.value;
            }
        }

        float ITezBonusReadModifierEntry.value => this.value;

        public TezBonusableInt()
        {
        }

        public TezBonusableInt(ITezValueDescriptor valueDescriptor)
        {
            mValue.descriptor = valueDescriptor;
        }

        public void setContainer(TezBonusBaseModifierContainer modifierContainer)
        {
            mModifierContainer = modifierContainer;
        }

        public void manualUpdate()
        {
            if (mModifierContainer.isDirty)
            {
                mValue.innerValue = mModifierContainer.calculate(ref mBaseValue);
            }

            mValue.manualUpdate();
        }

        public virtual void close()
        {
            mValue.close();
            //mModifierContainer.close();

            mModifierContainer = null;
            mValue = null;
        }
    }

    public class TezBonusableFloat
        : ITezBonusableValue<float>
        , ITezBonusReadModifierEntry
    {
        TezBonusBaseModifierContainer mModifierContainer = null;
        public TezBonusBaseModifierContainer modifierContainer => mModifierContainer;

        TezLitProperty<float> mValue = new TezLitProperty<float>();
        public ITezLitProperty baseProperty => mValue;
        public ITezLitProperty<float> porperty => mValue;

        float mBaseValue = 0;
        public float baseValue
        {
            get { return mBaseValue; }
            set
            {
                mBaseValue = value;
                mModifierContainer?.markDirty();
            }
        }

        public float value
        {
            get
            {
                if (mModifierContainer.isDirty)
                {
                    mValue.innerValue = mModifierContainer.calculate(ref mBaseValue);
                }

                return mValue.value;
            }
        }

        public TezBonusableFloat()
        {

        }

        public TezBonusableFloat(ITezValueDescriptor valueDescriptor)
        {
            mValue.descriptor = valueDescriptor;
        }

        public void setContainer(TezBonusBaseModifierContainer modifierContainer)
        {
            mModifierContainer = modifierContainer;
        }

        public void manualUpdate()
        {
            if (mModifierContainer.isDirty)
            {
                mValue.innerValue = mModifierContainer.calculate(ref mBaseValue);
            }

            mValue.manualUpdate();
        }

        public virtual void close()
        {
            mValue.close();
            //mModifierContainer.close();

            mModifierContainer = null;
            mValue = null;
        }
    }
}