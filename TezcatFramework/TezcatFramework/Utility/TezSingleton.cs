namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 快捷单例类
    /// 
    /// 另一个单例管理器为Service
    /// </summary>
    public abstract class TezSingleton<T> where T : class, new()
    {
        public static readonly T instance = new T();

        protected TezSingleton()
        {
            if (instance != null)
            {
                if (instance != this)
                {
                    throw new System.Exception(string.Format("{0} : Can not new Singleton Class!!", typeof(T).Name));
                }
            }
        }
    }
}