using System;
using System.Collections.Generic;
using System.Threading;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using UnityEngine;

namespace tezcat.Framework.Threading
{
    public class TezThread : ITezService
    {
        int m_IDGiver = 0;

        List<Task> m_Task = new List<Task>();

        public void close()
        {
            m_Task.Clear();
            m_Task = null;
        }

        /// <summary>
        /// 创建一个多线程任务
        /// <para>只有on_run方法执行于线程中</para>
        /// <para>其他方法都在主线程中执行</para>
        /// </summary>
        public Task add(
            TezEventExtension.Action<Task> on_start,
            TezEventExtension.Action<Task> on_run,
            TezEventExtension.Action<Task> on_complete,
            TezEventExtension.Action<Task> on_terminate)
        {
            return this.add("Default_Thread", on_start, on_run, on_complete, on_terminate);
        }

        /// <summary>
        /// 创建一个多线程任务
        /// <para>只有on_run方法执行于线程中</para>
        /// <para>其他方法都在主线程中执行</para>
        /// </summary>
        public Task add(
            string name,
            TezEventExtension.Action<Task> on_start,
            TezEventExtension.Action<Task> on_run,
            TezEventExtension.Action<Task> on_complete,
            TezEventExtension.Action<Task> on_terminate)
        {
            var task = new Task(name, on_start, on_run, on_complete, on_terminate);
            m_Task.Add(task);
            return task;
        }

        public void update()
        {
            for (int i = m_Task.Count - 1; i >= 0; i--)
            {
                var task = m_Task[i];
                switch (task.state)
                {
                    case Task.State.Start:
                        task.onStart();
                        break;
                    case Task.State.ResetToRun:
                        task.onReset();
                        break;
                    case Task.State.ResumeToRun:
                        task.onResume();
                        break;
                    case Task.State.CompleteToPause:
                        task.onComplete();
                        break;
                    case Task.State.Terminate:
                        task.onTerminate();
                        m_Task.RemoveAt(i);
                        task.close();
                        break;
                    default:
                        break;
                }
            }
        }

        public class Task : ITezCloseable
        {
            protected object m_LockObject = new object();
            public string name { get; private set; } = string.Empty;

            AutoResetEvent m_AutoEvent = new AutoResetEvent(true);

            public DateTime startedAt { get; private set; }
            public DateTime lastRun { get; private set; }
            public TimeSpan lastRuntime { get; private set; }
            public TimeSpan runtimeHighwater { get; private set; } = TimeSpan.Zero;

            private bool m_Running = false;
            private Thread m_Thread = null;

            private TezEventExtension.Action<Task> m_OnStart = null;
            private TezEventExtension.Action<Task> m_OnRun = null;
            private TezEventExtension.Action<Task> m_OnComplete = null;
            private TezEventExtension.Action<Task> m_OnTerminate = null;

            public enum State
            {
                Start,
                Pause,
                Run,
                Terminate,
                CompleteToPause,
                ResumeToRun,
                ResetToRun
            }

            public State state { get; private set; } = State.Start;

            public Task(
                string name,
                TezEventExtension.Action<Task> on_start,
                TezEventExtension.Action<Task> on_run,
                TezEventExtension.Action<Task> on_complete,
                TezEventExtension.Action<Task> on_terminate)
            {
                m_OnStart = on_start == null ? defaultFunction : on_start;
                m_OnRun = on_run == null ? defaultFunction : on_run;
                m_OnComplete = on_complete == null ? defaultFunction : on_complete;
                m_OnTerminate = on_terminate == null ? defaultFunction : on_terminate;
                this.name = name;
            }

            public void close()
            {
                m_Thread?.Abort();
                m_AutoEvent.Close();

                this.name = null;
                m_LockObject = null;
                m_AutoEvent = null;
                m_Thread = null;
                m_OnStart = null;
                m_OnRun = null;
                m_OnComplete = null;
                m_OnTerminate = null;
            }

            private void defaultFunction(Task task) { }

            #region 被动操作函数
            public void onStart()
            {
                lock (m_LockObject)
                {
                    m_OnStart(this);
                    state = State.Run;

                    m_Thread = new Thread(new ThreadStart(this.run));
                    m_Thread.Name = this.name;
                    m_Thread.IsBackground = true;
                    m_Thread.Start();
                }
            }

            public void onReset()
            {
                lock (m_LockObject)
                {
                    m_OnStart(this);
                    state = State.Run;
                    m_AutoEvent.Set();
                }
            }

            public void onComplete()
            {
                lock (m_LockObject)
                {
                    m_OnComplete(this);
                    state = State.Pause;
                }
            }

            public void onResume()
            {
                lock (m_LockObject)
                {
                    m_OnStart(this);
                    state = State.Run;
                    m_AutoEvent.Set();
                }
            }

            public void onTerminate()
            {
                lock (m_LockObject)
                {
                    m_OnTerminate(this);
                }
            }
            #endregion

            public void terminate()
            {
                lock (m_LockObject)
                {
                    if (m_Running)
                    {
                        m_Running = false;
                    }
                }
            }

            public void pause()
            {
                lock (m_LockObject)
                {
                    state = State.CompleteToPause;
                }
            }

            public void resume()
            {
                lock (m_LockObject)
                {
                    state = State.ResumeToRun;
                }
            }

            public void reset(
                TezEventExtension.Action<Task> on_start = null,
                TezEventExtension.Action<Task> on_progress = null,
                TezEventExtension.Action<Task> on_complete = null,
                TezEventExtension.Action<Task> on_terminate = null)
            {
                lock (m_LockObject)
                {
                    m_OnStart = on_start == null ? defaultFunction : on_start;
                    m_OnRun = on_progress == null ? defaultFunction : on_progress;
                    m_OnComplete = on_complete == null ? defaultFunction : on_complete;
                    m_OnTerminate = on_terminate == null ? defaultFunction : on_terminate;

                    state = State.ResetToRun;
                }
            }

            public void sleep(int msc)
            {
                Thread.Sleep(msc);
            }

            private void run()
            {
                try
                {
                    m_Running = true;
                    while (m_Running)
                    {
                        switch (state)
                        {
                            case State.Pause:
                                {
                                    m_AutoEvent.WaitOne();
                                }
                                break;
                            case State.Run:
                                {
                                    DateTime now = DateTime.Now;
                                    m_OnRun(this);
                                    this.lastRun = now;
                                    this.lastRuntime = DateTime.Now - now;
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    this.state = State.Terminate;
                }
                catch (ThreadAbortException ex)
                {
                    Debug.Log(ex.ToString());
                }
                catch (ThreadInterruptedException ex2)
                {
                    Debug.Log(ex2.ToString());
                }
                catch (Exception ex3)
                {
                    Debug.Log(ex3.ToString());
                    m_Running = false;
                }
                finally
                {
                    m_Running = false;
                }
            }

            public override string ToString()
            {
                return this.name;
            }
        }
    }
}