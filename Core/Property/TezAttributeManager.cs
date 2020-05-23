﻿namespace tezcat.Framework.Core
{
    public class TezAttributeManager : ITezCloseable
    {
        TezAttributeSortList m_Attributes = null;
        public TezAttributeSortList attributes
        {
            get
            {
                if (m_Attributes == null)
                {
                    m_Attributes = new TezAttributeSortList();
                }
                return m_Attributes;
            }
        }

        public T getOrCreate<T>(ITezValueDescriptor descriptor) where T : ITezAttribute, new()
        {
            int index = 0;
            if (this.attributes.binaryFind(descriptor, out index))
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

        public T get<T>(ITezValueDescriptor descriptor) where T : ITezAttribute
        {
            return (T)this.attributes.binaryFind(descriptor);
        }

        public bool remove(ITezValueDescriptor descriptor)
        {
            int index = 0;
            if (this.attributes.binaryFind(descriptor, out index))
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
                m_Attributes[i].close(false);
            }

            m_Attributes.clear();
        }

        public virtual void close(bool self_close = true)
        {
            this.clearAll();
            m_Attributes = null;
        }
    }
}
