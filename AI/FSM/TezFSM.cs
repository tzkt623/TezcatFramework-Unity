using tezcat.Framework.Core;
using tezcat.Framework.Event;

namespace tezcat.Framework.AI
{
    public class TezFSM<TData>
        : ITezCloseable
        where TData : class, ITezFSMData, new()
    {
        ITezFSMState<TData> m_GlobalState = null;
        ITezFSMState<TData> m_CurrentState = null;
        ITezFSMState<TData> m_PreviousState = null;

        TData m_Data = null;

        public void onEvent(ITezEventData evt)
        {
            if(m_CurrentState != null && m_CurrentState.onEvent(evt))
            {
                return;
            }

            if(m_GlobalState != null && m_GlobalState.onEvent(evt))
            {
                return;
            }
        }

        public void execute()
        {
            m_GlobalState?.execute(m_Data);
            m_CurrentState?.execute(m_Data);
        }

        public void setData(TData data)
        {
            m_Data = data;
        }

        public void setCurrentState(ITezFSMState<TData> state)
        {
            m_CurrentState = state;
            m_CurrentState.FSM = this;
        }

        public void setGlobalState(ITezFSMState<TData> state)
        {
            m_GlobalState = state;
            m_GlobalState.FSM = this;
        }

        public void changeState(ITezFSMState<TData> state)
        {
            m_PreviousState?.close();
            m_PreviousState = m_CurrentState;
            m_CurrentState.exit(m_Data);

            m_CurrentState = state;
            m_CurrentState.FSM = this;
            m_CurrentState.enter(m_Data);
        }

        public void revertState()
        {
            this.changeState(m_PreviousState);
        }

        public virtual void close()
        {
            m_Data.close();
            m_CurrentState?.close();
            m_GlobalState?.close();
            m_PreviousState?.close();

            m_Data = null;
            m_CurrentState = null;
            m_GlobalState = null;
            m_PreviousState = null;
        }
    }
}