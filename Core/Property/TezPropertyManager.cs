namespace tezcat.Framework.Core
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

        /// <summary>
        /// 加入一个Modifier
        /// </summary>
        public void addModifier(ITezValueModifier modifier)
        {
            var target = modifier.modifierConfig.target;
            var property = this.get<ITezProperty>(target);
            property?.addModifier(modifier);
        }

        /// <summary>
        /// 加入一个modifier
        /// 并返回被操作的property
        /// </summary>
        public bool addModifier(ITezValueModifier modifier, out ITezProperty property)
        {
            var target = modifier.modifierConfig.target;
            property = this.get<ITezProperty>(target);
            property?.addModifier(modifier);
            return property != null;
        }

        /// <summary>
        /// 移除一个Modifier
        /// </summary>
        public bool removeModifier(ITezValueModifier modifier)
        {
            var target = modifier.modifierConfig.target;
            var property = this.get<ITezProperty>(target);
            return (property != null) && property.removeModifier(modifier);
        }

        /// <summary>
        /// 移除一个Modifier
        /// 并返回被操作的Property
        /// </summary>
        public bool removeModifier(ITezValueModifier modifier, out ITezProperty property)
        {
            var target = modifier.modifierConfig.target;
            property = this.get<ITezProperty>(target);
            return (property != null) && property.removeModifier(modifier);
        }

        public T getOrCreate<T>(ITezValueDescriptor descriptor) where T : ITezProperty, new()
        {
            if (this.properties.binaryFind(descriptor.ID, out int index))
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
            return (T)this.properties.binaryFind(descriptor.ID);
        }

        public bool remove(ITezValueDescriptor descriptor)
        {
            if (this.properties.binaryFind(descriptor.ID, out int index))
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
                m_Properties[i].close();
            }

            m_Properties.clear();
        }

        public virtual void close()
        {
            this.clearAll();
            m_Properties = null;
        }
    }
}