namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 快捷单例类
    /// 
    /// 另一个单例管理器为Service
    /// </summary>
    public class TezSingleton<T> where T : class, new()
    {
        public static readonly T instance = new T();

        protected TezSingleton()
        {
            if (instance != null)
            {
                if (instance != this)
                {
                    throw new System.Exception($"{typeof(T).Name} : Can not new Singleton Class!!");
                }
            }
        }
    }
}