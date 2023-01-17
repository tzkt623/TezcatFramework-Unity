using tezcat.Framework.Math;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Test
{
    public class TestRandomIndex
    {
        public void run()
        {
            TezRandom random = new TezRandom();

            TezRandomIndex r1 = new TezRandomIndex(10, random.nextInt);
            for (int i = 0; i < 10; i++)
            {
                var index = r1.nextIndex();
            }


            int[] index_array = new int[]
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
            TezRandomIndex r2 = new TezRandomIndex(index_array, random.nextInt);
            for (int i = 0; i < 10; i++)
            {
                var index = r2.nextIndex();
            }
        }
    }
}

