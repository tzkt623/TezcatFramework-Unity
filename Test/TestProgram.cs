﻿using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;

namespace tezcat.Framework.Test
{
    class Path
    {
        public static string root { get; private set; }

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
            TezcatFramework.mainDB.load(Path.root + "Res/Item");
        }

        public void run()
        {
            this.init();
            this.register();
            this.loadRes();

            while (true)
            {
                var index = this.choose();
                if (index != -1)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine($"<=== Enter {index}.{mTestList[index].name} Test ===>");
                    mTestList[index].init();
                    mTestList[index].run();
                    mTestList[index].close();
                }

                Console.ResetColor();
                Console.WriteLine("");
                Console.WriteLine("Press Any Key to Continue");
                Console.ReadKey();
            }
        }

        int choose()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("<======== Choose Test ========>");
            for (int i = 0; i < mTestList.Count; i++)
            {
                Console.WriteLine($"{i}.{mTestList[i].name}");
            }
            Console.Write("Number:");
            string index_str = Console.ReadLine();

            int index;
            try
            {
                index = int.Parse(index_str);
            }
            catch (Exception e)
            {
                index = -1;
            }

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
            mTestList.Add(new TestAStarSystem());
            mTestList.Add(new TestTag());
            mTestList.Add(new TestRandomIndex());
            mTestList.Add(new TestFieldOffset());
            mTestList.Add(new TestBehaviorTree());
            mTestList.Add(new TestLifeMonitor());
            mTestList.Add(new TestGameMachine());
            mTestList.Add(new TestInventory());
            mTestList.Add(new TestTranslator());
            mTestList.Add(new TestSystemAttribute());
            mTestList.Add(new TestBonusSystem());
            mTestList.Add(new TestHexSystem());
            mTestList.Add(new TestCategoryGenerator());
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