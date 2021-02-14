using tezcat.Framework.Core;

namespace tezcat.Framework.ECS
{
    public abstract class TezComBaseRenderer
        : TezMonoObject
        , ITezComponent
    {
        static int s_UID = TezComponentManager.register<TezComBaseRenderer>();
        public static int SUID => s_UID;

        /// <summary>
        /// 类型的唯一ID
        /// </summary>
        public int UID => s_UID;

        public TezEntity entity { get; private set; }

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

        void ITezComponent.onOtherComponentAdded(ITezComponent component, int comID)
        {
            this.onOtherComponentAdded(component, comID);
        }

        void ITezComponent.onOtherComponentRemoved(ITezComponent component, int comID)
        {
            this.onOtherComponentRemoved(component, comID);
        }

        protected virtual void onAddComponent(TezEntity entity)
        {

        }

        protected virtual void onRemoveComponent(TezEntity entity)
        {

        }

        protected virtual void onOtherComponentAdded(ITezComponent com, int comID)
        {

        }

        protected virtual void onOtherComponentRemoved(ITezComponent com, int comID)
        {

        }

        protected override void onClose()
        {
        }
    }
}