using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.Test
{
    class Path
    {
        public static string root;

        public static void init()
        {
            var path = TezFilePath.getProjectPath();
            path = TezFilePath.cleanPath(path);

            var pos = path.IndexOf("bin");
            root = path.Remove(pos);
        }
    }

    class FunctionTest
    {
        List<TezBaseTest> mTestList = new List<TezBaseTest>();

        void init()
        {
            MyCategory.init();
            Path.init();
            TezcatFramework.set(new TezFixedDatabase());
            TezcatFramework.set(new TezRunTimeDatabase());
        }

        void loadRes()
        {
            TezcatFramework.classFactory.register(() => new Armor());
            TezcatFramework.classFactory.register(() => new Character());
            TezcatFramework.classFactory.register(() => new HealthPotion());
            TezcatFramework.mainDB.load(Path.root + "Res");
        }

        public void run()
        {
            this.init();
            this.register();
            this.loadRes();


            Console.WriteLine("<======== Choose Test ========>");
            for (int i = 0; i < mTestList.Count; i++)
            {
                Console.WriteLine($"{i}.{mTestList[i].name}");
            }

            while (true)
            {
                int index = this.choose();
                if (index != -1)
                {
                    mTestList[index].run();
                }
            }
        }

        int choose()
        {
            Console.WriteLine("<=============================>");
            Console.Write("Number:");
            string index_str = Console.ReadLine();
            int index = int.Parse(index_str);

            if (index < mTestList.Count)
            {
                return index;
            }
            else
            {
                return -1;
            }
        }

        void register()
        {
            mTestList.Add(new TestInventory());
            mTestList.Add(new TestFieldOffset());
            mTestList.Add(new TestCategory());
            mTestList.Add(new TestBehaviorTree());
            mTestList.Add(new TestLifeMonitor());
            mTestList.Add(new TestGameMachine());
            mTestList.Add(new TestTranslator());
            mTestList.Add(new TestSystemAttribute());
            mTestList.Add(new TestBonusSystem());
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