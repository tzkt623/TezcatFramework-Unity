﻿using System;
using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.ECS
{
    public abstract class TezComBaseData
        : TezObject
        , ITezComponent
    {
        static int s_ComUID = TezComponentManager.register<TezComBaseData>();
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
        protected virtual void onOtherComponentAdded(ITezComponent com, int comID) { }

        /// <summary>
        /// 当其他Com从Entity中移除时
        /// </summary>
        protected virtual void onOtherComponentRemoved(ITezComponent com, int comID) { }
    }
}