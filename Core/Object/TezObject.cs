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
        public abstract void close(bool self_close = true);


        #region 重载操作
        public static bool operator true(TezObject obj)
        {
            return !object.ReferenceEquals(obj, null);
        }

        public static bool operator false(TezObject obj)
        {
            return object.ReferenceEquals(obj, null);
        }

        public static bool operator !(TezObject obj)
        {
            return object.ReferenceEquals(obj, null);
        }
        #endregion
    }
}