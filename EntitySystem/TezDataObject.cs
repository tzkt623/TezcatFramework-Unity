using System;
using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.ECS
{
    public abstract class TezDataObject
        : TezObject
        , ITezComponent
    {
        #region Component
        /// <summary>
        /// Com的唯一ID
        /// </summary>
        public static int ComUID { get; } = TezComponentManager.register<TezDataObject>();

        public TezEntity entity { get; private set; }
        /// <summary>
        /// 此Com类型的唯一ID
        /// </summary>
        public int comID => ComUID;

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

        /// <summary>
        /// 当此Com添加进Entity时
        /// </summary>
        protected virtual void onAddComponent(TezEntity entity) { }

        /// <summary>
        /// 当此Com从Entity中移除时
        /// </summary>
        protected virtual void onRemoveComponent(TezEntity entity) { }

        /// <summary>
        /// 当其他Com添加进Entity时
        /// </summary>
        protected virtual void onOtherComponentAdded(ITezComponent com, int com_id) { }

        /// <summary>
        /// 当其他Com从Entity中移除时
        /// </summary>
        protected virtual void onOtherComponentRemoved(ITezComponent com, int com_id) { }
        #endregion
    }
}