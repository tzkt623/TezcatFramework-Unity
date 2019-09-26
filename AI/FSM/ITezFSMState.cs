using tezcat.Framework.Core;
using tezcat.Framework.Event;

namespace tezcat.Framework.AI
{
    public interface ITezFSMState<TData>
        : ITezCloseable
        where TData : class, ITezFSMData, new()
    {
        TezFSM<TData> FSM { get; set; }

        void enter(TData data);
        void exit(TData data);

        void execute(TData data);

        bool onEvent(ITezEventData evt);
    }
}