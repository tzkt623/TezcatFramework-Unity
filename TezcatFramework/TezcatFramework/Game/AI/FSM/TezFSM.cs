﻿using tezcat.Framework.Event;

namespace tezcat.Framework.AI
{
    public class TezFSM<TBlackboard, TState>
        : TezBaseFSM<TBlackboard, TState>
        where TState : TezBaseFSMState<TBlackboard>
        where TBlackboard : TezBaseFSMBlackboard
    {
        TState m_GlobalState = null;
        TBlackboard m_Blackboard = null;

        public override void execute()
        {
            m_GlobalState?.execute(m_Blackboard);
            mCurrentState?.execute(m_Blackboard);
        }

        public void setGlobalState(TState state)
        {
            m_GlobalState = state;
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

            if (m_GlobalState != null && m_GlobalState.onEvent(eventData))
            {
                return;
            }
        }

        public override void close()
        {
            base.close();

            m_GlobalState?.close();
            m_GlobalState = null;
        }
    }
}