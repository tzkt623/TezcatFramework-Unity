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
            get { return TezTranslator.translateName(myObject.NID); }
        }

        public string myDescription
        {
            get { return TezTranslator.translateDescription(myObject.NID); }
        }

        public TezGameObject myObject { get; private set; }

        public TezGameObjectWrapper(TezGameObject @object)
        {
            myObject = @object;
        }

        public virtual void close()
        {
            myObject = null;
        }

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

    public abstract class TezGameObjectWrapper<T> : TezGameObjectWrapper where T : TezGameObject
    {
        public TezGameObjectWrapper(T my_object) : base(my_object)
        {

        }

        public T getObject()
        {
            return (T)myObject;
        }
    }
}