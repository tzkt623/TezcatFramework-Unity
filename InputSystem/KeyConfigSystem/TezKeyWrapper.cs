using System;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Extension;

namespace tezcat.Framework.InputSystem
{
    /// <summary>
    /// 按键包装层
    /// 用于分离具体按键与操作的强耦合
    /// 使自定义按键成为可能
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

        public abstract void resetToDefault(TezReader reader, int index);
    }
}