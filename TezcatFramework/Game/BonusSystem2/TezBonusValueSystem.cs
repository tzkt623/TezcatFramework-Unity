using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public interface ITezBonusSystemHolder
    {
        TezBonusValueSystem bonusSystem { get; }
    }

    /// <summary>
    /// 属性加成系统
    /// 一个对象一般来说只会有一套属性加成系统
    /// 比如
    /// 飞船有飞船的整套属性
    /// 炮台有炮台的整套属性
    /// </summary>
    public class TezBonusValueSystem : ITezCloseable
    {
        ITezBonusValue[][] mArrays = null;

        public void init()
        {
            if (mArrays == null)
            {
                mArrays = new ITezBonusValue[TezValueDescriptor.getTypeCount()][];
            }
        }

        public void register(ITezBonusValue bonusValue)
        {
            var descriptor = bonusValue.descriptor;

            if (mArrays[descriptor.typeID] == null)
            {
                mArrays[descriptor.typeID] = new ITezBonusValue[TezValueDescriptor.getTypeCapacity((short)descriptor.typeID)];
            }

            mArrays[descriptor.typeID][descriptor.indexID] = bonusValue;
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

        public void close()
        {
            foreach (ITezBonusValue[] item in mArrays)
            {
                Array.Clear(item, 0, item.Length);
            }

            Array.Clear(mArrays, 0, mArrays.Length);
            mArrays = null;
        }

        public T get<T>(ITezValueDescriptor valueDescriptor) where T : ITezBonusValue
        {
            return (T)mArrays[valueDescriptor.typeID][valueDescriptor.indexID];
        }
    }

    public class TezBonusValueArray : TezValueWrapperArray<ITezBonusValue>
    {

    }
}