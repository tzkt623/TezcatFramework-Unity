using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    class TestStackStepSystem : TezBaseTest
    {
        public TestStackStepSystem() : base("StackStepSystem")
        {

        }

        public override void init()
        {
            Console.WriteLine("Press any key to update, Press ESC to exit");


            TezStackStep step = new TezStackStep();
            step.addStep((s) =>
            {
                Console.WriteLine("Step 1.0");
            });

            step.addStep((s) =>
            {
                Console.WriteLine("Step 1.1");

                var ss = new TezStackStep();
                ss.addStep((s1) =>
                {
                    Console.WriteLine("Step 2.0");
                });

                ss.addStep((s1) =>
                {
                    Console.WriteLine("Step 2.1");

                    s1.setWishedTiming(TezStackStepWishedTime.Abort);
                    s.setWishedTiming(TezStackStepWishedTime.Abort);
                });

                ss.addStep((s1) =>
                {
                    Console.WriteLine("Step 2.2");
                });

                s.wait(ss);
            });

            step.addStep((s) =>
            {
                Console.WriteLine("Step 1.2");
            });

            TezStackStepSystem.instance.runInNextFrame<TezStackStepContainer>(step);
        }

        public override void run()
        {

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                if(TezStackStepSystem.instance.isEmpty())
                {
                    Console.WriteLine("All Steps Complete");
                }
                else
                {
                    TezStackStepSystem.instance.update();
                }
            }
        }

        protected override void onClose()
        {

        }
    }
}