using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    public class TestStepSystem2 : TezBaseTest
    {
        public TestStepSystem2() : base("StepSystem2")
        {

        }

        public override void init()
        {

        }

        public override void run()
        {
            Console.WriteLine("Press any key to update, Press ESC to exit");

            TezStepGroup step2 = new TezStepGroup();
            step2.addStep((me) =>
            {
                Console.WriteLine("Step 1.0");
            });

            step2.addStep((me) =>
            {
                Console.WriteLine("Step 1.1");
                me.wait<TezStepStackExecutor>(this.createStep(2));
                //me.wait<TezStepStackExecutor>(this.createStep(3));
            });

            step2.addStep((me) =>
            {
                Console.WriteLine("Step 1.2");
            });

            step2.addStep((me) =>
            {
                Console.WriteLine("Step 1.3");
            });

            TezStepSystem2.instance.runInNextFrame<TezStepStackExecutor>(step2);

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                if (TezStepSystem2.instance.isEmpty())
                {
                    Console.WriteLine("Empty");
                }
                else
                {
                    TezStepSystem2.instance.update();
                }
            }
        }

        private TezStepGroup createStep(int x)
        {
            var step = new TezStepGroup();

            step.addStep((me) =>
            {
                Console.WriteLine($"Step {x}.0");
            });

            step.addStep((me) =>
            {
                Console.WriteLine($"Step {x}.1");
            });

            step.addStep((me) =>
            {
                Console.WriteLine($"Step {x}.2");
            });

            step.addStep((me) =>
            {
                Console.WriteLine($"Step {x}.3");
            });


            return step;
        }

        protected override void onClose()
        {

        }
    }
}