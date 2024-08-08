using System;
using System.Threading.Tasks;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Test
{
    public class TestTask : TezBaseTest
    {
        class Timer
        {
            public DateTime begin;
            public int seconds;

            public bool timeOut()
            {
                return (DateTime.Now - begin).Seconds > seconds;
            }
        }

        bool[] mRunning = null;
        int mRunningCount = 0;
        int mAnimationCount = 0;

        public TestTask() : base("Task")
        {

        }

        public override void init()
        {
            mRunning = new bool[2] { true, true };
        }

        private TezTask.BaseTask sequence()
        {
            var task1 = TezTask.task<Timer>((me) =>
            {
                var timer = me.userData;
                timer.seconds = 1;
                timer.begin = DateTime.Now;

                Console.WriteLine($"Task1 >> Dispath Wait {timer.seconds}s ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            })
            .onComplete((me) =>
            {
                Console.WriteLine($"Task1 >> Complete ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            })
            .createAgentExecutor((TezTask.Executor<Timer> executor) =>
            {
                if (executor.masterTask.userData.timeOut())
                {
                    executor.setComplete();
                }
            })
            .initUserData(new Timer());

            var task2 = TezTask.task<Timer>((me) =>
            {
                var timer = me.userData;
                timer.seconds = 4;
                timer.begin = DateTime.Now;

                Console.WriteLine($"Task2 >> Dispath Wait {timer.seconds}s ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            })
            .onComplete((me) =>
            {
                Console.WriteLine($"Task2 >> Complete ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            })
            .createAgentExecutor((TezTask.Executor<Timer> executor) =>
            {
                if (executor.masterTask.userData.timeOut())
                {
                    executor.setComplete();
                }
            })
            .initUserData(new Timer());

            return TezTask.sequence()
                .add(task1)
                .add(task2)
                .onComplete(() =>
                {
                    mRunningCount++;
                });
        }

        private TezTask.BaseTask parallel()
        {
            var task1 = TezTask.task<Timer>((me) =>
            {
                var timer = me.userData;
                timer.seconds = 6;
                timer.begin = DateTime.Now;

                Console.WriteLine($"Task3 >> Dispath Wait {timer.seconds}s ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            })
            .onComplete((me) =>
            {
                Console.WriteLine($"Task3 >> Complete ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            })
            .createAgentExecutor((TezTask.Executor<Timer> executor) =>
            {
                if (executor.masterTask.userData.timeOut())
                {
                    executor.setComplete();
                }
            })
            .initUserData(new Timer());

            var task2 = TezTask.task<Timer>((me) =>
            {
                var timer = me.userData;
                timer.seconds = 8;
                timer.begin = DateTime.Now;

                Console.WriteLine($"Task4 >> Dispath Wait {timer.seconds}s ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            })
            .onComplete((me) =>
            {
                Console.WriteLine($"Task4 >> Complete ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            })
            .createAgentExecutor((TezTask.Executor<Timer> executor) =>
            {
                if (executor.masterTask.userData.timeOut())
                {
                    executor.setComplete();
                }
            })
            .initUserData(new Timer());

            return TezTask.parallel()
                .add(task1)
                .add(task2)
                .onComplete(() =>
                {
                    mRunningCount++;
                });
        }

        private void beginBattle()
        {
            TezTask.task((TezTask.Task me) =>
            {
                //等待此任务执行完成
                //当前task进入等待状态
                this.enemyPrepare(me);
            })
            .onComplete(() =>
            {
                Console.WriteLine("Begin Battle Complete");
            })
            .run();
        }

        private void enemyPrepare(TezTask.ITask preTask)
        {
            TezTask.task<Timer>((me) =>
            {
                me.userData.seconds = 2;
                Console.WriteLine("Deploying Ships ......");
            })
            .createAgentExecutor(executor =>
            {
                if (executor.masterTask.userData.timeOut())
                {
                    executor.setComplete();
                }
            })
            .onComplete((me) =>
            {
                Console.WriteLine("Deploying Complete ......");
                preTask.setComplete();
            })
            .initUserData(new Timer())
            .run();
        }

        public void playAnimation()
        {
            Console.WriteLine("PlayAnimation......");
        }

        private TezTask.BaseTask battle()
        {
            return TezTask.task((me) =>
            {
                //等待此任务执行完成
                //当前task进入等待状态
                this.enemyPrepare(me);
            })
            .onComplete(() =>
            {
                Console.WriteLine("Enemy Prepare Complete");
                mRunningCount++;
            });
        }

        public override void run()
        {
            TezTask.sequence()
                .add(TezTask.parallel()
                    .add(this.sequence())
                    .add(this.parallel()))
                .add(this.battle())
                .run();

            //this.sequence();
            //this.parallel();

            Console.WriteLine($"Main Thread >> Waiting ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

            //while (mRunning[0])
            while (mRunningCount < 3)
            {
                TezTask.update();
            }
        }

        protected override void onClose()
        {
            mRunning = null;
        }
    }

    public class TestTaskAsync : TezBaseTest
    {
        bool[] mRunning = null;

        public TestTaskAsync() : base("TaskAsync")
        {

        }

        public override void init()
        {
            mRunning = new bool[2] { true, true };
        }

        private void sequence()
        {
            var task1 = TezTask.asyncTask(async () =>
            {
                int ms = 1000;
                Console.WriteLine($"Task1 >> Wait {ms / 1000}s ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(ms);

                return TezTask.AsyncState.Complete;
            }).onInit(() =>
            {
                Console.WriteLine($"Task1 >> Init ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            }).onComplete(() =>
            {
                Console.WriteLine($"Task1 >> Complete ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            });

            var task2 = TezTask.asyncTask(async () =>
            {
                int ms = 4000;
                Console.WriteLine($"Task2 >> Wait {ms / 1000}s ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(ms);

                return TezTask.AsyncState.Complete;
            }).onInit(() =>
            {
                Console.WriteLine($"Task2 >> Init ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            }).onComplete(() =>
            {
                Console.WriteLine($"Task2 >> Complete ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            });

            TezTask.asyncSequence()
                .add(task1)
                .add(task2)
                .onComplete(() =>
                {
                    mRunning[0] = false;
                })
                .run();
        }

        private void parallel()
        {
            var task1 = TezTask.asyncTask(async () =>
            {
                int ms = 6000;
                Console.WriteLine($"Task3 >> Wait {ms / 1000}s ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(ms);

                return TezTask.AsyncState.Complete;
            }).onInit(() =>
            {
                Console.WriteLine($"Task3 >> Init ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            }).onComplete(() =>
            {
                Console.WriteLine($"Task3 >> Complete ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            });

            var task2 = TezTask.asyncTask(async () =>
            {
                int ms = 8000;
                Console.WriteLine($"Task4 >> Wait {ms / 1000}s ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
                await Task.Delay(ms);

                return TezTask.AsyncState.Complete;
            }).onInit(() =>
            {
                Console.WriteLine($"Task4 >> Init ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            }).onComplete(() =>
            {
                Console.WriteLine($"Task4 >> Complete ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");
            });

            TezTask.asyncParallel()
                .add(task1)
                .add(task2)
                .onComplete(() =>
                {
                    mRunning[1] = false;
                })
                .run();
        }

        public override void run()
        {
            this.sequence();
            this.parallel();

            Console.WriteLine($"Main Thread >> Waiting ThreadID:{System.Threading.Thread.CurrentThread.ManagedThreadId}");

            while (mRunning[0] || mRunning[1])
            {

            }
        }

        protected override void onClose()
        {
            mRunning = null;
            Console.WriteLine("END");
        }
    }
}