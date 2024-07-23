using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 轻量级的Value包装器
    /// </summary>
	public class TezBindable<T> : ITezCloseable
    {
        /// <summary>
        /// 通知事件
        /// </summary>
	    public event TezEventExtension.Action<T> onChanged;

        /// <summary>
        /// 内部实际数据
        /// 修改此数据不会触发通知
        /// </summary>
	    public T innerValue;

        /// <summary>
        /// 外部数据
        /// 修改此数据会触发通知
        /// </summary>
	    public T value
        {
            get { return innerValue; }
            set
            {
                innerValue = value;
                onChanged?.Invoke(innerValue);
            }
        }

        /// <summary>
        /// 手动通知
        /// </summary>
        public void notify()
        {
            onChanged?.Invoke(innerValue);
        }

        /// <summary>
        /// 清空通知事件
        /// </summary>
        public void clearEvent()
        {
            onChanged = null;
        }

        void ITezCloseable.closeThis()
        {
            onChanged = null;
            innerValue = default;
        }
    }

    /// <summary>
    /// 轻量级的Value包装器
    /// Min Max 版
    /// </summary>
    public class TezBindableMax<T> : ITezCloseable
    {
        /// <summary>
        /// 通知事件
        /// Value,Max
        /// </summary>
        public event TezEventExtension.Action<T, T> onChanged;

        /// <summary>
        /// 内部实际数据
        /// 修改此数据不会触发通知
        /// </summary>
        public T innerValue;

        /// <summary>
        /// 内部实际数据
        /// 修改此数据不会触发通知
        /// </summary>
        public T innerMax;

        /// <summary>
        /// 外部数据
        /// 修改此数据会触发通知
        /// </summary>
        public T value
        {
            get { return innerValue; }
            set
            {
                this.innerValue = value;
                onChanged?.Invoke(this.innerValue, innerMax);
            }
        }

        /// <summary>
        /// 外部数据
        /// 修改此数据会触发通知
        /// </summary>
        public T max
        {
            get { return innerMax; }
            set
            {
                innerMax = value;
                onChanged?.Invoke(this.innerValue, innerMax);
            }
        }

        /// <summary>
        /// 手动通知
        /// </summary>
        public void notify()
        {
            onChanged?.Invoke(innerValue, innerMax);
        }


        void ITezCloseable.closeThis()
        {
            onChanged = null;
        }
    }
}