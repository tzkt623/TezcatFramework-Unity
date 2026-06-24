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
        void close();
    }

    /// <summary>
    /// 不允许手动释放的对象
    /// </summary>
    public interface ITezNonCloseable
    {

    }

    //public static class TezCloseableHelper
    //{
    //    public static void close(this ITezCloseable closeable)
    //    {
    //        closeable.close();
    //    }
    //}
}
