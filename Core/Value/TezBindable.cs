using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 自带通知的类型
    /// </summary>
	public class TezBindable<T>
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
	
	    public void close()
	    {
	        onChanged = null;
	        innerValue = default;
	    }
	}
}