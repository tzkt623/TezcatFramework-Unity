namespace tezcat.Framework.Core
{
    /// <summary>
    /// 可关闭对象
    /// </summary>
    public interface ITezCloseable
    {

        /// <summary>
        /// 关闭并删除整个对象
        /// </summary>
        /// <param name="self_close">区分是自己关闭自己 还是包含此类的类关闭时顺带关闭</param>
        void close(bool self_close = true);
    }
}
