namespace tezcat.Framework.Core
{
    public class TezLitPropertyManager : ITezCloseable
    {
        TezLitPropertySortList mAttributes = null;
        public TezLitPropertySortList attributes
        {
            get
            {
                if (mAttributes == null)
                {
                    mAttributes = new TezLitPropertySortList();
                }
                return mAttributes;
            }
        }

        public T getOrCreate<T>(ITezValueDescriptor descriptor) where T : ITezLitProperty, new()
        {
            if (this.attributes.binaryFind(descriptor.ID, out int index))
            {
                return (T)mAttributes[index];
            }
            else
            {
                var property = new T();
                property.descriptor = descriptor;
                this.attributes.insert(index, property);
                return property;
            }
        }

        public T get<T>(ITezValueDescriptor descriptor) where T : ITezLitProperty
        {
            return (T)this.attributes.binaryFind(descriptor.ID);
        }

        public bool remove(ITezValueDescriptor descriptor)
        {
            if (this.attributes.binaryFind(descriptor.ID, out int index))
            {
                this.attributes.removeAt(index);
                return true;
            }

            return false;
        }

        public void clearAll()
        {
            if (mAttributes == null)
            {
                return;
            }

            for (int i = 0; i < mAttributes.count; i++)
            {
                mAttributes[i].close();
            }

            mAttributes.clear();
        }

        public virtual void close()
        {
            this.clearAll();
            mAttributes = null;
        }
    }
}
