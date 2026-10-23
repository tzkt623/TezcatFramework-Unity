using System;
using System.Collections.Generic;

namespace tezcat.Framework.Game
{
    public enum TezStepTiming2
    {
        Run,
        Wait,
        Complete,
        Abort,
        Children,
        Parent
    }

    public enum TezStepWishedTiming
    {
        Run,
        Wait,
        Parent,
        Abort
    }

    public enum TezStepExecutorTiming
    {
        Run,
        Wait,
        Complete,
    }

    public interface ITezBaseStep
    {
        TezStepTiming2 timing { get; }

        void onExecute(TezStepSystem2 system);

        void onChildren(TezStepSystem2 system);
        void onParent(TezStepSystem2 system);

        void onWait(TezStepSystem2 system);
        void onComplete(TezStepSystem2 system);
        void onAbort(TezStepSystem2 system);

        void close();
    }

    /// <summary>
    /// 步骤组
    /// 可顺序执行多个步骤
    /// 并且可以嵌套执行子步骤组
    /// </summary>
    public class TezStepGroup : ITezBaseStep
    {
        public event Action evtAbort;
        public ITezStepExecutor parentExecutor { get; set; } = null;
        public ITezStepExecutor childExecutor { get; set; } = null;

        private int mIndex = 0;
        private List<Action<TezStepGroup>> mSteps = new List<Action<TezStepGroup>>();
        public TezStepTiming2 timing => mTiming;
        private TezStepTiming2 mTiming = TezStepTiming2.Complete;

        public void onInit(ITezStepExecutor parentExecutor)
        {
            this.parentExecutor = parentExecutor;
            mIndex = 0;
            if (mSteps.Count > 0)
            {
                mTiming = TezStepTiming2.Run;
            }
            else
            {
                mTiming = TezStepTiming2.Complete;
            }
        }

        public void onExecute(TezStepSystem2 system)
        {
            if (mIndex >= mSteps.Count)
            {
                mTiming = TezStepTiming2.Complete;
                return;
            }

            mSteps[mIndex++](this);

            if (mIndex >= mSteps.Count && mTiming == TezStepTiming2.Run)
            {
                mTiming = TezStepTiming2.Complete;
            }
        }

        /// <summary>
        /// 立即执行当前Group的执行器
        /// </summary>
        public void onParent(TezStepSystem2 system)
        {
            system.runImmediately(this.parentExecutor);
        }

        //
        public void onChildren(TezStepSystem2 system)
        {
            //把子步骤执行器加入到系统中,并且切换状态为运行子步骤
            system.runImmediately(this.childExecutor);
            mTiming = TezStepTiming2.Run;
        }

        public void onWait(TezStepSystem2 system)
        {
            ///啥也不执行,就是单纯等待指令
        }

        public void addStep(Action<TezStepGroup> step)
        {
            mSteps.Add(step);
        }

        /// <summary>
        /// 等待子步骤
        /// 子步骤会打包到一个新的执行器中
        /// </summary>
        public void wait<T>(TezStepGroup step) where T : ITezStepExecutor, new()
        {
            if (this.childExecutor == null)
            {
                this.childExecutor = new T();
                this.childExecutor.parentStep = this;
            }

            //将状态切换到等待子步骤状态
            mTiming = TezStepTiming2.Children;
            this.childExecutor.add(step);
        }

        public void onComplete(TezStepSystem2 system)
        {
            this.clear();
        }

        public void onAbort(TezStepSystem2 system)
        {
            evtAbort?.Invoke();
            this.clear();
        }

        public void setWishedTiming(TezStepWishedTiming wishedTiming)
        {
            switch(mTiming)
            {
                case TezStepTiming2.Abort:
                    break;
            }

            switch (wishedTiming)
            {
                //中止当前步骤组的执行
                case TezStepWishedTiming.Abort:
                    mTiming = TezStepTiming2.Abort;
                    break;
                //期望继续执行当前步骤组
                case TezStepWishedTiming.Run:
                    if (mTiming != TezStepTiming2.Abort)
                    {
                        mTiming = mIndex >= mSteps.Count ? TezStepTiming2.Complete : TezStepTiming2.Run;
                    }
                    break;
                //期望当前步骤进行等待
                case TezStepWishedTiming.Wait:
                    //必须要运行状态下才能进行等待
                    if (mTiming == TezStepTiming2.Run)
                    {
                        mTiming = TezStepTiming2.Wait;
                    }
                    break;
                //期望执行父步骤组,也就是中断当前步骤组的执行,并且切换到父步骤组的执行器中
                case TezStepWishedTiming.Parent:
                    mTiming = TezStepTiming2.Parent;
                    break;
                default:
                    throw new Exception();
            }
        }

        private void clear()
        {
            this.childExecutor?.close();

            this.evtAbort = null;
            this.parentExecutor = null;
            this.childExecutor = null;
            mIndex = 0;
            mSteps.Clear();
            mSteps = null;

            mTiming = TezStepTiming2.Complete;
        }

        public void close()
        {
            this.clear();
        }
    }

    /// <summary>
    /// 执行器
    /// 系统层最基础的运行单元
    /// StepGroup必须在Executor中运行
    /// </summary>
    public interface ITezStepExecutor
    {
        TezStepGroup parentStep { get; set; }
        void add(TezStepGroup step);
        void onAddIntoSystem(TezStepSystem2 system);
        TezStepExecutorTiming update(TezStepSystem2 system);
        void complete(TezStepSystem2 system);
        void close();
    }

    /// <summary>
    /// 堆栈式执行器
    /// </summary>
    public class TezStepStackExecutor : ITezStepExecutor
    {
        /// <summary>
        /// 那个父步骤持有这个执行器
        /// </summary>
        public TezStepGroup parentStep { get; set; } = null;
        List<TezStepGroup> mStackGroup = new List<TezStepGroup>();
        int mRunIndex = 0;
        TezStepExecutorTiming mTiming = TezStepExecutorTiming.Complete;

        void ITezStepExecutor.onAddIntoSystem(TezStepSystem2 system)
        {
            mTiming = TezStepExecutorTiming.Run;
            mRunIndex = mStackGroup.Count - 1;
        }

        void ITezStepExecutor.add(TezStepGroup step)
        {
            step.onInit(this);
            mStackGroup.Add(step);
            mRunIndex = mStackGroup.Count - 1;
        }

        void ITezStepExecutor.close()
        {
            if (this.parentStep != null)
            {
                return;
            }

            if (mStackGroup.Count > 0)
            {
                foreach (var item in mStackGroup)
                {
                    item.close();
                }
            }
            mStackGroup.Clear();
            mStackGroup = null;
        }

        void ITezStepExecutor.complete(TezStepSystem2 system)
        {
            mRunIndex = 0;

            if (mStackGroup.Count > 0)
            {
                foreach (var item in mStackGroup)
                {
                    item.close();
                }
            }
            mStackGroup.Clear();

            //如果父步骤不为空
            //那么此Executor必然为wait生成的
            //所以他的生命周期归父Step管
            if (this.parentStep != null)
            {
                this.parentStep.onParent(system);
            }
        }

        TezStepExecutorTiming ITezStepExecutor.update(TezStepSystem2 system)
        {
            if (mRunIndex < 0)
            {
                return TezStepExecutorTiming.Complete;
            }

            var group = mStackGroup[mRunIndex];
            switch (group.timing)
            {
                case TezStepTiming2.Run:
                    group.onExecute(system);
                    break;
                case TezStepTiming2.Complete:
                    mStackGroup.RemoveAt(mRunIndex);
                    mRunIndex = mStackGroup.Count - 1;
                    group.onComplete(system);
                    if (mRunIndex < 0)
                    {
                        mTiming = TezStepExecutorTiming.Complete;
                    }
                    break;
                //group主动中断自己的执行,但是不会中断执行器的执行
                //如果可能,执行器会继续执行下一个步骤
                case TezStepTiming2.Abort:
                    mStackGroup.RemoveAt(mRunIndex);
                    mRunIndex = mStackGroup.Count - 1;
                    group.onAbort(system);
                    if (mRunIndex < 0)
                    {
                        mTiming = TezStepExecutorTiming.Complete;
                    }
                    break;
                //让当前步骤组进入等待状态,但是不会中断执行器的执行
                case TezStepTiming2.Wait:
                    group.onWait(system);
                    break;
                case TezStepTiming2.Children:
                    group.onChildren(system);
                    mTiming = TezStepExecutorTiming.Wait;
                    break;
                //group主动呼叫执行器执行其父步骤
                //也就是中断当前执行器,不管里面是否还有步骤
                case TezStepTiming2.Parent:
                    //group.onParent(system);
                    //system.runInNextFrame(this.parentStep.parentExecutor);
                    mTiming = TezStepExecutorTiming.Complete;
                    break;
                default:
                    break;
            }

            return mTiming;
        }
    }



    public class TezSequenceStepExecutor : ITezStepExecutor
    {
        public TezStepGroup parentStep { get; set; }
        Queue<TezStepGroup> mQueueGroup = new Queue<TezStepGroup>();
        TezStepExecutorTiming mTiming = TezStepExecutorTiming.Complete;

        void ITezStepExecutor.add(TezStepGroup step)
        {
            step.onInit(this);
            mQueueGroup.Enqueue(step);
        }

        void ITezStepExecutor.onAddIntoSystem(TezStepSystem2 system)
        {
            mTiming = TezStepExecutorTiming.Run;
        }

        TezStepExecutorTiming ITezStepExecutor.update(TezStepSystem2 system)
        {
            if (mQueueGroup.Count == 0)
            {
                return TezStepExecutorTiming.Complete;
            }

            var group = mQueueGroup.Peek();
            switch (group.timing)
            {
                case TezStepTiming2.Run:
                    group.onExecute(system);
                    break;
                case TezStepTiming2.Complete:
                    mQueueGroup.Dequeue();
                    group.onComplete(system);
                    if (mQueueGroup.Count == 0)
                    {
                        mTiming = TezStepExecutorTiming.Complete;
                    }
                    break;
                case TezStepTiming2.Abort:
                    //
                    mQueueGroup.Dequeue();
                    group.onAbort(system);
                    if (mQueueGroup.Count == 0)
                    {
                        mTiming = TezStepExecutorTiming.Complete;
                    }
                    break;
                case TezStepTiming2.Wait:
                    group.onWait(system);
                    break;
                case TezStepTiming2.Children:
                    group.onChildren(system);
                    mTiming = TezStepExecutorTiming.Wait;
                    break;
                //group主动呼叫执行器执行其父步骤
                //也就是中断当前执行器,不管里面是否还有步骤
                case TezStepTiming2.Parent:
                    mTiming = TezStepExecutorTiming.Complete;
                    break;
                default:
                    break;
            }

            return mTiming;
        }

        void ITezStepExecutor.complete(TezStepSystem2 system)
        {
            if (mQueueGroup.Count > 0)
            {
                foreach (var item in mQueueGroup)
                {
                    item.close();
                }
            }
            mQueueGroup.Clear();

            if (this.parentStep != null)
            {
                this.parentStep.onParent(system);
            }
        }

        void ITezStepExecutor.close()
        {
            if (this.parentStep != null)
            {
                return;
            }

            if (mQueueGroup.Count > 0)
            {
                foreach (var item in mQueueGroup)
                {
                    item.close();
                }
            }
            mQueueGroup.Clear();
            mQueueGroup = null;
        }
    }



    public class TezStepSystem2
    {
        const int RUN_A = 0;
        const int RUN_B = 1;

        public static TezStepSystem2 instance { get; } = new TezStepSystem2();

        int mRunIndex = 0;
        List<ITezStepExecutor> mRunA = new List<ITezStepExecutor>();
        List<ITezStepExecutor> mRunB = new List<ITezStepExecutor>();
        List<ITezStepExecutor> mCache = new List<ITezStepExecutor>();
        List<ITezStepExecutor> mRunCurrent = null;
        List<ITezStepExecutor> mRunNext = null;

        public TezStepSystem2()
        {
            mRunCurrent = mRunA;
            mRunNext = mRunB;
            mRunIndex = RUN_A;
        }

        public bool isEmpty()
        {
            return (mRunCurrent.Count + mRunNext.Count + mCache.Count) == 0;
        }

        //         public void runInNextFrame(ITezStackStepContainer2 container)
        //         {
        //             mRunNext.Add(container);
        //         }

        public void runInNextFrame<T>(TezStepGroup step) where T : ITezStepExecutor, new()
        {
            var executor = new T();
            executor.onAddIntoSystem(this);
            executor.add(step);
            mCache.Add(executor);
        }

        public void runImmediately<T>(TezStepGroup step) where T : ITezStepExecutor, new()
        {
            var executor = new T();
            executor.onAddIntoSystem(this);
            executor.add(step);
            mRunCurrent.Add(executor);
        }

        public void runInNextFrame(ITezStepExecutor executor)
        {
            executor.onAddIntoSystem(this);
            mCache.Add(executor);
        }

        public void runImmediately(ITezStepExecutor executor)
        {
            executor.onAddIntoSystem(this);
            mRunCurrent.Add(executor);
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
                switch (container.update(this))
                {
                    case TezStepExecutorTiming.Run:
                        mRunNext.Add(container);
                        break;
                    case TezStepExecutorTiming.Wait:
                        Console.WriteLine("Container Wait Removed");
                        break;
                    case TezStepExecutorTiming.Complete:
                        container.complete(this);
                        container.close();
                        Console.WriteLine("Container Complete Removed");
                        break;
                    default:
                        break;
                }
            }

            mRunCurrent.Clear();
        }
    }

}

