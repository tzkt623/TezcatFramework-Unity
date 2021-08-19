using tezcat.Framework.Core;
using tezcat.Framework.Event;

namespace tezcat.Framework.AI
{
    public abstract class TezBaseFSMState<TBlackboard>
        : ITezCloseable
        where TBlackboard : TezBaseFSMBlackboard
    {
        public abstract void enter(TBlackboard blackboard);
        public abstract void exit(TBlackboard blackboard);

        public abstract void pause(TBlackboard blackboard);
        public abstract void resume(TBlackboard blackboard);

        public abstract void execute(TBlackboard blackboard);

        public abstract bool onEvent(ITezEventData eventData);
        public abstract void close();
    }
}