using tezcat.Core;
using tezcat.Utility;

namespace tezcat.Wrapper
{
    public interface ITezWrapper
    {
        string myName { get; }
        string myDescription { get; }

        void clean();
    }

    public interface ITezObjectWrapper : ITezWrapper
    {
        TezObject getObject();
    }

    public abstract class TezObjectWrapper : ITezObjectWrapper
    {
        public string myName
        {
            get { return TezLocalization.getName(this.getObject().NID); }
        }

        public string myDescription
        {
            get { return TezLocalization.getDescription(this.getObject().NID); }
        }

        public abstract void clean();

        public abstract TezObject getObject();
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

        public override void clean()
        {
            myObject.clear();
            myObject = null;
        }
    }
}