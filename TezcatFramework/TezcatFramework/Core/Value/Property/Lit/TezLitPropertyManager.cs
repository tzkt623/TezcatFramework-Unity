namespace tezcat.Framework.Core
{
    public class TezLitPropertyManager : ITezCloseable
    {
        TezLitPropertySortList m_Attributes = null;
        public TezLitPropertySortList attributes
        {
            get
            {
                if (m_Attributes == null)
                {
                    m_Attributes = new TezLitPropertySortList();
                }
                return m_Attributes;
            }
        }

        public T getOrCreate<T>(ITezValueDescriptor descriptor) where T : ITezLitProperty, new()
        {
            if (this.attributes.binaryFind(descriptor.ID, out int index))
            {
                return (T)m_Attributes[index];
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
            if (m_Attributes == null)
            {
                return;
            }

            for (int i = 0; i < m_Attributes.count; i++)
            {
                m_Attributes[i].close();
            }

            m_Attributes.clear();
        }

        public virtual void close()
        {
            this.clearAll();
            m_Attributes = null;
        }
    }
}
