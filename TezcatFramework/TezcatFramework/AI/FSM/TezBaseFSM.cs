using tezcat.Framework.Core;
using tezcat.Framework.Event;

namespace tezcat.Framework.AI
{
    public abstract class TezBaseFSM<TBlackboard, TState>
        : ITezCloseable
        where TState : TezBaseFSMState<TBlackboard>
        where TBlackboard : TezBaseFSMBlackboard
    {
        public TState currentState => m_CurrentState;
        public TBlackboard blackboard => m_Blackboard;

        protected TBlackboard m_Blackboard = null;
        protected TState m_CurrentState = null;

        public virtual void setBlackboard(TBlackboard blackboard)
        {
            m_Blackboard = blackboard;
            m_Blackboard.init();
        }

        public virtual void execute()
        {
            m_CurrentState.execute(m_Blackboard);
        }

        public virtual void change(TState state)
        {
            m_CurrentState?.exit(m_Blackboard);
            m_CurrentState = state;
            m_CurrentState.enter(m_Blackboard);
        }

        public virtual void dispatchEvent(ITezEventData eventData)
        {
            m_CurrentState.onEvent(eventData);
        }

        public virtual void close()
        {
            m_CurrentState.close();
            m_Blackboard.close();
            m_Blackboard = null;
        }
    }
}