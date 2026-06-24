using System;
using System.Collections.Generic;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    public static class TezTrigger
    {
        static List<IContainer> mContainerList = new List<IContainer>();

        public static OnlyOne onlyOne()
        {
            var container = new OnlyOne();
            return container;
        }

        public static Sequence sequence()
        {
            var container = new Sequence();
            return container;
        }

        public static Parallel parallel()
        {
            var container = new Parallel();
            return container;
        }

        private static void add(IContainer container)
        {
            mContainerList.Add(container);
        }

        public static void update()
        {
            for (int i = mContainerList.Count - 1; i >= 0; i--)
            {
                var container = mContainerList[i];
                if (!container.update())
                {
                    mContainerList.RemoveAt(i);
                    container.close();
                }
            }
        }

        public static Trigger createTrigger()
        {
            return Trigger.create();
        }

        public static Trigger<T> createTrigger<T>()
        {
            return Trigger<T>.create();
        }

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

        public enum TriggerState
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

        public interface IVisitorBase
        {

        }

        public class SequenceVisitor : IVisitorBase
        {
            Action<List<ITrigger>> mVisitor;

            public SequenceVisitor(Action<List<ITrigger>> visitor)
            {
                mVisitor = visitor;
            }

            public void visit(List<ITrigger> sequence)
            {
                mVisitor(sequence);
            }
        }

        public class ParallelVisitor : IVisitorBase
        {
            Action<List<ITrigger>> mVisitor;

            public ParallelVisitor(Action<List<ITrigger>> visitor)
            {
                mVisitor = visitor;
            }

            public void visit(List<ITrigger> sequence)
            {
                mVisitor(sequence);
            }
        }

        public interface IContainer
        {
            IContainer add(IContainerItem trigger);
            void trigger(ITrigger trigger);
            bool update();
            void close();
            void accept(IVisitorBase visitor);
            void run();
        }

        public class OnlyOne : IContainer
        {
            ITrigger mTrigger = null;

            public void accept(IVisitorBase visitor)
            {
                throw new NotImplementedException();
            }

            public IContainer add(IContainerItem trigger)
            {
                mTrigger = trigger;
                return this;
            }

            public void close()
            {
                mTrigger?.recycle();
                mTrigger = null;
            }

            public void run()
            {
                TezTrigger.add(this);
            }

            public void trigger(ITrigger trigger)
            {
                throw new NotImplementedException();
            }

            public bool update()
            {
                if (mTrigger == null)
                {
                    return false;
                }

                switch (mTrigger.state)
                {
                    case TriggerState.Error:
                        throw new Exception();
                    case TriggerState.Success:
                        mTrigger.onSuccess();
                        mTrigger = null;
                        break;
                    case TriggerState.Fail:
                        mTrigger.onFail();
                        mTrigger = null;
                        break;
                    case TriggerState.Waiting:
                        break;
                    case TriggerState.NextPhase:
                        mTrigger.execute();
                        break;
                    default:
                        break;
                }

                return true;
            }
        }

        public class Sequence : IContainer
        {
            int mMainIndex = 0;
            int mSecondIndex = -1;
            List<ITrigger> mMainSequence = new List<ITrigger>();
            List<ITrigger> mSecondSequence = new List<ITrigger>();

            public void accept(IVisitorBase visitor)
            {
                ((SequenceVisitor)visitor).visit(mMainSequence);
            }

            public IContainer add(IContainerItem trigger)
            {
                mMainSequence.Add(trigger);
                trigger.onAddContainer(this);
                return this;
            }

            void IContainer.trigger(ITrigger trigger)
            {
                mSecondSequence.Add(trigger);
                mSecondIndex = mSecondSequence.Count - 1;
            }

            public void close()
            {
                mMainSequence.Clear();
                mSecondSequence.Clear();
            }

            public bool update()
            {
                if (mSecondSequence.Count > 0)
                {
                    var trigger = mSecondSequence[mSecondIndex];
                    switch (trigger.state)
                    {
                        case TriggerState.Error:
                            throw new Exception();
                        case TriggerState.Success:
                            trigger.onSuccess();
                            mSecondSequence.RemoveAt(mSecondIndex);
                            mSecondIndex = mSecondSequence.Count - 1;
                            break;
                        case TriggerState.Fail:
                            trigger.onFail();
                            mSecondSequence.RemoveAt(mSecondIndex);
                            mSecondIndex = mSecondSequence.Count - 1;
                            break;
                        case TriggerState.Waiting:
                            break;
                        case TriggerState.NextPhase:
                            trigger.execute();
                            break;
                        default:
                            break;
                    }

                    return true;
                }
                else
                {
                    var trigger = mMainSequence[mMainIndex];
                    switch (trigger.state)
                    {
                        case TriggerState.Error:
                            throw new Exception();
                        case TriggerState.Success:
                            trigger.onSuccess();
                            mMainIndex++;
                            break;
                        case TriggerState.Fail:
                            trigger.onFail();
                            mMainIndex++;
                            break;
                        case TriggerState.Waiting:
                            break;
                        case TriggerState.NextPhase:
                            trigger.execute();
                            break;
                        default:
                            break;
                    }

                    return mMainIndex < mMainSequence.Count;
                }
            }

            public void run()
            {
                TezTrigger.add(this);
            }
        }

        public class TezTriggerStack : IContainer
        {
            int mIndex = 0;
            List<ITrigger> mSequence = new List<ITrigger>();

            public void accept(IVisitorBase visitor)
            {

            }

            public IContainer add(IContainerItem trigger)
            {
                mSequence.Add(trigger);
                trigger.onAddContainer(this);
                mIndex++;
                return this;
            }

            void IContainer.trigger(ITrigger trigger)
            {

            }

            public void run()
            {
                TezTrigger.add(this);
            }

            public void close()
            {
                mSequence.Clear();
            }

            public bool update()
            {
                if (mIndex >= mSequence.Count)
                {
                    return false;
                }

                var trigger = mSequence[mIndex];
                switch (trigger.state)
                {
                    case TriggerState.Error:
                        throw new Exception();
                    case TriggerState.Success:
                        trigger.onSuccess();
                        mIndex--;
                        break;
                    case TriggerState.Fail:
                        trigger.onFail();
                        mIndex--;
                        break;
                    case TriggerState.Waiting:
                        break;
                    case TriggerState.NextPhase:
                        trigger.execute();
                        break;
                    default:
                        break;
                }

                return true;
            }
        }

        public class Parallel : IContainer
        {
            List<ITrigger> mParallelList = new List<ITrigger>();

            public void accept(IVisitorBase visitor)
            {
                ((ParallelVisitor)visitor).visit(mParallelList);
            }

            public IContainer add(IContainerItem trigger)
            {
                mParallelList.Add(trigger);
                trigger.onAddContainer(this);
                return this;
            }

            void IContainer.trigger(ITrigger trigger)
            {
                mParallelList.Add(trigger);
            }

            public void run()
            {
                TezTrigger.add(this);
            }

            public void close()
            {
                mParallelList.Clear();
            }

            public bool update()
            {
                if (mParallelList.Count == 0)
                {
                    return false;
                }

                for (int i = mParallelList.Count - 1; i >= 0; i--)
                {
                    var trigger = mParallelList[i];
                    switch (trigger.state)
                    {
                        case TriggerState.Error:
                            throw new Exception();
                        case TriggerState.Success:
                            mParallelList.RemoveAt(i);
                            trigger.onSuccess();
                            break;
                        case TriggerState.Fail:
                            mParallelList.RemoveAt(i);
                            trigger.onFail();
                            break;
                        case TriggerState.Waiting:
                            break;
                        case TriggerState.NextPhase:
                            trigger.execute();
                            break;
                        default:
                            break;
                    }
                }

                return true;
            }
        }

        public interface ITriggerPhaseID
        {
            ulong ID { get; }
        }

        public interface ITrigger
        {
            IContainer masterContainer { get; }
            TriggerState state { get; }
            ITriggerPhaseID phaseID { get; set; }
            void onFail();
            void onSuccess();
            void execute();
            void recycle();
            void resume();
            void wait(ITrigger trigger);
            void run(ITrigger trigger);
        }

        public interface IContainerItem : ITrigger
        {
            void onAddContainer(IContainer container);
        }

        public interface ITrigger<T> : ITrigger
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
        /// </summary>

        public abstract class BaseTrigger
            : IContainerItem
        {
            Action<TriggerState> mOnComplete = null;
            TezLinkedList<ITrigger> mBeTriggeredList = new TezLinkedList<ITrigger>();
            ITrigger mMasterTrigger = null;
            TriggerState mState = TriggerState.Error;
            protected IContainer mMasterContainer = null;

            public TriggerState state => mState;
            public ITriggerPhaseID phaseID { get; set; }
            public IContainer masterContainer => mMasterContainer;

            protected BaseTrigger()
            {

            }

            public BaseTrigger setOnComplete(Action<TriggerState> funcComplete)
            {
                mOnComplete = funcComplete;
                return this;
            }

            private void wait(BaseTrigger trigger)
            {
                mBeTriggeredList.addBack(trigger);
            }

            public void resume()
            {
                mState = TriggerState.NextPhase;
                //TezTriggerListSystem.addTrigger(this);
            }

            public void wait()
            {
                mState = TriggerState.Waiting;
            }

            public void fail()
            {
                mState = TriggerState.Fail;
            }

            protected abstract bool isPhaseEmpty();

            protected abstract void executePhase();

            void ITrigger.execute()
            {
                switch (mState)
                {
                    case TriggerState.NextPhase:
                        if (mBeTriggeredList.count > 0)
                        {
                            mState = TriggerState.Waiting;
                            var child_trigger = mBeTriggeredList.popFront().value;
                            //TezTriggerSystem.addTrigger(child_trigger);
                            mMasterContainer.trigger(child_trigger);
                        }
                        else
                        {
                            if (this.isPhaseEmpty())
                            {
                                mState = TriggerState.Success;
                            }
                            else
                            {
                                this.executePhase();
                            }
                        }
                        break;
                    case TriggerState.Fail:
                        mState = TriggerState.Fail;
                        break;
                    case TriggerState.Waiting:
                        break;
                    default:
                        throw new Exception("");
                }
            }

            public void run(ITrigger masterTrigger)
            {
                if (masterTrigger == this)
                {
                    throw new ArgumentNullException(nameof(masterTrigger));
                }

                mState = TriggerState.NextPhase;
                mMasterTrigger = masterTrigger;
                mMasterContainer = masterTrigger.masterContainer;
                mMasterTrigger.wait(this);
            }

            void IContainerItem.onAddContainer(IContainer container)
            {
                if (mMasterContainer != null)
                {
                    throw new ArgumentNullException(nameof(mMasterContainer));
                }

                mState = TriggerState.NextPhase;
                mMasterContainer = container;
            }

            void ITrigger.onFail()
            {
                mOnComplete?.Invoke(TriggerState.Fail);
                mMasterTrigger?.resume();
                ((ITrigger)this).recycle();
            }

            void ITrigger.onSuccess()
            {
                mOnComplete?.Invoke(TriggerState.Success);
                mMasterTrigger?.resume();
                ((ITrigger)this).recycle();
            }

            void ITrigger.recycle()
            {
                while (mBeTriggeredList.count > 0)
                {
                    mBeTriggeredList.popFront().value.recycle();
                }

                mBeTriggeredList.clear();
                mOnComplete = null;
                mMasterTrigger = null;
                mMasterContainer = null;

                this.onRecycle();

                //mBeTriggeredList = null;
            }

            protected abstract void onRecycle();

            void ITrigger.wait(ITrigger trigger)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Trigger只要没有主动wait
        /// 就会一直执行下去
        /// 直到所有阶段执行完毕
        /// </summary>
        public class Trigger : BaseTrigger
        {
            private static Queue<Trigger> sPool = new Queue<Trigger>();

            public static Trigger create()
            {
                if (sPool.Count > 0)
                {
                    var trigger = sPool.Dequeue();
                    return trigger;
                }
                else
                {
                    return new Trigger();
                }
            }

            private Trigger() { }

            protected TezLinkedList<Action<Trigger>> mPhaseList = new TezLinkedList<Action<Trigger>>();

            public Trigger addPhase(Action<Trigger> func)
            {
                mPhaseList.addBack(func);
                return this;
            }

            protected override bool isPhaseEmpty()
            {
                return mPhaseList.count == 0;
            }

            protected override void executePhase()
            {
                mPhaseList.popFront().value(this);
            }

            protected override void onRecycle()
            {
                mPhaseList.clear();
                //mPhaseList = null;

                sPool.Enqueue(this);
            }
        }


        public class Trigger<T>
            : BaseTrigger
            , ITrigger<T>
        {
            private static Queue<Trigger<T>> sPool = new Queue<Trigger<T>>();

            public static Trigger<T> create()
            {
                if (sPool.Count > 0)
                {
                    var trigger = sPool.Dequeue();
                    return trigger;
                }
                else
                {
                    return new Trigger<T>();
                }
            }

            private Trigger() { }

            T mUserData;
            public T userData { get => mUserData; set => mUserData = value; }
            protected TezLinkedList<Action<Trigger<T>>> mPhaseList = new TezLinkedList<Action<Trigger<T>>>();

            public Trigger<T> addPhase(Action<Trigger<T>> func)
            {
                mPhaseList.addBack(func);
                return this;
            }

            protected override bool isPhaseEmpty()
            {
                return mPhaseList.count == 0;
            }

            protected override void executePhase()
            {
                mPhaseList.popFront().value(this);
            }

            protected override void onRecycle()
            {
                mPhaseList.clear();
                //mPhaseList = null;
                mUserData = default;
            }
        }
    }
}