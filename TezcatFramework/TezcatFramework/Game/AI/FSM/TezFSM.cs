using tezcat.Framework.Event;

namespace tezcat.Framework.AI
{
    public class TezFSM<TBlackboard, TState>
        : TezBaseFSM<TBlackboard, TState>
        where TState : TezBaseFSMState<TBlackboard>
        where TBlackboard : TezBaseFSMBlackboard
    {
        TState mGlobalState = null;
        TBlackboard m_Blackboard = null;

        public override void execute()
        {
            mGlobalState?.execute(m_Blackboard);
            mCurrentState?.execute(m_Blackboard);
        }

        public void setGlobalState(TState state)
        {
            mGlobalState = state;
        }

        public void changeState(TState state)
        {
            mCurrentState?.exit(m_Blackboard);
            mCurrentState = state;
            mCurrentState.enter(m_Blackboard);
        }

        public override void dispatchEvent(ITezEventData eventData)
        {
            if (mCurrentState != null && mCurrentState.onEvent(eventData))
            {
                return;
            }

            if (mGlobalState != null && mGlobalState.onEvent(eventData))
            {
                return;
            }
        }

        public override void close()
        {
            base.close();

            mGlobalState?.close();
            mGlobalState = null;
        }
    }
}