namespace tezcat.Framework.Utility
{
    public abstract class TezSingleton<T> where T : class, new()
    {
        public static readonly T instance = new T();
    }
}