namespace tezcat.Framework.Core
{
    public class TezPropertyManager<Container>
        : ITezCloseable
        where Container : TezPropertyContainer, new()
    {
        Container mContainer = new Container();
        public TezPropertyContainer container => mContainer;

        /// <summary>
        /// 加入一个Modifier
        /// </summary>
        public void addModifier(ITezValueModifier modifier)
        {
            var target = modifier.modifierConfig.target;
            var property = mContainer.get(target);
            property?.addModifier(modifier);
        }

        /// <summary>
        /// 加入一个modifier
        /// 并返回被操作的property
        /// </summary>
        public bool addModifier(ITezValueModifier modifier, out ITezProperty property)
        {
            var target = modifier.modifierConfig.target;
            return mContainer.tryGet(target, out property);
        }

        /// <summary>
        /// 移除一个Modifier
        /// </summary>
        public bool removeModifier(ITezValueModifier modifier)
        {
            var target = modifier.modifierConfig.target;

            if (mContainer.tryGet(target, out var property))
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

            if (mContainer.tryGet(target, out property))
            {
                property.removeModifier(modifier);
                return true;
            }

            return false;
        }

        public T getOrCreate<T>(ITezValueDescriptor descriptor) where T : ITezProperty, new()
        {
            return mContainer.create<T>(descriptor);
        }

        public T get<T>(ITezValueDescriptor descriptor) where T : ITezProperty
        {
            return (T)mContainer.get(descriptor.ID);
        }

        public bool tryGet<T>(ITezValueDescriptor descriptor, out T property) where T : class, ITezProperty
        {
            if (mContainer.tryGet(descriptor.ID, out var result))
            {
                property = (T)result;
                return true;
            }

            property = null;
            return false;
        }

        public bool remove(ITezValueDescriptor descriptor)
        {
            return mContainer.remove(descriptor);
        }

        public void clearAll()
        {
            mContainer.clear();
        }

        void ITezCloseable.closeThis()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {
            mContainer.close();
            mContainer = null;
        }
    }
}