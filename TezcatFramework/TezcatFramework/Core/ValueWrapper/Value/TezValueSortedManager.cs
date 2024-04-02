namespace tezcat.Framework.Core
{
    public class TezValueSortedManager : ITezCloseable
    {
        TezValueSortedList mSortList = null;
        public TezValueSortedList sortList
        {
            get
            {
                if (mSortList == null)
                {
                    mSortList = new TezValueSortedList();
                }
                return mSortList;
            }
        }

        public T getOrCreate<T>(ITezValueDescriptor descriptor) where T : ITezValue, new()
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

        public T get<T>(ITezValueDescriptor descriptor) where T : ITezValue
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

        void ITezCloseable.deleteThis()
        {
            this.clearAll();
            mSortList = null;
        }
    }
}
