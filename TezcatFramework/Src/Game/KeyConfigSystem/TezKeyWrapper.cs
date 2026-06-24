using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 按键包装器
    /// </summary>
    public abstract class TezKeyWrapper : ITezCloseable
    {
        public event TezEventExtension.Action<TezKeyWrapper> evtPrepare;
        public event TezEventExtension.Action<TezKeyWrapper> evtSave;
        public event TezEventExtension.Action<TezKeyWrapper> evtCancel;

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
            evtPrepare?.Invoke(this);
        }

        public virtual void cancelChange()
        {
            evtCancel?.Invoke(this);
        }


        public virtual void saveChange()
        {
            evtSave?.Invoke(this);
        }

        public void close()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {
            evtCancel = null;
            evtSave = null;
            evtPrepare = null;
        }

        public abstract void readFromSave(TezReader reader);
        public abstract void writeToSave(TezWriter writer);
    }
}