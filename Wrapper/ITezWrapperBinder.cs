using tezcat.Framework.Core;

namespace tezcat.Framework.Wrapper
{

    /// <summary>
    /// 此控件需要绑定一个Wrapper
    /// </summary>
    public interface ITezWrapperBinder
    {
        void bind(ITezWrapper wrapper);
        void bind(TezBaseDataSlot slot);
    }

    public interface ITezWrapperBinder<Wrapper>
        where Wrapper : ITezWrapper
    {
        void bind(Wrapper wrapper);
    }

    /// <summary>
    /// 此控件需要绑定一个Wrapper
    /// </summary>
    /// <typeparam name="Wrapper"></typeparam>
    public interface ITezWrapperBinder<Wrapper, Slot>
        where Wrapper : ITezWrapper
        where Slot : TezBaseDataSlot
    {
        void bind(Wrapper wrapper);
        void bind(Slot slot);
    }
}