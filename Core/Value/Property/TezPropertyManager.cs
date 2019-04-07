namespace tezcat.Framework.Core
{
    public abstract class TezPropertyManager : ITezCloseable
    {
        TezPropertySortList m_Properties = null;
        public TezPropertySortList properties
        {
            get
            {
                if (m_Properties == null)
                {
                    m_Properties = new TezPropertySortList();
                }
                return m_Properties;
            }
        }

        public void addModifier(ITezModifier modifier)
        {
            var target = modifier.definition.target;
            var property = this.get<ITezProperty>(target);
            property?.addModifier(modifier);
        }

        public bool removeModifier(ITezModifier modifier)
        {
            var target = modifier.definition.target;
            var property = this.get<ITezProperty>(target);
            return (property != null) && property.removeModifier(modifier);
        }

        public T getOrCreate<T>(ITezValueDescriptor descriptor) where T : ITezProperty, new()
        {
            int index = 0;
            if (this.properties.binarySearch(descriptor, out index))
            {
                return (T)m_Properties[index];
            }
            else
            {
                var property = new T();
                property.descriptor = descriptor;
                this.properties.insert(index, property);
                return property;
            }
        }

        public T get<T>(ITezValueDescriptor descriptor) where T : ITezProperty
        {
            return (T)this.properties.binaryFind(descriptor);
        }

        public bool remove(ITezValueDescriptor descriptor)
        {
            int index = 0;
            if (this.properties.binarySearch(descriptor, out index))
            {
                this.properties.removeAt(index);
                return true;
            }

            return false;
        }

        public void clearAllProperty()
        {
            foreach (var item in m_Properties)
            {
                item.close();
            }
            m_Properties.clear();
        }

        public virtual void close()
        {
            m_Properties?.clear();
            m_Properties = null;
        }
    }
}