using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

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
            Path.init();
            MyCategory.init();
            MyDescriptorConfig.init();
            //TezcatFramework.set(new TezProtoDatabase());
            //TezcatFramework.set(new TezRunTimeDatabase());

            //A
            mTestList.Add(new TestAStarSystem());
            //B
            mTestList.Add(new TestBehaviorTree());
            mTestList.Add(new TestBonusSystem2());
            //C
            mTestList.Add(new TestCategoryGenerator());
            //D
            //E
            //F
            mTestList.Add(new TestFieldOffset());
            //G
            mTestList.Add(new TestGameMachine());
            mTestList.Add(new TestGameState());
            //H
            mTestList.Add(new TestHexSystem());
            //I
            mTestList.Add(new TestInventory());
            //J
            //K
            //L
            mTestList.Add(new TestLifeMonitor());
            mTestList.Add(new TestLinkedList());
            //M
            //N
            //O
            mTestList.Add(new TestObject());
            mTestList.Add(new TestObjectPool());
            //P
            //Q
            //R
            mTestList.Add(new TestRandomIndex());
            //S
            mTestList.Add(new TestSystemAttribute());
            mTestList.Add(new TestSaveController());
            mTestList.Add(new TestSignalSystem());
            //T
            mTestList.Add(new TestTag());
            mTestList.Add(new TestTranslator());
            mTestList.Add(new TestTriggerListSystem());
            mTestList.Add(new TestTask());
            mTestList.Add(new TestTaskAsync());
            //U
            //V
            mTestList.Add(new TestValueDescriptor());
            mTestList.Add(new TestValueArrayManager());
            //W
            //X
            //Y
            //Z

            //mTestList.Add(new TestBonusSystem());
        }

        void register()
        {
            TezcatFramework.classFactory.register<Axe>();
            TezcatFramework.classFactory.register<Gun>();

            TezcatFramework.classFactory.register<Helmet>();
            TezcatFramework.classFactory.register<Breastplate>();
            TezcatFramework.classFactory.register<Leg>();

            TezcatFramework.classFactory.register<MagicPotion>();
            TezcatFramework.classFactory.register<HealthPotion>();

            TezcatFramework.classFactory.register<Character>();
            TezcatFramework.classFactory.register<Ship>();


            TezcatFramework.classFactory.register<AxeData>();
            TezcatFramework.classFactory.register<GunData>();

            TezcatFramework.classFactory.register<HelmetData>();
            TezcatFramework.classFactory.register<BreastplateData>();
            TezcatFramework.classFactory.register<LegData>();

            TezcatFramework.classFactory.register<MagicPotionData>();
            TezcatFramework.classFactory.register<HealthPotionData>();

            TezcatFramework.classFactory.register<CharacterData>();
            TezcatFramework.classFactory.register<ShipData>();
        }

        public void run()
        {
            this.init();
            this.register();

            new TestProtoDB().run();

            while (true)
            {
                var index = this.choose();
                if (index >= 0 && index < mTestList.Count)
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
                Console.WriteLine(e.Message);
                index = -1;
            }

            return index;
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