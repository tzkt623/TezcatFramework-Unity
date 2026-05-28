using System;
using System.Collections.Generic;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 
    /// </summary>
    public class TezStepSystem
    {
        public enum StepState
        {
            Run,
            Resume,
            Wait,
            Complete,
            Abort,
            Jump,
            Next,
            Suspend
        }

        public interface IStepController
        {
            void wait(IStepController stepController);
            void suspend();
            void complete();
            void resume();
            void abort();
            void jump();
        }

        public interface IStepExecutor : IStepController
        {
            StepState stepState { get; }
            int waitIndex { get; set; }
            void execute();
            void recycle();
        }

        public class Group : IStepExecutor
        {
            Queue<Action<IStepExecutor>> mSteps = new Queue<Action<IStepExecutor>>();
            StepState mStepState = StepState.Run;
            public StepState stepState => mStepState;

            int IStepExecutor.waitIndex { get; set; }

            public void createStep(Action<IStepExecutor> action)
            {
                mSteps.Enqueue(action);
            }

            void IStepExecutor.execute()
            {
                if (mSteps.Count > 0)
                {
                    switch (mStepState)
                    {
                        //执行当前步骤,只执行一帧
                        case StepState.Run:
                            mSteps.Dequeue().Invoke(this);
                            break;
                        //中断,清除所有步骤
                        case StepState.Abort:
                            mSteps.Clear();
                            break;
                        //跳过,跳过下一个步骤
                        case StepState.Jump:
                            mSteps.Dequeue();
                            break;
                    }
                }
                else
                {
                    mStepState = StepState.Complete;
                }
            }

            void IStepController.suspend()
            {
                mStepState = StepState.Suspend;
            }

            void IStepController.complete()
            {
                mStepState = StepState.Complete;
            }

            void IStepController.resume()
            {
                mStepState = StepState.Run;
                TezStepSystem.manager.resume(this);
            }

            void IStepController.abort()
            {
                //mStepState = StepState.Abort;
                mSteps.Clear();
                mStepState = StepState.Complete;
            }

            void IStepController.jump()
            {
                //mStepState = StepState.Jump;
                if (mSteps.Count > 0)
                {
                    mSteps.Dequeue();
                }
                else
                {
                    mStepState = StepState.Complete;
                }
            }

            void IStepExecutor.recycle()
            {
                mSteps.Clear();
                mStepState = StepState.Run;
                ((IStepExecutor)this).waitIndex = -1;
            }

            void IStepController.wait(IStepController stepController)
            {
                mStepState = StepState.Wait;
                manager.wait(this).push((IStepExecutor)stepController);
            }
        }

        /*
        public class Step : IStepExecutor
        {
            static Queue<Step> mPool = new Queue<Step>();
            public static Step create(Action<IStepController> action)
            {
                if (mPool.Count > 0)
                {
                    var step = mPool.Dequeue();
                    step.mActon = action;
                    return step;
                }

                return new Step(action);
            }


            StepState mStepState = StepState.Run;
            public StepState stepState => mStepState;
            Action<Step> mActon = null;

            int IStepExecutor.waitIndex { get; set; } = 0;

            public Step(Action<IStepController> action)
            {
                mActon = action;
                mStepState = StepState.Run;
            }

            void IStepExecutor.execute()
            {
                mActon(this);
            }

            void IStepController.suspend()
            {
                mStepState = StepState.Suspend;
            }

            void IStepController.resume()
            {
                switch (mStepState)
                {
                    case StepState.Wait:
                        manager.resume(this);
                        break;
                    case StepState.Suspend:
                        manager.run(this);
                        break;
                    default:
                        break;
                }
            }

            void IStepController.complete()
            {
                mStepState = StepState.Complete;
            }

            void IStepController.abort()
            {
                mStepState = StepState.Abort;
            }

            void IStepController.jump()
            {
                mStepState = StepState.Jump;
            }

            void IStepExecutor.recycle()
            {
                mActon = null;
                mStepState = StepState.Run;
                ((IStepExecutor)this).waitIndex = -1;
            }

            void IStepController.wait(IStepController stepController)
            {

            }
        }
        */

        public class Sequence : IStepExecutor
        {
            StepState mStepState = StepState.Run;
            StepState IStepExecutor.stepState => mStepState;
            int IStepExecutor.waitIndex { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            Queue<IStepExecutor> mQueue = new Queue<IStepExecutor>();

            void IStepController.abort()
            {
                throw new NotImplementedException();
            }

            void IStepController.complete()
            {
                throw new NotImplementedException();
            }

            void IStepExecutor.execute()
            {
                if (mQueue.Count > 0)
                {
                    var step = mQueue.Peek();
                    switch (step.stepState)
                    {
                        case StepState.Run:
                            step.execute();
                            break;
                        case StepState.Wait:
                            break;
                        case StepState.Complete:
                            mQueue.Dequeue();
                            step.recycle();
                            break;
                        case StepState.Jump:
                            mQueue.Dequeue();
                            step.recycle();
                            break;
                        case StepState.Abort:
                            mQueue.Dequeue();
                            step.recycle();
                            break;
                    }
                }
                else
                {
                    mStepState = StepState.Complete;
                }
            }

            void IStepController.jump()
            {
                throw new NotImplementedException();
            }

            void IStepExecutor.recycle()
            {
                throw new NotImplementedException();
            }

            void IStepController.resume()
            {
                switch (mStepState)
                {
                    case StepState.Wait:
                        manager.resume(this);
                        break;
                    case StepState.Suspend:
                        manager.run(this);
                        break;
                    default:
                        break;
                }
            }

            void IStepController.suspend()
            {
                throw new NotImplementedException();
            }

            void IStepController.wait(IStepController stepController)
            {
                mQueue.Enqueue((IStepExecutor)stepController);
            }
        }


        public class LinkSequence : IStepExecutor
        {
            IStepExecutor mMaster = null;
            Queue<IStepExecutor> mSteps = new Queue<IStepExecutor>();

            StepState mStepState = StepState.Run;
            StepState IStepExecutor.stepState => mStepState;
            int IStepExecutor.waitIndex { get; set; } = -1;

            public void setParent(IStepExecutor master)
            {
                mMaster = master;
            }

            public void push(IStepExecutor step)
            {
                mSteps.Enqueue(step);
            }

            void IStepExecutor.execute()
            {
                if (mSteps.Count > 0)
                {
                    var step = mSteps.Peek();
                    switch (step.stepState)
                    {
                        case StepState.Run:
                            step.execute();
                            break;
                        case StepState.Wait:
                            break;
                        case StepState.Complete:
                            mSteps.Dequeue();
                            step.recycle();
                            break;
                        case StepState.Jump:
                            mSteps.Dequeue();
                            step.recycle();
                            break;
                        case StepState.Abort:
                            mSteps.Dequeue();
                            step.recycle();
                            break;
                    }
                }
                else
                {
                    mStepState = StepState.Complete;
                    mMaster?.resume();
                }
            }

            internal void reset()
            {
                mMaster = null;
                mSteps.Clear();

                mStepState = StepState.Run;
                ((IStepExecutor)this).waitIndex = -1;
            }

            void IStepExecutor.recycle()
            {
                this.reset();
            }

            void IStepController.wait(IStepController stepController)
            {
                mStepState = StepState.Wait;
                manager.wait(this).push((IStepExecutor)stepController);
            }

            void IStepController.suspend()
            {
                mStepState = StepState.Suspend;
            }

            void IStepController.complete()
            {
                mStepState = StepState.Complete;
            }

            void IStepController.resume()
            {
                switch (mStepState)
                {
                    case StepState.Wait:
                        manager.resume(this);
                        break;
                    case StepState.Suspend:
                        manager.run(this);
                        break;
                    default:
                        break;
                }

                mStepState = StepState.Run;
            }

            void IStepController.abort()
            {
                throw new NotImplementedException();
            }

            void IStepController.jump()
            {
                throw new NotImplementedException();
            }
        }

        class PoolLinkSequence
        {
            List<LinkSequence> mLinkSequenceRegistry = new List<LinkSequence>();
            Queue<int> mFreeRegistry = new Queue<int>();

            public (LinkSequence, int) create(IStepExecutor master)
            {
                if (mFreeRegistry.Count > 0)
                {
                    var index = mFreeRegistry.Dequeue();
                    var temp = mLinkSequenceRegistry[index];
                    temp.reset();
                    temp.setParent(master);
                    return (temp, index);
                }
                else
                {
                    var temp = new LinkSequence();
                    temp.setParent(master);
                    mLinkSequenceRegistry.Add(temp);
                    return (temp, mLinkSequenceRegistry.Count - 1);
                }
            }

            public LinkSequence get(int index)
            {
                return mLinkSequenceRegistry[index];
            }

            public void recycle(int index)
            {
                mLinkSequenceRegistry[index].reset();
                mFreeRegistry.Enqueue(index);
            }
        }

        class PoolSuspend
        {
            List<IStepExecutor> mLinkSequenceRegistry = new List<IStepExecutor>();
            Queue<int> mFreeRegistry = new Queue<int>();

            public void push(IStepExecutor master)
            {
                if (mFreeRegistry.Count > 0)
                {
                    var index = mFreeRegistry.Dequeue();
                    mLinkSequenceRegistry[index] = master;
                }
                else
                {
                    mLinkSequenceRegistry.Add(master);
                }
            }
        }

        public class Manager
        {
            PoolLinkSequence mPoolLinkSequence = new PoolLinkSequence();
            List<IStepExecutor> mExecutors = new List<IStepExecutor>();
            int mExecutorSize = 0;

            Queue<IStepExecutor> mNewAddCache = new Queue<IStepExecutor>();

            public void run(IStepExecutor executor)
            {
                mNewAddCache.Enqueue(executor);
            }

            private void markForRemove(int removingIndex)
            {
                if (removingIndex != mExecutorSize - 1)
                {
                    var removing = mExecutors[removingIndex];
                    var last = mExecutors[mExecutorSize - 1];

                    mExecutors[removingIndex] = last;
                    mExecutors[mExecutorSize - 1] = removing;
                }
                mExecutorSize--;
            }

            public void update()
            {
                while(mNewAddCache.Count > 0)
                {
                    this.run(mNewAddCache.Dequeue());
                    mExecutorSize++;
                }

                for (int i = 0; i < mExecutors.Count; i++)
                {
                    var exe = mExecutors[i];
                    switch (exe.stepState)
                    {
                        case StepState.Run:
                            exe.execute();
                            break;
                        case StepState.Wait:
                            this.markForRemove(i);
                            break;
                        case StepState.Suspend:
                            this.markForRemove(i);
                            break;
                        case StepState.Complete:
                            this.markForRemove(i);
                            exe.recycle();
                            break;
                        default:
                            break;
                    }
                }

                if (mExecutors.Count > mExecutorSize)
                {
                    mExecutors.RemoveRange(mExecutorSize, mExecutors.Count - mExecutorSize);
                    mExecutorSize = mExecutors.Count;
                }
            }

            public Group createGroup()
            {
                var group = new Group();
                return group;
            }

            internal LinkSequence wait(IStepExecutor master)
            {
                if (master.waitIndex < 0)
                {
                    (var temp, var index) = mPoolLinkSequence.create(master);
                    master.waitIndex = index;
                    mNewAddCache.Enqueue(temp);
                    return temp;
                }

                return mPoolLinkSequence.get(master.waitIndex);
            }

            internal void resume(IStepExecutor master)
            {
                if (master.waitIndex < 0)
                {
                    throw new Exception("step is not in wait state");
                }

                mPoolLinkSequence.recycle(master.waitIndex);
                master.waitIndex = -1;
                mNewAddCache.Enqueue(master);
            }
        }

        public static readonly Manager manager = new Manager();

        public void test()
        {
            var group = TezStepSystem.manager.createGroup();

            group.createStep((controller) =>
            {
                controller.suspend();
            });

            group.createStep((controller) =>
            {
                controller.suspend();
            });

            group.createStep((controller) =>
            {
                controller.wait(this.test2());
            });


            TezStepSystem.manager.run(group);
        }

        public IStepController test2()
        {
            var group = TezStepSystem.manager.createGroup();

            group.createStep((controller) =>
            {
                controller.suspend();
            });

            group.createStep((controller) =>
            {
                controller.suspend();
            });

            group.createStep((controller) =>
            {
                controller.suspend();
            });

            return group;
        }
    }
}