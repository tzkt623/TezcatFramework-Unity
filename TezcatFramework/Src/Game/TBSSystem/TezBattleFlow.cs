using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game.TBSSystem
{
    /// <summary>
    /// 回合制战斗的时间点(一次攻击行为中的 5 个阶段 + 1 个内部收尾占位)
    /// </summary>
    public enum TezBattleTiming
    {
        /// <summary>
        /// 1.攻击者攻击前:启动攻击者的"攻击前"技能
        /// </summary>
        BeforeAttack = 0,

        /// <summary>
        /// 2.被攻击者攻击前:启动被攻击者的"被攻击前"技能
        /// (闪避、格挡、抢先反击都在这里)
        /// </summary>
        BeforeBeAttacked = 1,

        /// <summary>
        /// 3.
        /// </summary>
        Run = 2,

        /// <summary>
        /// 4.被攻击者攻击后:启动被攻击者的"被攻击后"技能
        /// </summary>
        AfterBeAttacked = 3,

        /// <summary>
        /// 5.攻击者攻击后:启动攻击者的"攻击后"技能
        /// </summary>
        AfterAttack = 4,
    }


    public interface ITezBattleData
    {
        TezBattleTiming timing { get; }
        TezTags tags { get; }
    }

    public class TezBattleData : ITezBattleData
    {
        public TezBattleTiming timing { get; }
        public TezTags tags { get; } = new TezTags();
    }

    public interface ITezBattleAction
    {
        void stepBeforeSourceUse(TezStepGroup me);
        void stepBeforeUseNotifyTarget(TezStepGroup me);
        void stepUse(TezStepGroup me);
        void stepAfterUseNotifyTarget(TezStepGroup me);
        void stepAfterAttack(TezStepGroup me);
        void stepEnd(TezStepGroup me);
    }

    public class TezBattleContext
    {
        public bool isisCancelled = false;
        public TezBattleTiming timing = TezBattleTiming.BeforeAttack;

        public object sourceUnit;
        public List<object> targetUnits = new List<object>();
        public object mainTargetUnit => targetUnits[0];
    }

    public class TezBattleAction : ITezBattleAction
    {
        public event Action evtFinished;

        TezBattleContext mBattleContext = new TezBattleContext();

        public void stepBeforeSourceUse(TezStepGroup me)
        {
            this.runPhase(me, TezBattleTiming.BeforeAttack);
        }

        public void stepBeforeUseNotifyTarget(TezStepGroup me)
        {
            this.runPhase(me, TezBattleTiming.BeforeBeAttacked);
        }

        public void stepUse(TezStepGroup me)
        {
            this.runPhase(me, TezBattleTiming.Run);
        }

        public void stepAfterUseNotifyTarget(TezStepGroup me)
        {
            this.runPhase(me, TezBattleTiming.AfterBeAttacked);
        }

        public void stepAfterAttack(TezStepGroup me)
        {
            this.runPhase(me, TezBattleTiming.AfterAttack);
        }

        public void stepEnd(TezStepGroup me)
        {
            this.runPhase(me, TezBattleTiming.AfterAttack);
        }


        protected void runPhase(TezStepGroup me, TezBattleTiming timing)
        {
            mBattleContext.timing = timing;

            if (mBattleContext.isisCancelled)
            {
                me.setWishedTiming(TezStepWishedTiming.Abort);
                return;
            }

            this.notifySkills(me, timing);
        }

        protected void notifySkills(TezStepGroup me, TezBattleTiming timing)
        {

        }
    }

    public static class TezBattleHelper
    {
        public static TezStepGroup createGroup(this ITezBattleAction action)
        {
            var group = new TezStepGroup();
            group.addStep(action.stepBeforeSourceUse);
            group.addStep(action.stepBeforeUseNotifyTarget);
            group.addStep(action.stepUse);
            group.addStep(action.stepAfterUseNotifyTarget);
            group.addStep(action.stepAfterAttack);
            // 末位占位:保证 5 个阶段都不是"最后一步",否则阶段 5 里 wait 会越界。
            // 它同时充当"本行为正常结束"的回调点。
            group.addStep(action.stepEnd);
            return group;
        }
    }

    public class TezBattleSystem
    {
        TezStepSystem2 mStepSystem = new TezStepSystem2();
        Queue<TezBattleAction> mActions = new Queue<TezBattleAction>();
        TezBattleAction mCurrent = null;

        public void update()
        {
            if (mCurrent == null && mActions.Count > 0)
            {
                mCurrent = mActions.Dequeue();
                mCurrent.evtFinished += onActionFinished;
                mStepSystem.runImmediately<TezSequenceStepExecutor>(mCurrent.createGroup());
            }

            mStepSystem.update();
        }

        private void onActionFinished()
        {
            throw new NotImplementedException();
        }
    }
}