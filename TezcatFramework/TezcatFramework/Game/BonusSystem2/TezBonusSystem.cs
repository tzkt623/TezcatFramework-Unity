using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public interface ITezBonusSystemHolder
    {
        TezBonusSystem bonusSystem { get; }
    }

    /// <summary>
    /// 属性加成系统
    /// 一个对象一般来说只会有一套属性加成系统
    /// 比如
    /// 飞船有飞船的整套属性
    /// 炮台有炮台的整套属性
    /// </summary>
    public class TezBonusSystem : ITezCloseable
    {
        //ITezBonusableValue[] mArray = null;
        ITezBonusableValue[][] mArrays = null;

        public void init()
        {
            //var capacity = TezValueDescriptor.getTypeCapacity(bonusTokenTypeID);
            //mArray = new ITezBonusableValue[capacity];

            if (mArrays == null)
            {
                mArrays = new ITezBonusableValue[TezValueDescriptor.getTypeCount()][];
            }
        }

        public void set(ITezBonusableValue bonusableValue)
        {
            var token = bonusableValue.descriptor;

            if (mArrays[token.typeID] == null)
            {
                mArrays[token.typeID] = new ITezBonusableValue[TezValueDescriptor.getTypeCapacity((short)token.typeID)];
            }

            mArrays[token.typeID][token.indexID] = bonusableValue;

            //mArray[token.indexID] = bonusableValue;
        }

        public void addModifier(ITezBonusModifier modifier)
        {
            var descriptor = modifier.valueDescriptor;
            mArrays[descriptor.typeID][descriptor.indexID].addModifier(modifier);
        }

        public void removeModifier(ITezBonusModifier modifier)
        {
            var descriptor = modifier.valueDescriptor;
            mArrays[descriptor.typeID][descriptor.indexID].removeModifier(modifier);
        }

        void ITezCloseable.closeThis()
        {
            mArrays = null;
        }

        public T get<T>(ITezValueDescriptor valueDescriptor) where T : ITezBonusableValue
        {
            return (T)mArrays[valueDescriptor.typeID][valueDescriptor.indexID];
        }
    }

    public class TezBonusableValueArray : TezValueWrapperArray<ITezBonusableValue>
    {

    }
}