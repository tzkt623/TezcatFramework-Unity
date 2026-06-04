using System;
using System.Collections.Generic;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    class TestStepSystem : TezBaseTest
    {
        TezStepSystem.IStep mGroup;
        Queue<TezStepSystem.IStep> mQueue = new Queue<TezStepSystem.IStep>();

        public TestStepSystem() : base("StepSystem")
        {

        }

        public override void init()
        {

        }

        public void test()
        {
            var group = TezStepSystem.manager.createStepGroup("T1");

            group.addStep((self) =>
            {
                Console.WriteLine("T1 Step 1");
                //self.suspend();
            });

            group.addStep((self) =>
            {
                Console.WriteLine("T1 Step 2");
                self.wait(this.createStep("T2"));
                //self.suspend();
            });

            group.addStep((self) =>
            {
                Console.WriteLine("T1 Step 3");
                self.wait(this.createStep("T3"));
                self.wait(this.createStep("T4"));
            });

            group.addStep((self) =>
            {
                Console.WriteLine("T1 Step 4");
            });

            TezStepSystem.manager.run(group);
        }

        public TezStepSystem.IStep createStep(string groupName)
        {
            var group = TezStepSystem.manager.createStepGroup(groupName);

            group.addStep((self) =>
            {
                Console.WriteLine($"{groupName} Step 1");
                //self.suspend();
            });

            group.addStep((self) =>
            {
                Console.WriteLine($"{groupName} Step 2 suspend");
                //self.suspend();
                //self.parent.abort();
                //self.abort();
            });

            group.addStep((self) =>
            {
                Console.WriteLine($"{groupName} Step 3");
                //self.suspend();
                self.wait(this.createStep2($"{groupName}-1"));
            });

            //mGroup = group;

            mQueue.Enqueue(group);
            return group;
        }

        public TezStepSystem.IStep createStep2(string groupName)
        {
            var group = TezStepSystem.manager.createStepGroup(groupName);

            group.addStep((self) =>
            {
                Console.WriteLine($"{groupName} Step 1");
                //self.suspend();
            });

            group.addStep((self) =>
            {
                Console.WriteLine($"{groupName} Step 2 suspend");
                //self.suspend();
            });

            group.addStep((self) =>
            {
                Console.WriteLine($"{groupName} Step 3");
                //self.suspend();
            });

            //mGroup = group;

            mQueue.Enqueue(group);
            return group;
        }

        public override void run()
        {
            this.test();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    break;
                }

//                 if (key.Key == ConsoleKey.Spacebar && mQueue.Count > 0)
//                 {
//                     mQueue.Dequeue().resume();
//                 }

                TezStepSystem.manager.update();
            }
        }

        protected override void onClose()
        {
            mGroup = null;
            TezStepSystem.manager.clear();
        }
    }
}