using System;
using tezcat.Framework.AI;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 状态
    /// </summary>
    public abstract class TezGameMachineState<TBlackborad, Self>
        : TezBaseFSMState<TBlackborad>
        where TBlackborad : TezBaseFSMBlackboard
        where Self : TezGameMachineState<TBlackborad, Self>
    {
        public ITezGameMachine<TBlackborad, Self> gameMachine { get; set; }

        int mPauseCount = 0;
        public int pauseCount => mPauseCount;

        int mPushCount = 0;
        public int pushCount => mPushCount;

        int mMarkPopCount = 0;

        public bool needMarkPop()
        {
            if (mMarkPopCount > 0)
            {
                mMarkPopCount--;
                return true;
            }

            return false;
        }

        ///标记弹出
        ///用于在A状态中添加B状态后
        ///再弹出A状态
        public void markPop()
        {
            if (mPushCount == 0)
            {
                throw new Exception(string.Format("TezGameMachineState : This State Not In Stack [{0}]", this.GetType().Name));
            }

            mMarkPopCount++;
            if(mMarkPopCount > 50)
            {
                throw new Exception(string.Format("TezGameMachineState : MarkPopo > 50!!!", this.GetType().Name));
            }
        }

        public override void enter(TBlackborad blackboard)
        {
            mPushCount++;
        }

        public override void exit(TBlackborad blackboard)
        {
            mPushCount--;
        }

        public override void pause(TBlackborad blackboard)
        {
            mPauseCount++;
        }

        public override void resume(TBlackborad blackboard)
        {
            mPauseCount--;
            if (mPauseCount < 0)
            {
                throw new ArgumentOutOfRangeException("PauseCount < 0");
            }
        }

        public override void close()
        {
            this.gameMachine = null;
        }
    }
}