using System;
using tezcat.Framework.Core;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    class Context : ITezBTContext
    {
        public int foods;
        public int tools;
        public int rice;
        public int dish;

        void ITezCloseable.closeThis()
        {

        }
    }

    [TezBTRegister(name = "NeedFoods")]
    class NeedFoods : TezBTCondition
    {
        protected override void onExecute()
        {
            var context = this.tree.getContext<Context>();
            if (context.foods < 5)
            {
                Console.WriteLine("需要食物!");
                this.setSuccess();
            }
            else
            {
                Console.WriteLine("不需要食物!");
                this.setFail();
            }
        }

        public override void init()
        {
        }
    }

    [TezBTRegister(name = "NeedTools")]
    class NeedTools : TezBTCondition
    {
        protected override void onExecute()
        {
            var context = this.tree.getContext<Context>();
            if (context.tools < 5)
            {
                Console.WriteLine("需要厨具!");
                this.setSuccess();
            }
            else
            {
                Console.WriteLine("不需要厨具!");
                this.setFail();
            }
        }

        public override void init()
        {
        }
    }

    [TezBTRegister(name = "GetFoods")]
    class GetFoods : TezBTAction
    {
        public override void init()
        {

        }

        protected override void onExecute()
        {
            var context = this.tree.getContext<Context>();
            if (context.foods < 5)
            {
                context.foods += 1;
                Console.WriteLine($"取得食物{context.foods}");
            }
            else
            {
                Console.WriteLine("食物获取完毕!");
                this.setSuccess();
            }
        }
    }

    [TezBTRegister(name = "GetTools")]
    class GetTools : TezBTAction
    {
        protected override void onExecute()
        {
            var context = this.tree.getContext<Context>();
            if (context.tools < 5)
            {
                context.tools += 1;
                Console.WriteLine($"取得厨具{context.tools}");
            }
            else
            {
                Console.WriteLine("厨具获取完毕!");
                this.setSuccess();
            }
        }

        public override void init()
        {

        }
    }

    [TezBTRegister(name = "CookRice")]
    class CookRice : TezBTAction
    {
        public override void init()
        {

        }

        protected override void onExecute()
        {
            var context = this.tree.getContext<Context>();
            if (context.rice < 1)
            {
                if (context.tools < 1 || context.foods < 1)
                {
                    Console.WriteLine("无法做饭,资源不够");
                    this.setFail();
                    return;
                }

                context.rice += 1;
                context.tools -= 1;
                context.foods -= 1;
                Console.WriteLine($"蒸饭{context.rice}");
            }
            else
            {
                Console.WriteLine("饭蒸好了!");
                this.setSuccess();
            }
        }
    }

    [TezBTRegister(name = "Cooking")]
    class Cooking : TezBTAction
    {
        public override void init()
        {

        }

        protected override void onExecute()
        {
            var context = this.tree.getContext<Context>();
            if (context.dish < 3)
            {
                if (context.tools < 1 || context.foods < 2)
                {
                    Console.WriteLine("无法做菜,资源不够");
                    this.setFail();
                    return;
                }

                context.dish += 1;
                context.tools -= 1;
                context.foods -= 2;
                Console.WriteLine($"做菜{context.dish}");
            }
            else
            {
                Console.WriteLine("菜做好了!");
                this.setSuccess();
            }
        }
    }

    [TezBTRegister(name = "Eating")]
    class Eating : TezBTAction
    {
        public override void init()
        {

        }

        protected override void onExecute()
        {
            var context = this.tree.getContext<Context>();
            if (context.dish == 0 && context.rice == 0)
            {
                Console.WriteLine("饭吃完了");
                this.setSuccess();
                return;
            }

            if (context.dish > 0)
            {
                context.dish -= 1;
                Console.WriteLine($"吃菜{context.dish}");
            }

            if (context.rice > 0)
            {
                context.rice -= 1;
                Console.WriteLine($"吃饭{context.rice}");
            }
        }
    }

    public class TestBehaviorTree : TezBaseTest
    {
        TezBehaviorTree mTree = null;
        bool mBreak = false;

        static TestBehaviorTree()
        {
            TezBehaviorTree.register<NeedFoods>();
            TezBehaviorTree.register<NeedTools>();

            TezBehaviorTree.register<GetFoods>();
            TezBehaviorTree.register<GetTools>();

            TezBehaviorTree.register<CookRice>();
            TezBehaviorTree.register<Cooking>();
            TezBehaviorTree.register<Eating>();
        }

        public TestBehaviorTree() : base("BehaviorTree")
        {

        }

        public void buildTree()
        {


            ///做饭
            ///如果没有食物,先取得食物
            ///如果没有工具,取得工具
            ///先蒸饭,再炒菜,最后吃饭
            ///
            TezJsonReader reader = new TezJsonReader();
            reader.load($"{Path.root}Res/BehaviorTree/Config.json");

            mTree = new TezBehaviorTree();
            mTree.evtBehaviorComplete += onBehaviorComplete;
            mTree.loadConfig(reader);
            reader.close();


            /*
            var root = mTree.createRoot<TezBTSequence>();

            var get_foods = root.createNode<TezBTSequence>();

            get_foods.addChild(TezBehaviorTree.create("NeedFoods"));
            get_foods.addChild(TezBehaviorTree.create("GetFoods"));

            var get_tools = root.createNode<TezBTSequence>();
            get_tools.createNode<NeedTools>();
            get_tools.createNode<GetTools>();

            var cook = root.createNode<TezBTParallel>();
            cook.createNode<CookRice>();
            cook.createNode<Cooking>();

            root.createNode<Eating>();
            */

            mTree.build();
            mTree.setContext(new Context());
        }

        private void onBehaviorComplete(TezBTNode.Result result)
        {
            if(result == TezBTNode.Result.Success)
            {
                mBreak = true;
            }
        }

        protected override void onClose()
        {
            mTree.close();
            mTree = null;
            mBreak = false;
        }

        public override void init()
        {
            this.buildTree();
        }

        public override void run()
        {
            Console.WriteLine("Press Any key to run, Press Esc to exit");
            while (Console.ReadKey(true).Key != ConsoleKey.Escape && !mBreak)
            {
                mTree.execute();
            }
        }
    }
}