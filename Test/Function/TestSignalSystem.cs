using System;

namespace tezcat.Framework.Test
{
    public class TestSignalSystem : TezBaseTest
    {
        public TestSignalSystem() : base("SignalSystem")
        {

        }

        public override void init()
        {
            TezcatFramework.signalSystem.registerSignal("TestSignal", () =>
            {
                Console.WriteLine("Signal Triggered");
            }, 
            new string[] { "0", "1", "2", "3" });
        }

        public override void run()
        {
            TezcatFramework.signalSystem.emitSignal("TestSignal", "0");
            TezcatFramework.signalSystem.emitSignal("TestSignal", "3");
            TezcatFramework.signalSystem.emitSignal("TestSignal", "2");
            TezcatFramework.signalSystem.emitSignal("TestSignal", "1");
        }

        protected override void onClose()
        {
            TezcatFramework.signalSystem.clear();
        }
    }
}
