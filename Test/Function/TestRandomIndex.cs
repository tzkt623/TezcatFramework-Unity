using System;
using tezcat.Framework.Core;
using tezcat.Framework.TMath;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Test
{
    public class TestRandomIndex : TezBaseTest
    {
        TezRandom mRandom = null;
        TezRandomIndex mIndexRandom = null;
        TezRandomIndex mIndexRandom2 = null;
        int[] mIndexArray = null;

        public TestRandomIndex() : base("RandomIndex")
        {

        }

        protected override void onClose()
        {
            mIndexRandom.close();
            mIndexRandom2.close();

            mIndexArray = null;
            mIndexRandom = null;
            mIndexRandom2 = null;
            mRandom = null;
        }

        public override void init()
        {
            mIndexArray = new int[]
            {
                23,
                345,
                57,
                1,
                98,
                34,
                44,
                3,
                9,
                2
            };

            mRandom = new TezRandom();
            mIndexRandom = new TezRandomIndex(0, 0, mRandom.nextInt);
            mIndexRandom2 = new TezRandomIndex(mIndexArray, mRandom.nextInt);
        }

        public override void run()
        {
            Console.WriteLine("Random with 10 int numbers:");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(mIndexRandom.nextIndex());
                Console.Write(",");
            }
            Console.WriteLine("");

            Console.WriteLine("");
            Console.WriteLine("Random with a numbers array");
            Console.Write("Array numbers:");
            foreach (var item in mIndexArray)
            {
                Console.Write(item);
                Console.Write(",");
            }

            Console.WriteLine("");
            Console.Write("Random:");
            for (int i = 0; i < 10; i++)
            {
                Console.Write(mIndexRandom2.nextIndex());
                Console.Write(",");
            }
        }
    }
}

