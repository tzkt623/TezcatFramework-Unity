using System;

namespace tezcat.Framework.Core
{
    public class TezLitPropertySortedManager : ITezCloseable
    {
        TezLitPropertySortList mSortList = null;
        public TezLitPropertySortList sortList
        {
            get
            {
                if (mSortList == null)
                {
                    mSortList = new TezLitPropertySortList();
                }
                return mSortList;
            }
        }

        public T getOrCreate<T>(ITezValueDescriptor descriptor) where T : ITezLitProperty, new()
        {
            if (this.sortList.binaryFind(descriptor.ID, out int index))
            {
                return (T)mSortList[index];
            }
            else
            {
                var property = new T();
                property.descriptor = descriptor;
                this.sortList.insert(index, property);
                return property;
            }
        }

        public T get<T>(ITezValueDescriptor descriptor) where T : ITezLitProperty
        {
            return (T)this.sortList.binaryFind(descriptor.ID);
        }

        public bool remove(ITezValueDescriptor descriptor)
        {
            if (this.sortList.binaryFind(descriptor.ID, out int index))
            {
                this.sortList.removeAt(index);
                return true;
            }

            return false;
        }

        public void clearAll()
        {
            if (mSortList == null)
            {
                return;
            }

            for (int i = 0; i < mSortList.count; i++)
            {
                mSortList[i].close();
            }

            mSortList.clear();
        }

        public virtual void close()
        {
            this.clearAll();
            mSortList = null;
        }
    }

    public class TezLitPropertyArray : TezValueWrapperArray<ITezLitProperty>
    {

    }
}
