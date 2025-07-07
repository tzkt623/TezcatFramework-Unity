using System;
using System.Collections.Generic;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 
    /// 命令列表基本设计
    /// 1.支持命令插入
    /// 2.支持命令排序
    /// 3.支持命令取消
    /// 
    /// 命令的生成方式
    /// 1.实时命令生成
    /// 2.预生成
    /// 
    /// 实时生成命令
    /// 
    /// 例子.英雄无敌里面的抢先反击,我触发了攻击动作,但是敌人触发抢先反击,需要先攻击
    /// 
    /// 
    /// </summary>

    public enum TezTriggerState
    {
        Error,
        /// <summary>
        /// 执行下个阶段
        /// </summary>
        NextPhase,
        /// <summary>
        /// 当前功能成功执行完成
        /// 后续功能会继续执行
        /// </summary>
        Success,
        /// <summary>
        /// 当前功能执行失败
        /// 后续功能不会执行
        /// </summary>
        Fail,
        /// <summary>
        /// 等待其他指令完成
        /// </summary>
        Waiting,
    }


    public static class TezTriggerListSystem
    {
        static List<ITezTrigger> mTriggerList = new List<ITezTrigger>();

        public static void addTrigger(ITezTrigger trigger)
        {
            mTriggerList.Add(trigger);
        }

        public static void update()
        {
            for (int i = mTriggerList.Count - 1; i >= 0; i--)
            {
                var trigger = mTriggerList[i];
                switch (trigger.state)
                {
                    case TezTriggerState.Error:
                        throw new Exception();
                    case TezTriggerState.Success:
                    case TezTriggerState.Fail:
                    case TezTriggerState.Waiting:
                        mTriggerList.RemoveAt(i);
                        break;
                    case TezTriggerState.NextPhase:
                        trigger.execute();
                        break;
                    default:
                        break;
                }
            }
        }
    }

    public interface ITezTriggerPhase
    {
        TezTriggerState execute();
    }

    public interface ITezTrigger
    {
        TezTriggerState state { get; }
        void execute();
    }

    public interface ITezTriggerUserData
    {

    }

    /// <summary>
    /// 
    /// 一个trigger里可能存在很多个阶段
    /// 例如,播放攻击动画阶段,攻击判定阶段,攻击触发阶段,攻击后阶段
    /// trigger必须要有暂停等待和取消后续阶段的能力
    /// 
    /// 例如抢先反击能力,需要攻击者先发起询问,被攻击者触发抢先反击技能
    /// 
    /// 
    /// </summary>
    public class TezTrigger : ITezTrigger
    {
        public class Phase
        {
            public Action<TezTrigger> onExecute;
        }

        Action mOnComplete;
        TezLinkedList<Phase> mPhaseList = new TezLinkedList<Phase>();
        TezLinkedList<TezTrigger> mBeTriggeredList = new TezLinkedList<TezTrigger>();

        ITezTriggerUserData mUserData = null;
        public ITezTriggerUserData userData { get => mUserData; set=> mUserData = value; }

        TezTrigger mMasterTrigger = null;

        TezTriggerState mState = TezTriggerState.Error;
        public TezTriggerState state => mState;

        public TezTrigger()
        {

        }

        public TezTrigger(TezTrigger master)
        {
            if(master == this)
            {
                throw new ArgumentNullException(nameof(master));
            }

            mMasterTrigger = master;
            mMasterTrigger?.wait(this);
        }

        public void setOnComplete(Action funcComplete)
        {
            mOnComplete = funcComplete;
        }

        public void wait(TezTrigger trigger)
        {
            mBeTriggeredList.addBack(trigger);
        }

        public void resume()
        {
            mState = TezTriggerState.NextPhase;
            TezTriggerListSystem.addTrigger(this);
        }

        public void pause()
        {
            mState = TezTriggerState.Waiting;
        }

        void ITezTrigger.execute()
        {
            switch (mState)
            {
                case TezTriggerState.NextPhase:
                    if (mBeTriggeredList.count > 0)
                    {
                        var trigger = mBeTriggeredList.popFront().value;
                        TezTriggerListSystem.addTrigger(trigger);
                        mState = TezTriggerState.Waiting;
                        //Console.WriteLine("Waiting......");
                    }
                    else
                    {
                        if(mPhaseList.count == 0)
                        {
                            mState = TezTriggerState.Success;
                            mOnComplete?.Invoke();
                            if (mMasterTrigger != null)
                            {
                                mMasterTrigger.resume();
                            }
                        }
                        else
                        {
                            mPhaseList.popFront().value.onExecute(this);
                        }
                    }
                    break;
                case TezTriggerState.Fail:
                    if (mMasterTrigger != null)
                    {
                        mOnComplete?.Invoke();
                        mMasterTrigger.resume();
                    }
                    break;
                default:
                    throw new Exception("");
            }
        }

        public Phase addPhase(Action<TezTrigger> func)
        {
            Phase phase = new Phase()
            {
                onExecute = func
            };
            mPhaseList.addBack(phase);
            return phase;
        }

        public void run()
        {
            mState = TezTriggerState.NextPhase;
            if (mMasterTrigger != null)
            {
                return;
            }

            TezTriggerListSystem.addTrigger(this);
        }
    }

    public abstract class TezCommandSequence
    {
        int mIndex = 0;
        List<TezCommand> mCommandList = new List<TezCommand>();

        public void add(TezCommand command)
        {
            command.sequence = this;
            command.ID = mCommandList.Count;
            mCommandList.Add(command);
        }

        public void remove(TezCommand command)
        {
            var last = mCommandList[mCommandList.Count - 1];
        }

        public void nextCommand()
        {
            mCommandList[mIndex].onEnter();
        }

        public void commandExit(TezCommand command)
        {
            mIndex = command.ID + 1;
            if (mIndex >= mCommandList.Count)
            {
                mIndex = 0;
                this.onSequenceEnd();
            }
            else
            {
                mCommandList[mIndex].onEnter();
            }
        }

        protected abstract void onSequenceBegin();
        protected abstract void onSequenceEnd();
    }
}

