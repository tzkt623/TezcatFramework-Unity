using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 按键包装器
    /// </summary>
    public abstract class TezKeyWrapper : ITezCloseable
    {
        public event TezEventExtension.Action<TezKeyWrapper> onPrepare;
        public event TezEventExtension.Action<TezKeyWrapper> onSave;
        public event TezEventExtension.Action<TezKeyWrapper> onCancel;

        public virtual string name { get; }

        public abstract bool active();

        public void waitingKey()
        {
            if (this.onChangeKeyDown())
            {
                this.saveChange();
            }
        }

        protected abstract bool onChangeKeyDown();


        public virtual void prepareChange()
        {
            onPrepare?.Invoke(this);
        }

        public virtual void cancelChange()
        {
            onCancel?.Invoke(this);
        }


        public virtual void saveChange()
        {
            onSave?.Invoke(this);
        }

        public virtual void close()
        {
            onCancel = null;
            onSave = null;
            onPrepare = null;
        }

        public abstract void readFromSave(TezReader reader);
        public abstract void writeToSave(TezWriter writer);
    }
}