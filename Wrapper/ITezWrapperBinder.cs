namespace tezcat.Wrapper
{
    public interface ITezWrapperBinder
    {
        void bind(ITezObjectWrapper wrapper);
    }

    public interface ITezWrapperBinder<Wrapper> where Wrapper : ITezObjectWrapper
    {
        void bind(Wrapper wrapper);
    }
}