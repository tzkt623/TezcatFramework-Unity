using tezcat.Framework.Core;

namespace tezcat.Framework.Wrapper
{
    public abstract class TezGameObjectMB
        : TezMonoBehaviour
        , ITezMBWrapper
    {
        public string myName
        {
            get { return TezTranslator.translateName(null); }
        }

        public string myDescription
        {
            get { return TezTranslator.translateDescription(null); }
        }

        public abstract TezObject getObject();
    }

    public abstract class TezGameObjectMB<T> : TezGameObjectMB where T : TezObject
    {
        public T myObject { get; protected set; }

        public sealed override TezObject getObject()
        {
            return this.myObject;
        }

        public void bind(T my_object)
        {
            this.myObject = my_object;
            this.onBind();
            this.refresh(RefreshPhase.System1);
        }

        protected abstract void onBind();

        protected override void clear()
        {
            this.myObject = null;
        }
    }
}