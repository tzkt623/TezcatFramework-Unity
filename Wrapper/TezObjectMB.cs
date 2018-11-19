using tezcat.Framework.Core;

namespace tezcat.Framework.Wrapper
{
    public abstract class TezGameObjectMB
        : TezMonoBehaviour
    {
        public string myName
        {
            get { return TezService.get<TezTranslator>().translateName(this.getObject().NID); }
        }

        public string myDescription
        {
            get { return TezService.get<TezTranslator>().translateDescription(this.getObject().NID); }
        }

        public abstract TezGameObject getObject();
    }

    public abstract class TezGameObjectMB<T> : TezGameObjectMB where T : TezGameObject
    {
        public T myObject { get; protected set; }

        public sealed override TezGameObject getObject()
        {
            return this.myObject;
        }

        public void bind(T my_object)
        {
            this.myObject = my_object;
            my_object.gameObject = this;
            this.onBind();
        }

        protected abstract void onBind();

        protected override void clear()
        {
            this.myObject = null;
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