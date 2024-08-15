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

        int mRunningCount = 0;

        public TestTask() : base("Task")
        {

        }

        public override void init()
        {
            mRunningCount = 0;
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
                    executor.reportComplete();
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
                    executor.reportComplete();
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
                    executor.reportComplete();
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
                    executor.reportComplete();
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

        private void enemyPrepare(TezTask.ITask preTask)
        {
            var seq = TezTask.sequence()
                .onComplete(() =>
                {
                    preTask.reportComplete();
                });

            var task1 = TezTask.task<Timer>((me) =>
            {
                me.userData.seconds = 2;
                me.userData.begin = DateTime.Now;
                Console.WriteLine("Deploying Ships ......");
            })
            .createAgentExecutor(executor =>
            {
                if (executor.masterTask.userData.timeOut())
                {
                    executor.reportComplete();
                }
            })
            .onComplete((me) =>
            {
                Console.WriteLine("Deploying Completes ......");
            })
            .initUserData(new Timer());

            var task2 = TezTask.task((me) =>
            {
                Console.WriteLine("Thinking ......");
                this.aiThinking(me);
            })
            .onComplete(() =>
            {
                Console.WriteLine("Thinking Completes ......");
            });

            seq.add(task1);
            seq.add(task2);

            for (int i = 0; i < 4; i++)
            {
                int id = i;
                var taskx = TezTask.task((me) =>
                {
                    this.movingShip(me, id);
                });

                seq.add(taskx);
            }

            seq.run();
        }

        private void movingShip(TezTask.ITask preTask, int index)
        {
            TezTask.task<Timer>((me) =>
            {
                Console.WriteLine($"Moving Ship ...... {index}");
                me.userData.seconds = 2;
                me.userData.begin = DateTime.Now;
            })
            .createAgentExecutor(executor =>
            {
                if (executor.masterTask.userData.timeOut())
                {
                    executor.reportComplete();
                }
            })
            .onComplete(me=>
            {
                Console.WriteLine($"Moving Completes ...... {index}");
                preTask.reportComplete();
            })
            .initUserData(new Timer())
            .run();
        }

        private void aiThinking(TezTask.ITask preTask)
        {
            TezTask.task<Timer>(me =>
            {
                me.userData.seconds = 2;
                me.userData.begin = DateTime.Now;
            })
            .createAgentExecutor(executor =>
            {
                if (executor.masterTask.userData.timeOut())
                {
                    executor.reportComplete();
                }
            })
            .onComplete(me=>
            {
                preTask.reportComplete();
            })
            .initUserData(new Timer())
            .run();
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

            while (mRunningCount < 3)
            {
                TezTask.update();
            }
        }

        protected override void onClose()
        {

        }
    }

    public class TestTaskAsync : TezBaseTest
    {
        int mRunningCount = 0;

        public TestTaskAsync() : base("TaskAsync")
        {

        }

        public override void init()
        {
            mRunningCount = 0;
        }

        private void sequence()
        {
            var task1 = TezTask.asyncTask(async () =>
            {
                int ms = 1000;
                Console.WriteLine($"Task1({System.Threading.Thread.CurrentThread.ManagedThreadId}) >> Dispath Wait {ms / 1000}s");
                await Task.Delay(ms);
            })
            .onComplete(() =>
            {
                Console.WriteLine($"Task1({System.Threading.Thread.CurrentThread.ManagedThreadId}) >> Complete");
            });

            var task2 = TezTask.asyncTask(async () =>
            {
                int ms = 4000;
                Console.WriteLine($"Task2({System.Threading.Thread.CurrentThread.ManagedThreadId}) >> Dispath Wait {ms / 1000}s");
                await Task.Delay(ms);
            })
            .onComplete(() =>
            {
                Console.WriteLine($"Task2({System.Threading.Thread.CurrentThread.ManagedThreadId}) >> Complete");
            });

            TezTask.asyncSequence()
                .add(task1)
                .add(task2)
                .onComplete(() =>
                {
                    mRunningCount++;
                })
                .run();
        }

        private void parallel()
        {
            var task1 = TezTask.asyncTask(async () =>
            {
                int ms = 6000;
                Console.WriteLine($"Task3({System.Threading.Thread.CurrentThread.ManagedThreadId}) >> Dispath Wait {ms / 1000}s");
                await Task.Delay(ms);
            })
            .onComplete(() =>
            {
                Console.WriteLine($"Task3({System.Threading.Thread.CurrentThread.ManagedThreadId}) >> Complete");
            });

            var task2 = TezTask.asyncTask(async () =>
            {
                int ms = 8000;
                Console.WriteLine($"Task4({System.Threading.Thread.CurrentThread.ManagedThreadId}) >> Dispath Wait {ms / 1000}s");
                await Task.Delay(ms);
            })
            .onComplete(() =>
            {
                Console.WriteLine($"Task4({System.Threading.Thread.CurrentThread.ManagedThreadId}) >> Complete");
            });

            TezTask.asyncParallel()
                .add(task1)
                .add(task2)
                .onComplete(() =>
                {
                    mRunningCount++;
                })
                .run();
        }

        public override void run()
        {
            Console.WriteLine($"Main Thread({System.Threading.Thread.CurrentThread.ManagedThreadId}) >> Waiting");

            this.sequence();
            this.parallel();

            while (mRunningCount < 2)
            {

            }
        }

        protected override void onClose()
        {
            Console.WriteLine("END");
        }
    }
}