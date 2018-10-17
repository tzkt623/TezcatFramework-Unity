using tezcat.Core;
using tezcat.Utility;

namespace tezcat.Wrapper
{
    /// <summary>
    /// GameObject包装器
    /// 用来获得基础资源信息
    /// </summary>
    public abstract class TezGameObjectWrapper : ITezGameObjectWrapper
    {
        public string myName
        {
            get { return TezTranslator.translateName(this.getObject().NID); }
        }

        public string myDescription
        {
            get { return TezTranslator.translateDescription(this.getObject().NID); }
        }

        public abstract TezGameObject getObject();

        public abstract void close();

        #region 重载操作
        public static bool operator true(TezGameObjectWrapper obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezGameObjectWrapper obj)
        {
            return object.ReferenceEquals(obj, null);
        }

        public static bool operator !(TezGameObjectWrapper obj)
        {
            return object.ReferenceEquals(obj, null);
        }
        #endregion
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
        public T myObject { get; set; } = null;

        public sealed override TezGameObject getObject()
        {
            return this.myObject;
        }

        public override void close()
        {
            this.myObject = null;
        }
    }
}