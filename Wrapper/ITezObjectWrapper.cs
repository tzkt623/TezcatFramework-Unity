namespace tezcat.Wrapper
{
    public interface ITezObjectWrapper
    {
        void bind(TezObjectWrapper wrapper);
    }

    public interface ITezObjectWrapper<Wrapper> where Wrapper : TezObjectWrapper
    {
        void bind(Wrapper wrapper);
    }
}