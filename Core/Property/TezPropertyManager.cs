﻿namespace tezcat.Framework.Core
{
    public class TezPropertyManager : ITezCloseable
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

        public void addModifier(ITezValueModifier modifier)
        {
            var target = ((TezValueModifierDefinition)modifier.definition).target;
            var property = this.get<ITezProperty>(target);
            property?.addModifier(modifier);
        }

        public bool removeModifier(ITezValueModifier modifier)
        {
            var target = ((TezValueModifierDefinition)modifier.definition).target;
            var property = this.get<ITezProperty>(target);
            return (property != null) && property.removeModifier(modifier);
        }

        public T getOrCreate<T>(ITezValueDescriptor descriptor) where T : ITezProperty, new()
        {
            int index = 0;
            if (this.properties.binaryFind(descriptor, out index))
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
            if (this.properties.binaryFind(descriptor, out index))
            {
                this.properties.removeAt(index);
                return true;
            }

            return false;
        }

        public void clearAll()
        {
            if (m_Properties == null)
            {
                return;
            }

            for (int i = 0; i < m_Properties.count; i++)
            {
                m_Properties[i].close(false);
            }

            m_Properties.clear();
        }

        public virtual void close(bool self_close = true)
        {
            this.clearAll();
            m_Properties = null;
        }
    }
}