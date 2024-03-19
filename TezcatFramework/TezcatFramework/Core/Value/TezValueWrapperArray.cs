using System;

namespace tezcat.Framework.Core
{
    public class TezValueWrapperArray<ValueType>
        : ITezCloseable
        where ValueType : ITezValueWrapper
    {
        ValueType[] mArray = null;

        public virtual void init(short descriptorTypeID)
        {
            var capacity = TezValueDescriptor.generateTypeCapacity(descriptorTypeID);
            mArray = new ValueType[capacity];
        }

        public void set(ValueType value)
        {
            var iid = value.descriptor.indexID;
            if (mArray[iid] != null)
            {
                throw new Exception();
            }
            mArray[iid] = value;
        }

        public ITezValueWrapper get(ITezValueDescriptor valueDescriptor)
        {
            return mArray[valueDescriptor.indexID];
        }

        public T get<T>(ITezValueDescriptor valueDescriptor) where T : ValueType
        {
            return (T)mArray[valueDescriptor.indexID];
        }

        public virtual void close()
        {
            mArray = null;
        }
    }
}