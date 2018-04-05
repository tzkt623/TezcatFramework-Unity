namespace tezcat.Wrapper
{
    public interface ITezObjectWrapper
    {
        void clear();
    }

    public abstract class TezObjectWrapper : ITezObjectWrapper
    {
        public abstract void clear();
    }
}