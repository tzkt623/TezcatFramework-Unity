using tezcat.Framework.ECS;

namespace tezcat.Framework.Wrapper
{
    public interface ITezWrapper
        : ITezComponent
    {
        void retain();
    }

    public abstract class TezWrapper : ITezWrapper
    {
        public TezEntity entity { get; private set; }
        int m_RefCount = 0; 

        public void close()
        {
            m_RefCount -= 1;
            if (m_RefCount == 0)
            {
                this.onClose();
                entity.removeComponent<TezWrapper>();
            }
        }

        public void retain()
        {
            m_RefCount += 1;
        }

        protected abstract void onClose();
        
        void ITezComponent.onAdd(TezEntity entity)
        {
            this.entity = entity;
            this.onAddComponent(entity);
        }

        void ITezComponent.onRemove(TezEntity entity)
        {
            this.onRemoveComponent(entity);
            this.entity = null;
        }

        void ITezComponent.onOtherComponentAdded(ITezComponent component, int com_id)
        {
            this.onOtherComponentAdded(component, com_id);
        }

        void ITezComponent.onOtherComponentRemoved(ITezComponent component, int com_id)
        {
            this.onOtherComponentRemoved(component, com_id);
        }

        protected virtual void onAddComponent(TezEntity entity)
        {

        }

        protected virtual void onRemoveComponent(TezEntity entity)
        {

        }

        protected virtual void onOtherComponentAdded(ITezComponent com, int com_id)
        {

        }

        protected virtual void onOtherComponentRemoved(ITezComponent com, int com_id)
        {

        }

        #region 重载操作
        public static bool operator true(TezWrapper wrapper)
        {
            return !object.ReferenceEquals(wrapper, null);
        }

        public static bool operator false(TezWrapper wrapper)
        {
            return object.ReferenceEquals(wrapper, null);
        }

        public static bool operator !(TezWrapper wrapper)
        {
            return object.ReferenceEquals(wrapper, null);
        }
        #endregion
    }
}