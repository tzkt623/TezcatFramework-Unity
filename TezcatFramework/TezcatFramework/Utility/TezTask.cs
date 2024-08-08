using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 
    /// 任务系统
    /// 
    /// 基本逻辑
    /// 当前任务做完了,通知并开始下一项任务
    /// 
    /// 1.发布一个任务,当前任务状态变为等待,等待任务执行完成
    ///   比如发布一个执行动画的任务,动画就自己去执行了,执行完毕之后才会通知当前任务执行完毕
    /// 2.任务逻辑每一帧都会执行一次,直到执行完毕
    /// 3.任务执行器和等待器并不是同一个
    /// 
    /// C#自带的await会卡住当前函数位置,当当前await执行完毕之后
    /// 才继续后续逻辑,所以他不用手动保存一个回调器来通知完毕动作
    /// 
    /// </summary>
    public class TezTask
    {
        public delegate void DispathFuncT<T>(Task<T> task);
        static void defaultDelegate<T>(ref T userData) { }
        static void defaultDelegate() { }
        static void defaultDelegate(Task task) { }
        static void defaultDelegate<T>(Task<T> task) { }

        #region AsyncTask
        public enum AsyncState
        {
            Running,
            Complete
        }

        public abstract class AsyncBaseTask : ITezCloseable
        {
            protected AsyncState mState = AsyncState.Running;
            public AsyncState state => mState;

            protected abstract void onInit();
            protected abstract void onComplete();
            protected abstract void onClose();

            protected async virtual System.Threading.Tasks.Task<AsyncState> onExecute() { return AsyncState.Running; }

            public System.Threading.Tasks.Task run()
            {
                return System.Threading.Tasks.Task.Run(async () => await this.execute());
            }

            public async System.Threading.Tasks.Task execute()
            {
                this.onInit();

                mState = AsyncState.Running;
                mState = await this.onExecute();
                mState = AsyncState.Complete;

                this.onComplete();
                this.close();
            }

            void ITezCloseable.closeThis()
            {
                this.onClose();
            }
        }

        public class AsyncTask : AsyncBaseTask
        {
            Func<System.Threading.Tasks.Task<AsyncState>> mOnExecute;
            Action mOnComplete = defaultDelegate;
            Action mOnInit = defaultDelegate;

            public AsyncTask(Func<System.Threading.Tasks.Task<AsyncState>> task)
            {
                mOnExecute = task;
            }

            public AsyncTask onInit(Action action)
            {
                mOnInit = action;
                return this;
            }

            public AsyncTask onComplete(Action action)
            {
                mOnComplete = action;
                return this;
            }

            protected override void onClose()
            {
                mOnComplete = null;
                mOnExecute = null;
                mOnInit = null;
            }

            protected override void onComplete()
            {
                mOnComplete();
            }

            protected override void onInit()
            {
                mOnInit();
            }

            protected override System.Threading.Tasks.Task<AsyncState> onExecute()
            {
                return mOnExecute();
            }
        }

        public class AsyncTask<T> : AsyncBaseTask
        {
            Func<System.Threading.Tasks.Task<AsyncState>> mOnExecute;
            Action mOnComplete = defaultDelegate;
            Action mOnInit = defaultDelegate;

            T mUserData;
            public T userData => mUserData;

            public AsyncTask(Func<System.Threading.Tasks.Task<AsyncState>> task)
            {
                mOnExecute = task;
            }

            public AsyncTask<T> initUserData(T userData)
            {
                mUserData = userData;
                return this;
            }

            public AsyncTask<T> onInit(Action action)
            {
                mOnInit = action;
                return this;
            }

            public AsyncTask<T> onComplete(Action action)
            {
                mOnComplete = action;
                return this;
            }

            protected override void onClose()
            {
                mOnComplete = null;
                mOnExecute = null;
                mOnInit = null;
            }

            protected override void onComplete()
            {
                mOnComplete();
            }

            protected override void onInit()
            {
                mOnInit();
            }

            protected override System.Threading.Tasks.Task<AsyncState> onExecute()
            {
                return mOnExecute();
            }
        }

        public class AsyncSequence : AsyncBaseTask
        {
            List<AsyncBaseTask> mList = new List<AsyncBaseTask>();
            Action mOnComplete;

            public AsyncSequence add(AsyncBaseTask task)
            {
                mList.Add(task);
                return this;
            }

            public AsyncSequence onComplete(Action action)
            {
                mOnComplete = action;
                return this;
            }

            protected async override System.Threading.Tasks.Task<AsyncState> onExecute()
            {
                foreach (var item in mList)
                {
                    await item.execute();
                }

                return AsyncState.Complete;
            }

            protected override void onClose()
            {
                foreach (var item in mList)
                {
                    item.close();
                }
                mList.Clear();
                mList = null;

                mOnComplete = null;
            }

            protected override void onComplete()
            {
                mOnComplete();
            }

            protected override void onInit()
            {

            }
        }

        public class AsyncParallel : AsyncBaseTask
        {
            List<AsyncBaseTask> mList = new List<AsyncBaseTask>();
            Action mOnComplete;

            public AsyncParallel add(AsyncBaseTask task)
            {
                mList.Add(task);
                return this;
            }

            public AsyncParallel onComplete(Action action)
            {
                mOnComplete = action;
                return this;
            }

            protected async override System.Threading.Tasks.Task<AsyncState> onExecute()
            {
                System.Threading.Tasks.Task[] ary = new System.Threading.Tasks.Task[mList.Count];

                for (int i = 0; i < mList.Count; i++)
                {
                    ary[i] = mList[i].run();
                }

                await System.Threading.Tasks.Task.WhenAll(ary);

                return AsyncState.Complete;
            }

            protected override void onClose()
            {
                foreach (var item in mList)
                {
                    item.close();
                }
                mList.Clear();
                mList = null;

                mOnComplete = null;
            }

            protected override void onComplete()
            {
                mOnComplete();
            }

            protected override void onInit()
            {

            }
        }


        public static AsyncTask asyncTask(Func<System.Threading.Tasks.Task<AsyncState>> func)
        {
            return new AsyncTask(func);
        }

        public static AsyncTask<T> asyncTask<T>(Func<System.Threading.Tasks.Task<AsyncState>> func)
        {
            return new AsyncTask<T>(func);
        }

        public static AsyncSequence asyncSequence()
        {
            return new AsyncSequence();
        }

        public static AsyncParallel asyncParallel()
        {
            return new AsyncParallel();
        }
        #endregion

        #region Task

        public enum State
        {
            Init,
            Await,
            Complete,
        }

        public interface ITezTaskDispather
        {
            void dispath();
        }

        public interface IExecutor
        {
            BaseTask masterTask { get; }
            bool isCompleted { get; }

            void execute();
            void setComplete();
        }

        public abstract class BaseExecutor
            : ITezCloseable
            , IExecutor
        {
            protected BaseTask mTask = null;
            BaseTask IExecutor.masterTask => mTask;

            bool mIsCompleted = false;
            bool IExecutor.isCompleted => mIsCompleted;

            public BaseExecutor(BaseTask task)
            {
                mTask = task;
                mIsCompleted = false;
            }

            public abstract void execute();

            public void setComplete()
            {
                mIsCompleted = true;
            }

            void ITezCloseable.closeThis()
            {
                mTask = null;
                this.onClose();
            }

            protected abstract void onClose();
        }

        public class Executor : BaseExecutor
        {
            Action<Executor> mAction = null;

            public Executor(BaseTask task, Action<Executor> action) : base(task)
            {
                mAction = action;
            }

            public override void execute()
            {
                mAction(this);
            }

            protected override void onClose()
            {
                mAction = null;
            }
        }

        public class Executor<T> : BaseExecutor
        {
            Action<Executor<T>> mAction = null;

            public Task<T> masterTask => (Task<T>)mTask;

            public Executor(Task<T> task, Action<Executor<T>> action) : base(task)
            {
                mAction = action;
            }

            public override void execute()
            {
                mAction(this);
            }

            protected override void onClose()
            {
                mAction = null;
            }
        }

        public interface ITask
        {
            void setComplete();
        }

        public interface ITaskParent
        {
            void setComplete();
        }

        public abstract class BaseTask
            : ITezCloseable
            , ITask
        {
            protected State mState = State.Init;
            public State state => mState;

            protected ITaskParent mParent = null;

            public abstract BaseExecutor executor { get; }
            public abstract void insert(BaseTask task);

            public void dispath()
            {
                this.onDispath();
                mState = State.Await;
                if (this.executor != null)
                {
                    sTaskExecutor.AddLast(this.executor);
                }
            }

            public void insert()
            {
                this.insert(this);
            }

            public abstract void setComplete();

            public void setParent(ITaskParent parent)
            {
                mParent = parent;
            }

            public void run()
            {
                sTaskDispather.Enqueue(this);
            }

            protected abstract void onDispath();
            protected abstract void onClose();

            void ITezCloseable.closeThis()
            {
                this.onClose();
                mParent = null;
            }
        }


        public class Task : BaseTask
        {
            Action<Task> mOnDispath = defaultDelegate;
            Action mOnComplete = defaultDelegate;
            Executor mExecutor = null;

            public override BaseExecutor executor => mExecutor;

            public Task(Action<Task> dispathTask)
            {
                mOnDispath = dispathTask;
            }

            public Task createAgentExecutor(Action<Executor> action)
            {
                mExecutor = new Executor(this, action);
                return this;
            }

            public Task onComplete(Action func)
            {
                mOnComplete = func;
                return this;
            }

            protected override void onDispath()
            {
                mOnDispath(this);
            }

            public override void setComplete()
            {
                mState = State.Complete;
                mOnComplete.Invoke();
                mParent?.setComplete();
            }

            protected override void onClose()
            {
                mExecutor?.close();
                mExecutor = null;

                mOnDispath = null;
                mOnComplete = null;
            }

            public override void insert(BaseTask task)
            {

            }
        }

        public class Task<T> : BaseTask
        {
            Action<Task<T>> mOnDispath = defaultDelegate;
            Action<Task<T>> mOnComplete = defaultDelegate;

            Executor<T> mExecutor = null;
            public override BaseExecutor executor => mExecutor;

            T mUserData;
            public T userData => mUserData;

            public Task(Action<Task<T>> dispathFunction)
            {
                mOnDispath = dispathFunction;
            }

            public Task<T> createAgentExecutor(Action<Executor<T>> executor)
            {
                mExecutor = new Executor<T>(this, executor);
                return this;
            }

            protected override void onDispath()
            {
                mOnDispath(this);
            }

            public Task<T> initUserData(T userData)
            {
                mUserData = userData;
                return this;
            }

            public Task<T> onComplete(Action<Task<T>> func)
            {
                mOnComplete = func;
                return this;
            }

            public override void setComplete()
            {
                mState = State.Complete;
                mOnComplete.Invoke(this);
                mParent?.setComplete();
            }

            protected override void onClose()
            {
                mExecutor?.close();

                mExecutor = null;
                mOnDispath = null;
                mOnComplete = null;
            }

            public override void insert(BaseTask task)
            {

            }
        }


        #region Sequence
        public class Sequence
            : BaseTask
            , ITaskParent
        {
            List<BaseTask> mList = new List<BaseTask>();
            int mIndex = 0;
            Action mOnComplete = defaultDelegate;

            public override BaseExecutor executor => null;

            public Sequence add(BaseTask task)
            {
                task.setParent(this);
                mList.Add(task);
                return this;
            }

            protected override void onDispath()
            {
                mIndex = 0;
                mList[mIndex].dispath();
            }

            public Sequence onComplete(Action action)
            {
                mOnComplete = action;
                return this;
            }

            public override void setComplete()
            {
                if (mList[mIndex].state == State.Complete)
                {
                    mIndex++;
                    if (mIndex >= mList.Count)
                    {
                        mState = State.Complete;
                        mOnComplete.Invoke();
                        mParent?.setComplete();
                        return;
                    }
                }

                mList[mIndex].dispath();
            }

            protected override void onClose()
            {
                foreach (var task in mList)
                {
                    task.close();
                }

                mList.Clear();
                mList = null;
                mOnComplete = null;
            }

            public override void insert(BaseTask task)
            {
                mList.Insert(mIndex + 1, task);
            }
        }
        #endregion

        #region Parallel
        public class Parallel
            : BaseTask
            , ITaskParent
        {
            List<BaseTask> mList = new List<BaseTask>();
            Action mOnComplete = defaultDelegate;
            int mIndex = 0;

            public override BaseExecutor executor => null;

            public Parallel add(BaseTask task)
            {
                mList.Add(task);
                task.setParent(this);
                return this;
            }

            protected override void onDispath()
            {
                foreach (var item in mList)
                {
                    item.dispath();
                }
            }

            public Parallel onComplete(Action action)
            {
                mOnComplete = action;
                return this;
            }

            public override void setComplete()
            {
                mIndex++;
                if (mIndex == mList.Count)
                {
                    mState = State.Complete;
                    mOnComplete.Invoke();
                    mParent?.setComplete();
                }
            }

            protected override void onClose()
            {
                foreach (var task in mList)
                {
                    task.close();
                }

                mList.Clear();
                mList = null;

                mOnComplete = null;
            }

            public override void insert(BaseTask task)
            {
                mList.Insert(mIndex + 1, task);
            }
        }
        #endregion

        static Queue<BaseTask> sTaskDispather = new Queue<BaseTask>();
        static LinkedList<IExecutor> sTaskExecutor = new LinkedList<IExecutor>();
        static LinkedListNode<BaseTask> mCurrentTask = null;
        public static BaseTask currentTask => mCurrentTask.Value;


        private static void add(BaseTask task)
        {
            if (mCurrentTask != null)
            {
                mCurrentTask.Value.insert(task);
            }
            else
            {

            }
        }

        public static Task task(Action<Task> dispathFunction)
        {
            return new Task(dispathFunction);
        }

        public static Task<T> task<T>(Action<Task<T>> dispathFunction)
        {
            return new Task<T>(dispathFunction);
        }

        public static Sequence sequence()
        {
            return new Sequence();
        }

        public static Parallel parallel()
        {
            return new Parallel();
        }

        public static void update()
        {
            dispath();
            execute();
        }

        private static void dispath()
        {
            while (sTaskDispather.Count > 0)
            {
                sTaskDispather.Dequeue().dispath();
            }
        }

        private static void execute()
        {
            var executor_node = sTaskExecutor.First;
            while (executor_node != null)
            {
                var executor = executor_node.Value;
                executor.execute();
                if (executor.isCompleted)
                {
                    sTaskExecutor.Remove(executor_node);
                    executor.masterTask.setComplete();
                }

                executor_node = executor_node.Next;
            }
        }
        #endregion
    }
}