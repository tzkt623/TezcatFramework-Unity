namespace tezcat.Framework.Core
{
    public interface ITezBinder
    {
        
    }

    public interface ITezBinder<T> : ITezBinder
    {
        void bind(T value);
    }
}