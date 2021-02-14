namespace tezcat.Framework.Core
{
    /// <summary>
    /// 基础Object
    /// </summary>
    public abstract class TezObject : ITezCloseable
    {
        public string CID
        {
            get { return this.GetType().Name; }
        }

        /// <summary>
        /// 删除Object时调用
        /// </summary>
        public abstract void close();
    }
}