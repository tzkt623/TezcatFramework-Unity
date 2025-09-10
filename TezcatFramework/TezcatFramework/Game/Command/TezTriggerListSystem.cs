using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
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
                //trigger.execute();
                switch (trigger.state)
                {
                    case TezTriggerState.Error:
                        throw new Exception();
                    case TezTriggerState.Success:
                        mTriggerList.RemoveAt(i);
                        break;
                    case TezTriggerState.Fail:
                        mTriggerList.RemoveAt(i);
                        trigger.onFail();
                        break;
                    case TezTriggerState.Waiting:
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

    public interface ITezTriggerPhaseID
    {
        ulong ID { get; }
    }

    public interface ITezTrigger
    {
        TezTriggerState state { get; }
        ITezTriggerPhaseID phaseID { get; set; }
        void execute();
        void onFail();
        void wait(ITezTrigger trigger);
        void resume();
        void pause();
    }

    public interface ITezTrigger<T> : ITezTrigger
    {
        T userData { get; set; }
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
    /// 

    public abstract class TezBasicTrigger : ITezTrigger, ITezCloseable
    {
        protected interface IPhase
        {
            void execute(ITezTrigger trigger);
        }

        Action<TezTriggerState> mOnComplete;
        protected TezLinkedList<IPhase> mPhaseList = new TezLinkedList<IPhase>();
        TezLinkedList<ITezTrigger> mBeTriggeredList = new TezLinkedList<ITezTrigger>();

        ITezTrigger mMasterTrigger = null;

        TezTriggerState mState = TezTriggerState.Error;
        public TezTriggerState state => mState;
        public ITezTriggerPhaseID phaseID { get; set; }


        public TezBasicTrigger setOnComplete(Action<TezTriggerState> funcComplete)
        {
            mOnComplete = funcComplete;
            return this;
        }

        public void wait(ITezTrigger trigger)
        {
            mBeTriggeredList.addBack(trigger);
        }

        public void resume()
        {
            mState = TezTriggerState.NextPhase;
            //TezTriggerListSystem.addTrigger(this);
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
                    }
                    else
                    {
                        if (mPhaseList.count == 0)
                        {
                            mState = TezTriggerState.Success;
                            mOnComplete?.Invoke(mState);
                            if (mMasterTrigger != null)
                            {
                                mMasterTrigger.resume();
                            }
                        }
                        else
                        {
                            mPhaseList.popFront().value.execute(this);
                        }
                    }
                    break;
                case TezTriggerState.Fail:
                    mOnComplete?.Invoke(mState);
                    if (mMasterTrigger != null)
                    {
                        mMasterTrigger.resume();
                    }
                    break;
                case TezTriggerState.Waiting:
                    break;
                default:
                    throw new Exception("");
            }
        }

        public void run(ITezTrigger masterTrigger)
        {
            if (masterTrigger == this)
            {
                throw new ArgumentNullException(nameof(masterTrigger));
            }

            mState = TezTriggerState.NextPhase;
            mMasterTrigger = masterTrigger;
            if (mMasterTrigger != null)
            {
                mMasterTrigger.wait(this);
            }
            else
            {
                TezTriggerListSystem.addTrigger(this);
            }
        }

        public void fail()
        {
            mState = TezTriggerState.Fail;
        }

        void ITezTrigger.onFail()
        {
            mOnComplete?.Invoke(mState);
            if (mMasterTrigger != null)
            {
                mMasterTrigger.resume();
            }
        }

        public void close()
        {
            mBeTriggeredList.close();
            mPhaseList.close();
            this.onCloseThis();

            mBeTriggeredList = null;
            mPhaseList = null;
            mOnComplete = null;
            mMasterTrigger = null;
        }

        protected abstract void onCloseThis();
    }

    public class TezTrigger : TezBasicTrigger
    {
        public class Phase : IPhase
        {
            public Action<ITezTrigger> onExecute;

            void IPhase.execute(ITezTrigger trigger)
            {
                this.onExecute(trigger);
            }
        }

        public TezTrigger addPhase(Action<ITezTrigger> func)
        {
            Phase phase = new Phase()
            {
                onExecute = func
            };
            mPhaseList.addBack(phase);
            return this;
        }

        protected override void onCloseThis()
        {
        }
    }


    public class TezTrigger<T> : TezBasicTrigger, ITezTrigger<T>
    {
        public class Phase : IPhase
        {
            public Action<ITezTrigger<T>> onExecute;

            void IPhase.execute(ITezTrigger trigger)
            {
                this.onExecute((ITezTrigger<T>)trigger);
            }
        }

        T mUserData;
        public T userData { get => mUserData; set => mUserData = value; }

        public TezTrigger<T> addPhase(Action<ITezTrigger<T>> func)
        {
            Phase phase = new Phase()
            {
                onExecute = func
            };
            mPhaseList.addBack(phase);
            return this;
        }

        protected override void onCloseThis()
        {
            mUserData = default;
        }
    }
}