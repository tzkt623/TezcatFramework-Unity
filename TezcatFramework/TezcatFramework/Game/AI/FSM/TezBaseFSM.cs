using tezcat.Framework.Core;
using tezcat.Framework.Event;

namespace tezcat.Framework.AI
{
    public abstract class TezBaseFSM<TBlackboard, TState>
        : ITezCloseable
        where TState : TezBaseFSMState<TBlackboard>
        where TBlackboard : TezBaseFSMBlackboard
    {
        public TState currentState => mCurrentState;
        public TBlackboard blackboard => mBlackboard;

        protected TBlackboard mBlackboard = null;
        protected TState mCurrentState = null;

        public virtual void setBlackboard(TBlackboard blackboard)
        {
            mBlackboard = blackboard;
            mBlackboard.init();
        }

        public virtual void execute()
        {
            mCurrentState.execute(mBlackboard);
        }

        public virtual void change(TState state)
        {
            mCurrentState?.exit(mBlackboard);
            mCurrentState = state;
            mCurrentState.enter(mBlackboard);
        }

        public virtual void dispatchEvent(ITezEventData eventData)
        {
            mCurrentState.onEvent(eventData);
        }

        public virtual void close()
        {
            mCurrentState.close();
            mBlackboard.close();
            mBlackboard = null;
        }
    }
}