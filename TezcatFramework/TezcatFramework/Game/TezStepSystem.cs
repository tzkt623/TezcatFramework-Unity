using System;
using System.Collections.Generic;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 
    /// 子节点完成时把父节点又加入进循环导致bug
    /// 
    /// 
    /// 如果子节点挂起,那就挂起他的父节点
    /// 
    /// 改成childreport
    /// 
    /// </summary>
    public class TezStepSystem
    {
        public enum StepState
        {
            Run,
            Wait,
            Pause,
            Complete,
            Abort,
            Jump,
        }

        public interface IStepData
        {
            void close();
        }

        public interface IStep
        {
            IStep parent { get; }
            IStepData userData { get; set; }

            /// <summary>
            /// 等待另一个Step执行完成
            /// </summary>
            /// <param name="step"></param>
            void wait(IStep step);

            /// <summary>
            /// 暂停当前Group
            /// </summary>
            void pause();

            /// <summary>
            /// 从暂停中恢复
            /// </summary>
            void resume();

            /// <summary>
            /// 中断当前Group
            /// </summary>
            void abort();

            /// <summary>
            /// 跳过当前Group中的下一个步骤
            /// </summary>
            void jump();
        }

        public interface IStepExecutor : IStep
        {
            IStepExecutor parent { get; set; }
            StepState stepState { get; }
            void childReportComplete(IStepExecutor child);
            void recycle();

            void onJump();
            void onRun();
            void onAbort();
            void onComplete();
            void onWait();
        }


        /// <summary>
        /// Grop是一个单元
        /// 通过内部的Step执行逻辑
        /// 每一个step只执行一帧,每一帧结束后,根据step的状态决定下一步的执行逻辑
        /// 
        /// 在step内部调用wait pause resume abort jump来达到不同的功能效果
        /// 
        /// </summary>
        public class StepGroup : IStepExecutor
        {
            public string name { get; set; }
            string oldName = null;

            Queue<Action<IStep>> mSteps = new Queue<Action<IStep>>();
            StepState mStepState = StepState.Run;
            Sequence mTriggerGroup = null;

            public StepState stepState
            {
                get
                {
                    //Console.WriteLine($"Group[{name}|{oldName}]: StepState {mStepState}");
                    return mStepState;
                }
            }

            IStepExecutor mParent = null;
            IStepExecutor IStepExecutor.parent
            {
                get { return mParent; }
                set { mParent = value; }
            }

            IStep IStep.parent
            {
                get { return mParent; }
            }

            IStepData mUserData = null;
            IStepData IStep.userData
            {
                get { return mUserData; }
                set { mUserData  = value; }
            }

            /// <summary>
            /// 添加步骤
            /// </summary>
            public void addStep(Action<IStep> action)
            {
                mSteps.Enqueue(action);
            }

            private void reset()
            {
                //Console.WriteLine($"Reset Group: {this.name}|{oldName}");
                oldName = this.name;
                mUserData?.close();
                mSteps.Clear();
                mStepState = StepState.Run;

                mParent = null;
                this.name = null;
                mUserData = null;

                if (mTriggerGroup != null)
                {
                    mPoolLinkSequence.recycle(mTriggerGroup);
                    mTriggerGroup = null;
                }

                mPoolGroup.recycle(this);
            }

            void IStepExecutor.recycle()
            {
                this.reset();
            }

            void IStepExecutor.onRun()
            {
                //Console.WriteLine($"Group[{name}|{oldName}]: Exe {mStepState}");
                //                 if (mTriggerGroup != null)
                //                 {
                //                     if (mTriggerGroup.runImmediately())
                //                     {
                //                         mStepState = StepState.Wait;
                //                         return;
                //                     }
                // 
                //                     mPoolLinkSequence.recycle(mTriggerGroup);
                //                     mTriggerGroup = null;
                //                 }

                if (mSteps.Count == 0)
                {
                    mStepState = StepState.Complete;
                    return;
                }

                mSteps.Dequeue().Invoke(this);
            }

            void IStepExecutor.onWait()
            {
                if (mTriggerGroup == null)
                {
                    throw new InvalidOperationException();
                }

                if (!mTriggerGroup.runImmediately())
                {
                    if(mSteps.Count == 0)
                    {
                        mStepState = StepState.Complete;
                    }
                    else
                    {
                        mStepState = StepState.Run;
                    }
                    manager.runNextFrame(this);
                }

//                 mPoolLinkSequence.recycle(mTriggerGroup);
//                 mTriggerGroup = null;
            }

            void IStepExecutor.onJump()
            {
                mSteps.Dequeue();
                if (mSteps.Count == 0)
                {
                    mStepState = StepState.Complete;
                }
                else
                {
                    mStepState = StepState.Run;
                }
            }

            void IStepExecutor.onAbort()
            {
                if (mParent != null)
                {
                    mParent.childReportComplete(this);
                }

                this.reset();
            }

            void IStepExecutor.onComplete()
            {
                if (mParent != null)
                {
                    mParent.childReportComplete(this);
                }

                this.reset();
            }

            void IStep.pause()
            {
                mStepState = StepState.Pause;
            }

            void IStep.resume()
            {
                if(mStepState != StepState.Pause)
                {
                    throw new Exception();
                }

                mStepState = StepState.Run;
                manager.runNextFrame(this);
            }

            void IStep.abort()
            {
                mStepState = StepState.Abort;
            }

            void IStep.jump()
            {
                if (mSteps.Count == 0)
                {
                    mStepState = StepState.Complete;
                }
                else
                {
                    mStepState = StepState.Jump;
                }
            }

            void IStep.wait(IStep stepController)
            {
                mStepState = StepState.Wait;
                if (mTriggerGroup == null)
                {
                    mTriggerGroup = mPoolLinkSequence.create();
                }

                ((IStepExecutor)stepController).parent = this;
                mTriggerGroup.add((IStepExecutor)stepController);
            }

            void IStepExecutor.childReportComplete(IStepExecutor child)
            {
                if (mStepState == StepState.Abort || mStepState == StepState.Jump)
                {
                    manager.runNextFrame(this);
                    return;
                }

                if (mTriggerGroup == null)
                {
                    mStepState = StepState.Run;
                    manager.runNextFrame(this);
                    return;
                }

                if (!mTriggerGroup.runNextFrame(child))
                {
                    mPoolLinkSequence.recycle(mTriggerGroup);
                    mTriggerGroup = null;

                    mStepState = StepState.Run;
                    manager.runNextFrame(this);
                }
            }
        }

        internal class Sequence
        {
            Queue<IStepExecutor> mSteps = new Queue<IStepExecutor>();

            internal void add(IStepExecutor step)
            {
                mSteps.Enqueue(step);
            }

            internal bool runImmediately()
            {
                if (mSteps.Count > 0)
                {
                    manager.runImmediately(mSteps.Dequeue());
                    return true;
                }

                return false;
            }

            internal bool runNextFrame(IStepExecutor step)
            {
                if (mSteps.Count > 0)
                {
                    manager.runNextFrame(mSteps.Dequeue());
                    return true;
                }

                return false;
            }

            internal void reset()
            {
                foreach (var item in mSteps)
                {
                    item.recycle();
                }
                mSteps.Clear();
            }
        }

        class PoolLinkSequence
        {
            Queue<Sequence> mFreeLinkSequence = new Queue<Sequence>();

            internal void clear()
            {
                foreach (var item in mFreeLinkSequence)
                {
                    item.reset();
                }
            }

            internal Sequence create()
            {
                if (mFreeLinkSequence.Count > 0)
                {
                    var temp = mFreeLinkSequence.Dequeue();
                    return temp;
                }
                else
                {
                    var temp = new Sequence();
                    return temp;
                }
            }

            internal void recycle(Sequence linkSequence)
            {
                linkSequence.reset();
                mFreeLinkSequence.Enqueue(linkSequence);
            }
        }

        class PoolGroup
        {
            Queue<StepGroup> mFreeRegistry = new Queue<StepGroup>();

            internal StepGroup create()
            {
                if (mFreeRegistry.Count > 0)
                {
                    return mFreeRegistry.Dequeue();
                }
                else
                {
                    return new StepGroup();
                }
            }

            internal StepGroup create(IStepData userData)
            {
                if (mFreeRegistry.Count > 0)
                {
                    var g = mFreeRegistry.Dequeue();
                    ((IStep)g).userData = userData;
                    return g;
                }
                else
                {
                    var g = new StepGroup();
                    ((IStep)g).userData = userData;
                    return g;
                }
            }

            internal StepGroup create(string name)
            {
                if (mFreeRegistry.Count > 0)
                {
                    var temp = mFreeRegistry.Dequeue();
                    temp.name = name;
                    return temp;
                }
                else
                {
                    return new StepGroup()
                    {
                        name = name
                    };
                }
            }

            internal void recycle(StepGroup group)
            {
                mFreeRegistry.Enqueue(group);
            }
        }

        public class Manager
        {
            enum SwapBuffer
            {
                Init, A, B
            }

            List<IStepExecutor> mExeBufferA = new List<IStepExecutor>();
            List<IStepExecutor> mExeBufferB = new List<IStepExecutor>();

            List<IStepExecutor> mCurrentExecutors = null;
            List<IStepExecutor> mNextExecutors = null;
            List<IStepExecutor> mNewAddCache = new List<IStepExecutor>();

            SwapBuffer mSwitchExecutor = SwapBuffer.Init;


            internal Manager()
            {
                mCurrentExecutors = mExeBufferA;
                mNextExecutors = mExeBufferB;
                mSwitchExecutor = SwapBuffer.Init;
            }

            public void run(IStepExecutor executor)
            {
                mNewAddCache.Add(executor);
            }

            internal void runNextFrame(IStepExecutor executor)
            {
                mNewAddCache.Add(executor);
            }

            internal void runImmediately(IStepExecutor executor)
            {
                mCurrentExecutors.Add(executor);
            }

            public void clear()
            {
                foreach (var item in mExeBufferA)
                {
                    item.recycle();
                }

                foreach (var item in mExeBufferB)
                {
                    item.recycle();
                }

                foreach (var item in mNewAddCache)
                {
                    item.recycle();
                }

                mNewAddCache.Clear();
                mExeBufferA.Clear();
                mExeBufferB.Clear();
            }

            public void update()
            {
                switch (mSwitchExecutor)
                {
                    case SwapBuffer.Init:
                        mSwitchExecutor = SwapBuffer.B;
                        break;
                    case SwapBuffer.A:
                        mCurrentExecutors = mExeBufferA;
                        mNextExecutors = mExeBufferB;
                        mSwitchExecutor = SwapBuffer.B;
                        break;
                    case SwapBuffer.B:
                        mCurrentExecutors = mExeBufferB;
                        mNextExecutors = mExeBufferA;
                        mSwitchExecutor = SwapBuffer.A;
                        break;
                    default:
                        break;
                }

                if (mNewAddCache.Count > 0)
                {
                    mCurrentExecutors.AddRange(mNewAddCache);
                    mNewAddCache.Clear();
                }

                for (int i = 0; i < mCurrentExecutors.Count; i++)
                {
                    //先验证一下运行状态
                    var exe = mCurrentExecutors[i];
                    switch (exe.stepState)
                    {
                        //如果是运行状态,添加到下一轮继续执行
                        case StepState.Run:
                            exe.onRun();
                            switch (exe.stepState)
                            {
                                case StepState.Run:
                                case StepState.Wait:
                                case StepState.Jump:
                                case StepState.Abort:
                                    mNextExecutors.Add(exe);
                                    break;
                                case StepState.Complete:
                                    exe.onComplete();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case StepState.Wait:
                            exe.onWait();
                            break;
                        case StepState.Jump:
                            exe.onJump();
                            switch (exe.stepState)
                            {
                                case StepState.Run:
                                    mNextExecutors.Add(exe);
                                    break;
                                case StepState.Complete:
                                    exe.onComplete();
                                    break;
                                default:
                                    break;
                            }
                            break;
                        case StepState.Abort:
                            exe.onAbort();
                            break;
                        //如果已经完成,就执行清理
                        case StepState.Complete:
                            exe.onComplete();
                            break;
                        case StepState.Pause:
                            break;
                        default:
                            break;
                    }
                }
                mCurrentExecutors.Clear();
            }

            public StepGroup createStepGroup(string name)
            {
                return mPoolGroup.create(name);
            }

            public StepGroup createStepGroup()
            {
                return mPoolGroup.create();
            }

            public StepGroup createStepGroup(IStepData userData)
            {
                return mPoolGroup.create(userData);
            }
        }

        private static PoolLinkSequence mPoolLinkSequence = new PoolLinkSequence();
        private static PoolGroup mPoolGroup = new PoolGroup();
        public static readonly Manager manager = new Manager();
    }
}