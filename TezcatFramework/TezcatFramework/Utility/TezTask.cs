using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 
    /// 任务系统
    /// 
    /// <para>
    /// 基本逻辑
    /// 发布一个任务,当前任务状态变为等待,等待任务执行完成
    /// 比如发布一个执行动画的任务,动画就自己去执行了
    /// 具体谁负责执行,不重要,执行完毕之后通知当前任务执行完毕
    /// </para>
    /// 
    /// <para>
    /// 附加功能
    /// 执行器代理
    /// 可以使用执行器代理来帮助执行任务,执行器代理每一帧执行一次
    /// 注意!不要在执行器代理中写循环执行逻辑,只能写一帧的逻辑
    /// </para>
    /// 
    /// 
    /// C#自带的await会卡住当前函数位置,当前await执行完毕之后
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
        static void defaultDelegate<T>(AsyncTask<T> task) { }

        #region AsyncTask
        public abstract class AsyncBaseTask : ITezCloseable
        {
            protected abstract void onComplete();
            protected abstract void onClose();

            protected async virtual System.Threading.Tasks.Task onExecute() { }

            public System.Threading.Tasks.Task run()
            {
                return System.Threading.Tasks.Task.Run(this.execute);
            }

            public async System.Threading.Tasks.Task execute()
            {
                await this.onExecute();

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
            Func<System.Threading.Tasks.Task> mOnExecute;
            Action mOnComplete = defaultDelegate;

            public AsyncTask(Func<System.Threading.Tasks.Task> task)
            {
                mOnExecute = task;
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
            }

            protected override void onComplete()
            {
                mOnComplete();
            }

            protected override System.Threading.Tasks.Task onExecute()
            {
                return mOnExecute();
            }
        }

        public class AsyncTask<T> : AsyncBaseTask
        {
            Func<AsyncTask<T>, System.Threading.Tasks.Task> mOnExecute;
            Action<AsyncTask<T>> mOnComplete = defaultDelegate;

            T mUserData;
            public T userData
            {
                get { return mUserData; }
                set { mUserData = value; }
            }

            public AsyncTask(Func<AsyncTask<T>, System.Threading.Tasks.Task> task)
            {
                mOnExecute = task;
            }

            public AsyncTask<T> initUserData(T userData)
            {
                mUserData = userData;
                return this;
            }

            public AsyncTask<T> onComplete(Action<AsyncTask<T>> action)
            {
                mOnComplete = action;
                return this;
            }

            protected override void onClose()
            {
                mOnComplete = null;
                mOnExecute = null;
            }

            protected override void onComplete()
            {
                mOnComplete(this);
            }

            protected override System.Threading.Tasks.Task onExecute()
            {
                return mOnExecute(this);
            }
        }

        public class AsyncSequence : AsyncBaseTask
        {
            List<AsyncBaseTask> mList = new List<AsyncBaseTask>();
            Action mOnComplete = defaultDelegate;

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

            protected async override System.Threading.Tasks.Task onExecute()
            {
                foreach (var item in mList)
                {
                    await item.execute();
                }
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
        }

        public class AsyncParallel : AsyncBaseTask
        {
            List<AsyncBaseTask> mList = new List<AsyncBaseTask>();
            Action mOnComplete = defaultDelegate;

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

            protected async override System.Threading.Tasks.Task onExecute()
            {
                System.Threading.Tasks.Task[] ary = new System.Threading.Tasks.Task[mList.Count];

                for (int i = 0; i < mList.Count; i++)
                {
                    ary[i] = mList[i].run();
                }

                await System.Threading.Tasks.Task.WhenAll(ary);
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
        }


        public static AsyncTask asyncTask(Func<System.Threading.Tasks.Task> func)
        {
            return new AsyncTask(func);
        }

        public static AsyncTask<T> asyncTask<T>(Func<AsyncTask<T>, System.Threading.Tasks.Task> func)
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
            void reportComplete();
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

            public void reportComplete()
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
            void reportComplete();
        }

        public interface ITaskParent
        {
            void reportComplete();
            void childComplete();
        }

        public interface ITaskExecutor
        {
            BaseExecutor executor { get; }
        }

        public abstract class BaseTask
            : ITezCloseable
            , ITask
        {
            protected State mState = State.Init;
            public State state => mState;
            protected ITaskParent mParent = null;

            public void dispath()
            {
                mState = State.Await;
                this.onDispath();
            }

            public void reportComplete()
            {
                mState = State.Complete;
                this.onComplete();
                mParent?.childComplete();
            }

            protected abstract void onComplete();

            public void setParent(ITaskParent parent)
            {
                mParent = parent;
            }

            public void run()
            {
                sTaskQueue.Enqueue(this);
            }

            protected abstract void onDispath();
            protected abstract void onClose();

            void ITezCloseable.closeThis()
            {
                this.onClose();
                mParent = null;
            }
        }


        public class Task
            : BaseTask
            , ITaskExecutor
        {
            Action<Task> mOnDispath = defaultDelegate;
            Action mOnComplete = defaultDelegate;
            Executor mExecutor = null;

            public BaseExecutor executor => mExecutor;

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
                if (mExecutor != null)
                {
                    sExecutorList.AddLast(mExecutor);
                }
            }

            protected override void onComplete()
            {
                mOnComplete.Invoke();
            }

            protected override void onClose()
            {
                mExecutor?.close();
                mExecutor = null;

                mOnDispath = null;
                mOnComplete = null;
            }
        }

        public class Task<T>
            : BaseTask
            , ITaskExecutor
        {
            Action<Task<T>> mOnDispath = defaultDelegate;
            Action<Task<T>> mOnComplete = defaultDelegate;

            Executor<T> mExecutor = null;
            public BaseExecutor executor => mExecutor;

            T mUserData;
            public T userData
            {
                get { return mUserData; }
                set { mUserData = value; }
            }

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
                if (mExecutor != null)
                {
                    sExecutorList.AddLast(mExecutor);
                }
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

            protected override void onComplete()
            {
                mOnComplete.Invoke(this);
            }

            protected override void onClose()
            {
                mExecutor?.close();

                mExecutor = null;
                mOnDispath = null;
                mOnComplete = null;
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

            public Sequence add(BaseTask task)
            {
                task.setParent(this);
                mList.Add(task);
                return this;
            }

            protected override void onDispath()
            {
                if(mList.Count == 0)
                {
                    throw new Exception($"TezTask : Sequence Count {mList.Count}");
                }

                mList[0].dispath();
            }

            public Sequence onComplete(Action action)
            {
                mOnComplete = action;
                return this;
            }

            protected override void onComplete()
            {
                mOnComplete.Invoke();
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

            void ITaskParent.childComplete()
            {
                mIndex++;
                if (mIndex == mList.Count)
                {
                    this.reportComplete();
                }
                else
                {
                    mList[mIndex].dispath();
                }
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

            public Parallel add(BaseTask task)
            {
                mList.Add(task);
                task.setParent(this);
                return this;
            }

            protected override void onDispath()
            {
                if (mList.Count == 0)
                {
                    throw new Exception($"TezTask : Parallel Count {mList.Count}");
                }

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

            protected override void onComplete()
            {
                mOnComplete();
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

            void ITaskParent.childComplete()
            {
                mIndex++;
                if (mIndex == mList.Count)
                {
                    this.reportComplete();
                }
            }
        }
        #endregion

        static Queue<BaseTask> sTaskQueue = new Queue<BaseTask>();
        static LinkedList<IExecutor> sExecutorList = new LinkedList<IExecutor>();

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
            while (sTaskQueue.Count > 0)
            {
                sTaskQueue.Dequeue().dispath();
            }
        }

        private static void execute()
        {
            var executor_node = sExecutorList.First;
            while (executor_node != null)
            {
                var executor = executor_node.Value;
                executor.execute();
                if (executor.isCompleted)
                {
                    var temp = executor_node.Next;
                    sExecutorList.Remove(executor_node);
                    executor.masterTask.reportComplete();
                    executor_node = temp;
                }
                else
                {
                    executor_node = executor_node.Next;
                }
            }
        }
        #endregion
    }
}