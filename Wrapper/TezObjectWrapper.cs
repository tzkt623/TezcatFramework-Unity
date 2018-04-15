namespace tezcat.Wrapper
{
    public interface ITezObjectWrapper
    {
        string name { get; }
        string description { get; }
        void clear();
    }

    public abstract class TezObjectWrapper : ITezObjectWrapper
    {
        public abstract string name { get; }
        public abstract string description { get; }
        public abstract void clear();
    }
}