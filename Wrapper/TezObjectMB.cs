using tezcat.Core;
using tezcat.DataBase;
using tezcat.Utility;

namespace tezcat.Wrapper
{
    public abstract class TezObjectMB
        : TezMonoBehaviour
        , ITezMBWrapper
    {
        public string myName
        {
            get { return TezTranslator.translateName(this.getObject().NID); }
        }

        public string myDescription
        {
            get { return TezTranslator.translateDescription(this.getObject().NID); }
        }

        public TezDatabase.GroupType group
        {
            get { return this.getObject().groupType; }
        }

        public TezDatabase.CategoryType category
        {
            get { return this.getObject().categoryType; }
        }

        public abstract TezObject getObject();
    }

    public abstract class TezObjectMB<T> : TezObjectMB where T : TezObject
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
            this.refresh();
        }

        protected abstract void onBind();

        protected override void clear()
        {
            this.myObject = null;
        }
    }
}