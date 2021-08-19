using System;
using tezcat.Framework.AI;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 状态
    /// </summary>
    public abstract class TezGameMachineState<TBlackborad>
        : TezBaseFSMState<TBlackborad>
        where TBlackborad : TezBaseFSMBlackboard
    {
        public int pauseCount => m_PauseCount;
        int m_PauseCount;

        public override void pause(TBlackborad blackboard)
        {
            m_PauseCount++;
        }

        public override void resume(TBlackborad blackboard)
        {
            m_PauseCount--;
            if (m_PauseCount < 0)
            {
                throw new ArgumentOutOfRangeException("PauseCount < 0");
            }
        }
    }
}