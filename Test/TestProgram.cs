using System;
using System.Collections.Generic;

namespace tezcat.Framework.Test
{
    class FunctionTest
    {
        List<TezBaseTest> mTestList = new List<TezBaseTest>();
        TezBaseTest mCurrent = null;

        public void run()
        {
            this.register();
            Console.WriteLine("Choose Test ==============>");
            for (int i = 0; i < mTestList.Count; i++)
            {
                Console.WriteLine(mTestList[i].name);
            }
            Console.WriteLine("==========================>");
            Console.Write("Number:");
            string index_str = Console.ReadLine();
            int index = int.Parse(index_str);
            mCurrent = mTestList[index];

            mCurrent.run();
        }

        void register()
        {
            mTestList.Add(new TestInventory());
            mTestList.Add(new TestFieldOffset());
        }
    }


    class TestProgram
    {
        static void Main()
        {
            FunctionTest mTest = new FunctionTest();
            mTest.run();
        }
    }
}
