using tezcat.Framework.Core;

namespace tezcat.Framework.ECS
{
    public abstract class TezComBaseRenderer
        : TezMonoObject
        , ITezComponent
    {
        static int s_ComUID = TezComponentManager.register<TezComBaseRenderer>();
        public static int SComUID => s_ComUID;

        /// <summary>
        /// 类型的唯一ID
        /// </summary>
        public int comUID => s_ComUID;

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