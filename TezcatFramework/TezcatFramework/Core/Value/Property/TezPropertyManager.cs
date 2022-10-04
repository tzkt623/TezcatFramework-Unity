namespace tezcat.Framework.Core
{
    public class TezPropertyManager<Container>
        : ITezCloseable
        where Container : TezPropertyContainer, new()
    {
        Container m_Container = new Container();
        public TezPropertyContainer container => m_Container;




        /// <summary>
        /// 加入一个Modifier
        /// </summary>
        public void addModifier(ITezValueModifier modifier)
        {
            var target = modifier.modifierConfig.target;
            var property = m_Container.get(target);
            property?.addModifier(modifier);
        }

        /// <summary>
        /// 加入一个modifier
        /// 并返回被操作的property
        /// </summary>
        public bool addModifier(ITezValueModifier modifier, out ITezProperty property)
        {
            var target = modifier.modifierConfig.target;
            return m_Container.tryGet(target, out property);
        }

        /// <summary>
        /// 移除一个Modifier
        /// </summary>
        public bool removeModifier(ITezValueModifier modifier)
        {
            var target = modifier.modifierConfig.target;

            if (m_Container.tryGet(target, out var property))
            {
                property.removeModifier(modifier);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 移除一个Modifier
        /// 并返回被操作的Property
        /// </summary>
        public bool removeModifier(ITezValueModifier modifier, out ITezProperty property)
        {
            var target = modifier.modifierConfig.target;

            if (m_Container.tryGet(target, out property))
            {
                property.removeModifier(modifier);
                return true;
            }

            return false;
        }

        public T getOrCreate<T>(ITezValueDescriptor descriptor) where T : ITezProperty, new()
        {
            return m_Container.create<T>(descriptor);
        }

        public T get<T>(ITezValueDescriptor descriptor) where T : ITezProperty
        {
            return (T)m_Container.get(descriptor.ID);
        }

        public bool tryGet<T>(ITezValueDescriptor descriptor, out T property) where T : class, ITezProperty
        {
            if (m_Container.tryGet(descriptor.ID, out var result))
            {
                property = (T)result;
                return true;
            }

            property = null;
            return false;
        }

        public bool remove(ITezValueDescriptor descriptor)
        {
            return m_Container.remove(descriptor);
        }

        public void clearAll()
        {
            m_Container.clear();
        }

        public virtual void close()
        {
            m_Container.close();
            m_Container = null;
        }
    }
}