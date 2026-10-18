using System;
using System.Collections.Generic;

namespace tezcat.Framework.Game
{
    public enum TezStackStepTiming
    {
        Run,
        Wait,
        Complete,
        Abort,
    }

    public enum TezStackStepWishedTime
    {
        Run,
        Wait,
        Abort,
    }

    public class TezStackStep
    {
        public event Action evtAbort;
        public ITezStackStepContainer stepContainer { get; private set; } = null;

        private int mIndex = 0;
        private List<Action<TezStackStep>> mSteps = new List<Action<TezStackStep>>();
        public TezStackStepTiming timing => mTiming;
        private TezStackStepTiming mTiming = TezStackStepTiming.Complete;

        public void onInit(ITezStackStepContainer stepContainer)
        {
            this.stepContainer = stepContainer;
            mIndex = 0;
            if (mSteps.Count > 0)
            {
                mTiming = TezStackStepTiming.Run;
            }
            else
            {
                mTiming = TezStackStepTiming.Complete;
            }
        }

        public void onExecute()
        {
            mSteps[mIndex++](this);

            if (mTiming == TezStackStepTiming.Run && mIndex >= mSteps.Count)
            {
                mTiming = TezStackStepTiming.Complete;
            }
        }

        public void onWait()
        {

        }

        public void addStep(Action<TezStackStep> step)
        {
            mSteps.Add(step);
        }

        public void wait(TezStackStep step)
        {
            this.stepContainer.run(step);
        }

        public void onComplete()
        {
            this.clear();
        }

        public void onAbort()
        {
            evtAbort?.Invoke();
            this.clear();
        }

        public void setWishedTiming(TezStackStepWishedTime wishedTiming)
        {
            switch (wishedTiming)
            {
                case TezStackStepWishedTime.Abort:
                    mTiming = TezStackStepTiming.Abort;
                    break;
                case TezStackStepWishedTime.Run:
                    if (mTiming != TezStackStepTiming.Abort)
                    {
                        if (mIndex >= mSteps.Count)
                        {
                            mTiming = TezStackStepTiming.Complete;
                        }
                        else
                        {
                            mTiming = TezStackStepTiming.Run;
                        }
                    }
                    break;
                case TezStackStepWishedTime.Wait:
                    if (mTiming == TezStackStepTiming.Run)
                    {
                        mTiming = TezStackStepTiming.Wait;
                    }
                    break;
                default:
                    throw new Exception();
            }
        }

        private void clear()
        {
            this.evtAbort = null;
            this.stepContainer = null;
            mIndex = 0;
            mSteps.Clear();
            mTiming = TezStackStepTiming.Complete;
        }
    }

    public interface ITezStackStepContainer
    {
        bool update();
        void run(TezStackStep step);
    }

    public class TezStackStepContainer : ITezStackStepContainer
    {
        Stack<TezStackStep> mStackChains = new Stack<TezStackStep>();

        public bool update()
        {
            if (mStackChains.Count > 0)
            {
                var chain = mStackChains.Peek();
                switch (chain.timing)
                {
                    case TezStackStepTiming.Run:
                        chain.onExecute();
                        break;
                    case TezStackStepTiming.Complete:
                        mStackChains.Pop();
                        chain.onComplete();
                        break;
                    case TezStackStepTiming.Abort:
                        mStackChains.Pop();
                        chain.onAbort();
                        break;
                    case TezStackStepTiming.Wait:
                        chain.onWait();
                        break;
                    default:
                        break;
                }

                return mStackChains.Count > 0;
            }

            return false;
        }

        public void run(TezStackStep step)
        {
            step.onInit(this);
            mStackChains.Push(step);
        }
    }

    public class TezSequenceStepContainer : ITezStackStepContainer
    {
        Queue<TezStackStep> mQueueChains = new Queue<TezStackStep>();

        public bool update()
        {
            if (mQueueChains.Count > 0)
            {
                var chain = mQueueChains.Peek();
                switch (chain.timing)
                {
                    case TezStackStepTiming.Run:
                        chain.onExecute();
                        break;
                    case TezStackStepTiming.Complete:
                        mQueueChains.Dequeue();
                        chain.onComplete();
                        break;
                    case TezStackStepTiming.Abort:
                        mQueueChains.Dequeue();
                        chain.onAbort();
                        break;
                    case TezStackStepTiming.Wait:
                        chain.onWait();
                        break;
                    default:
                        break;
                }
                return mQueueChains.Count > 0;
            }
            return false;
        }

        public void run(TezStackStep step)
        {
            step.onInit(this);
            mQueueChains.Enqueue(step);
        }
    }

    public class TezStackStepSystem
    {
        const int RUN_A = 0;
        const int RUN_B = 1;

        public static TezStackStepSystem instance { get; } = new TezStackStepSystem();

        int mRunIndex = 0;
        List<ITezStackStepContainer> mRunA = new List<ITezStackStepContainer>();
        List<ITezStackStepContainer> mRunB = new List<ITezStackStepContainer>();
        List<ITezStackStepContainer> mCache = new List<ITezStackStepContainer>();
        List<ITezStackStepContainer> mRunCurrent = null;
        List<ITezStackStepContainer> mRunNext = null;

        public TezStackStepSystem()
        {
            mRunCurrent = mRunA;
            mRunNext = mRunB;
            mRunIndex = RUN_A;
        }

        public bool isEmpty()
        {
            return (mRunCurrent.Count + mRunNext.Count + mCache.Count) == 0;
        }

        public void runInNextFrame<T>(TezStackStep step) where T : ITezStackStepContainer, new()
        {
            var container = new T();
            container.run(step);
            mCache.Add(container);
        }

        public void runImmediately<T>(TezStackStep step) where T : ITezStackStepContainer, new()
        {
            var container = new T();
            container.run(step);
            mRunCurrent.Add(container);
        }

        public void update()
        {
            switch (mRunIndex)
            {
                case RUN_A:
                    mRunCurrent = mRunA;
                    mRunNext = mRunB;
                    mRunIndex = RUN_B;
                    break;
                case RUN_B:
                    mRunCurrent = mRunB;
                    mRunNext = mRunA;
                    mRunIndex = RUN_A;
                    break;
                default:
                    break;
            }

            if (mCache.Count > 0)
            {
                mRunCurrent.AddRange(mCache);
                mCache.Clear();
            }

            for (int i = 0; i < mRunCurrent.Count; i++)
            {
                var container = mRunCurrent[i];
                if (container.update())
                {
                    mRunNext.Add(container);
                }
            }

            mRunCurrent.Clear();
        }
    }

}