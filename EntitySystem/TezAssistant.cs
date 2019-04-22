namespace tezcat.Framework.ECS
{
    /// <summary>
    /// 模块助理
    /// 本身并没有任何功能
    /// 用于处理一些非常零散或者特别的功能
    /// </summary>
//     public abstract class TezAssistant : ITezComponent
//     {
//         public TezEntity entity { get; private set; }
// 
//         void ITezComponent.onAdd(TezEntity entity)
//         {
//             this.entity = entity;
//             this.onAddComponent(entity);
//         }
// 
//         void ITezComponent.onRemove(TezEntity entity)
//         {
//             this.onRemoveComponent(entity);
//             this.entity = null;
//         }
// 
//         void ITezComponent.onOtherComponentAdded(ITezComponent component, int com_id)
//         {
//             this.onOtherComponentAdded(component, com_id);
//         }
// 
//         void ITezComponent.onOtherComponentRemoved(ITezComponent component, int com_id)
//         {
//             this.onOtherComponentRemoved(component, com_id);
//         }
// 
//         protected abstract void onAddComponent(TezEntity entity);
//         protected abstract void onRemoveComponent(TezEntity entity);
//         protected abstract void onOtherComponentAdded(ITezComponent com, int com_id);
//         protected abstract void onOtherComponentRemoved(ITezComponent com, int com_id);
// 
//         public virtual void close()
//         {
//             this.entity = null;
//         }
//     }
}