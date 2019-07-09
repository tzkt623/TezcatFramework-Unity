using tezcat.Framework.Core;

namespace tezcat.Framework.AI
{
    public interface ITezFSMState<TData>
        : ITezCloseable
        where TData : class, ITezFSMData
    {
        TezFSM<TData> FSM { get; set; }

        void enter(TData data);
        void exit(TData data);

        void execute(TData data);
    }
}