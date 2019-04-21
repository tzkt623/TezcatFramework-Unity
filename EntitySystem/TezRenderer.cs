﻿using tezcat.Framework.Core;

namespace tezcat.Framework.ECS
{
    public abstract class TezRenderer
        : TezMonoBehaviour
        , ITezComponent
    {
//         public string myName
//         {
//             get { return TezService.get<TezTranslator>().translateName(this.getObject().NID); }
//         }
// 
//         public string myDescription
//         {
//             get { return TezService.get<TezTranslator>().translateDescription(this.getObject().NID); }
//         }

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

    public abstract class TezToolObjectMB<T> : TezMonoBehaviour where T : TezToolObject
    {
        public T myObject { get; protected set; }

        public void bind(T my_object)
        {
            this.myObject = my_object;
            this.onBind();
        }

        protected abstract void onBind();

        protected override void clear()
        {
            this.myObject = null;
        }
    }
}