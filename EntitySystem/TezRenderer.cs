using tezcat.Framework.Core;

namespace tezcat.Framework.ECS
{
    public abstract class TezRenderer
        : TezMonoObject
        , ITezComponent
    {
        public static int StaticComID { get; private set; } = TezComponentManager.register<TezRenderer>();
        public int ComID => StaticComID;

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

        protected override void clear()
        {
           
        }
    }
}