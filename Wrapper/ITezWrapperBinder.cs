namespace tezcat.Wrapper
{

    /// <summary>
    /// 此控件需要绑定一个Wrapper
    /// </summary>
    public interface ITezWrapperBinder
    {
        void bind(ITezObjectWrapper wrapper);
    }

    /// <summary>
    /// 此控件需要绑定一个Wrapper
    /// </summary>
    /// <typeparam name="Wrapper"></typeparam>
    public interface ITezWrapperBinder<Wrapper> where Wrapper : ITezObjectWrapper
    {
        void bind(Wrapper wrapper);
    }
}