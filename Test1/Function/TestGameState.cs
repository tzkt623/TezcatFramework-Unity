using System;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    class PrimaryState
    {
        public static readonly TezGameState<PrimaryState>.Mask Begin = TezGameState<PrimaryState>.createOrGet("Begin");
        public static readonly TezGameState<PrimaryState>.Mask End = TezGameState<PrimaryState>.createOrGet("End");
        public static readonly TezGameState<PrimaryState>.Mask PlayerTurn = TezGameState<PrimaryState>.createOrGet("PlayerTurn");
        public static readonly TezGameState<PrimaryState>.Mask Deploy = TezGameState<PrimaryState>.createOrGet("Deploy");
        public static readonly TezGameState<PrimaryState>.Mask Wating = TezGameState<PrimaryState>.createOrGet("Wating");
    }


    class TestGameState : TezBaseTest
    {
        public TestGameState() : base("GameState")
        {

        }

        public override void init()
        {
            TezGameState<PrimaryState>.evtPrintState += onPrintState;
        }

        private void onPrintState(TezGameState<PrimaryState>.Mask mask)
        {
            Console.Write($"{mask.name},");
        }

        public override void run()
        {
            var test_mask = PrimaryState.Begin | PrimaryState.Wating;

            Console.WriteLine("add Begin");
            TezGameState<PrimaryState>.add(PrimaryState.Begin);

            Console.WriteLine($"onlyof [Begin|Wating]:{TezGameState<PrimaryState>.onlyOf(test_mask)}");
            Console.WriteLine($"anyOf [Begin|Wating]:{TezGameState<PrimaryState>.anyOf(test_mask)}");
            Console.WriteLine($"noneOf [Begin|Wating]:{TezGameState<PrimaryState>.noneOf(test_mask)}");
            Console.WriteLine($"allOf [Begin|Wating]:{TezGameState<PrimaryState>.allOf(test_mask)}");

            Console.WriteLine("remove Begin");
            TezGameState<PrimaryState>.remove(PrimaryState.Begin);

            Console.WriteLine("add PlayerTurn");
            Console.WriteLine("add Wating");
            TezGameState<PrimaryState>.add(PrimaryState.PlayerTurn | PrimaryState.Wating);

            Console.WriteLine("remove Wating");
            TezGameState<PrimaryState>.remove(PrimaryState.Wating);

            Console.WriteLine("add Wating");
            TezGameState<PrimaryState>.add(PrimaryState.Deploy);


            TezGameState<PrimaryState>.add(PrimaryState.End);

            Console.WriteLine("\nPrint State");
            TezGameState<PrimaryState>.printState();
            Console.WriteLine();

            Console.WriteLine($"allOf [End|PlayerTurn]:{TezGameState<PrimaryState>.allOf(PrimaryState.End | PrimaryState.PlayerTurn)}");
        }

        protected override void onClose()
        {

        }
    }
}