using tezcat.Core;
using tezcat.Utility;

namespace tezcat.Wrapper
{
    public interface ITezObjectWrapper : ITezWrapper
    {
        TezObject getObject();
    }

    public abstract class TezObjectWrapper : ITezObjectWrapper
    {
        public string myName
        {
            get { return TezTranslator.translateName(this.getObject().NID); }
        }

        public string myDescription
        {
            get { return TezTranslator.translateDescription(this.getObject().NID); }
        }

        public abstract void close();

        public abstract TezObject getObject();

        #region 重载操作
        public static bool operator true(TezObjectWrapper obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezObjectWrapper obj)
        {
            return object.ReferenceEquals(obj, null);
        }

        public static bool operator !(TezObjectWrapper obj)
        {
            return object.ReferenceEquals(obj, null);
        }
        #endregion
    }

    /// <summary>
    /// Object专用包装器
    /// 
    /// 用来获得Object的基础资源信息
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TezObjectWrapper<T> : TezObjectWrapper where T : TezObject
    {
        public T myObject { get; protected set; }

        public TezObjectWrapper(T my_object)
        {
            this.myObject = my_object;
        }

        public sealed override TezObject getObject()
        {
            return this.myObject;
        }

        public override void close()
        {
            myObject = null;
        }
    }
}