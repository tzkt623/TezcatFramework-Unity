using tezcat.Framework.Core;
using tezcat.Framework.ECS;

namespace tezcat.Framework.Wrapper
{
    public interface ITezGameObjectWrapper : ITezWrapper
    {
        string myName { get; }
        string myDescription { get; }
        TezGameObject getGameObject();
    }

    /// <summary>
    /// GameObject包装器
    /// 用来获得基础资源信息
    /// </summary>
    public abstract class TezGameObjectWrapper : TezWrapper
    {
        public string myName
        {
            get { return TezService.get<TezTranslator>().translateName(this.getGameObject().NID); }
        }

        public string myDescription
        {
            get { return TezService.get<TezTranslator>().translateDescription(this.getGameObject().NID); }
        }


        public abstract TezGameObject getGameObject();
    }

    /// <summary>
    /// 游戏对象的包装器
    /// 
    /// <para>用来获得游戏对象的基础信息内容,比如 名称 描述 图标等等</para>
    /// <para>包装器不会也不能控制游戏对象的生命周期,只是弱引用</para>
    /// <para>请勿在此类中释放对象</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TezGameObjectWrapper<T>
        : TezGameObjectWrapper
        where T : TezGameObject
    {
        public T myGameObject { get; private set; } = null;

        public sealed override TezGameObject getGameObject()
        {
            return this.myGameObject;
        }

        protected override void onClose()
        {
            this.myGameObject = null;
        }

        protected override void onAddComponent(TezEntity entity)
        {
            this.myGameObject = entity.getComponent<TezDataObject, T>();
        }

        protected override void onRemoveComponent(TezEntity entity)
        {
            this.myGameObject = null;
        }
    }


    public abstract class TezToolObjectWrapper : TezWrapper
    {
        public abstract TezToolObject getToolObject();
    }

    public abstract class TezToolObjectWrapper<T>
        : TezToolObjectWrapper
        where T : TezToolObject
    {
        public T myToolObject { get; set; } = null;

        public sealed override TezToolObject getToolObject()
        {
            return this.myToolObject;
        }

        protected override void onClose()
        {
            this.myToolObject = null;
        }
    }
}