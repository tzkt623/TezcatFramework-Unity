using System;
using tezcat.Framework.AI;

namespace tezcat.Framework.Test
{
    class Context : ITezBTContext
    {
        public int foods;
        public int tools;
        public int rice;
        public int dish;

        public void close()
        {

        }
    }

    [TezBTRegister(name = "NeedFoods")]
    class NeedFoods : TezBTCondition
    {
        //         public override void execute()
        //         {
        //             var context = this.tree.getContext<Context>();
        //             if (context.foods < 5)
        //             {
        //                 Console.WriteLine("需要食物!");
        //                 this.reportToParent(Result.Success);
        //             }
        //             else
        //             {
        //                 Console.WriteLine("不需要食物!");
        //                 this.reportToParent(Result.Fail);
        //             }
        //         }

        public override Result imdExecute()
        {
            var context = this.tree.getContext<Context>();
            if (context.foods < 5)
            {
                Console.WriteLine("需要食物!");
                return Result.Success;
            }
            else
            {
                Console.WriteLine("不需要食物!");
                return Result.Fail;
            }
        }

        public override void init()
        {
        }

        public override void reset()
        {
        }
    }

    [TezBTRegister(name = "NeedTools")]
    class NeedTools : TezBTCondition
    {
        //         public override void execute()
        //         {
        //             var context = this.tree.getContext<Context>();
        //             if (context.tools < 5)
        //             {
        //                 Console.WriteLine("需要厨具!");
        //                 this.reportToParent(Result.Success);
        //             }
        //             else
        //             {
        //                 Console.WriteLine("不需要厨具!");
        //                 this.reportToParent(Result.Fail);
        //             }
        //         }

        public override Result imdExecute()
        {
            var context = this.tree.getContext<Context>();
            if (context.tools < 5)
            {
                Console.WriteLine("需要厨具!");
                return Result.Success;
            }
            else
            {
                Console.WriteLine("不需要厨具!");
                return Result.Fail;
            }
        }

        public override void init()
        {
        }

        public override void reset()
        {
        }
    }

    [TezBTRegister(name = "GetFoods")]
    class GetFoods : TezBTAction
    {
        public override void init()
        {

        }

        //         public override void execute()
        //         {
        //             var context = this.tree.getContext<Context>();
        //             if (context.foods < 5)
        //             {
        //                 context.foods += 1;
        //                 Console.WriteLine(string.Format("取得食物{0}", context.foods));
        //                 this.reportToParent(Result.Running);
        //             }
        //             else
        //             {
        //                 Console.WriteLine("食物获取完毕!");
        //                 this.reportToParent(Result.Success);
        //             }
        //         }

        public override Result imdExecute()
        {
            var context = this.tree.getContext<Context>();
            if (context.foods < 5)
            {
                context.foods += 1;
                Console.WriteLine(string.Format("取得食物{0}", context.foods));
                return Result.Running;
            }
            else
            {
                Console.WriteLine("食物获取完毕!");
                return Result.Success;
            }
        }
    }

    [TezBTRegister(name = "GetTools")]
    class GetTools : TezBTAction
    {
        //         public override void execute()
        //         {
        //             var context = this.tree.getContext<Context>();
        //             if (context.tools < 5)
        //             {
        //                 context.tools += 1;
        //                 Console.WriteLine(string.Format("取得厨具{0}", context.tools));
        //                 this.reportToParent(Result.Running);
        //             }
        //             else
        //             {
        //                 Console.WriteLine("厨具获取完毕!");
        //                 this.reportToParent(Result.Success);
        //             }
        //         }

        public override Result imdExecute()
        {
            var context = this.tree.getContext<Context>();
            if (context.tools < 5)
            {
                context.tools += 1;
                Console.WriteLine(string.Format("取得厨具{0}", context.tools));
                return Result.Running;
            }
            else
            {
                Console.WriteLine("厨具获取完毕!");
                return Result.Success;
            }
        }

        public override void init()
        {

        }
    }

    [TezBTRegister(name = "SteamRice")]
    class SteamRice : TezBTAction
    {
        public override void init()
        {

        }

        //         public override void execute()
        //         {
        //             var context = this.tree.getContext<Context>();
        //             if (context.rice < 1)
        //             {
        //                 if (context.tools < 1 || context.foods < 1)
        //                 {
        //                     Console.WriteLine("无法做饭,资源不够");
        //                     this.reportToParent(Result.Fail);
        //                     return;
        //                 }
        // 
        //                 context.rice += 1;
        //                 context.tools -= 1;
        //                 context.foods -= 1;
        //                 Console.WriteLine(string.Format("蒸饭{0}", context.rice));
        //                 this.reportToParent(Result.Running);
        //             }
        //             else
        //             {
        //                 Console.WriteLine("饭蒸好了!");
        //                 this.reportToParent(Result.Success);
        //             }
        //         }

        public override Result imdExecute()
        {
            var context = this.tree.getContext<Context>();
            if (context.rice < 1)
            {
                if (context.tools < 1 || context.foods < 1)
                {
                    Console.WriteLine("无法做饭,资源不够");
                    return Result.Fail;
                }

                context.rice += 1;
                context.tools -= 1;
                context.foods -= 1;
                Console.WriteLine(string.Format("蒸饭{0}", context.rice));
                return Result.Running;
            }
            else
            {
                Console.WriteLine("饭蒸好了!");
                return Result.Success;
            }
        }
    }

    [TezBTRegister(name = "Cooking")]
    class Cooking : TezBTAction
    {
        public override void init()
        {

        }

        //         public override void execute()
        //         {
        //             var context = this.tree.getContext<Context>();
        //             if (context.tools < 1 || context.foods < 2)
        //             {
        //                 Console.WriteLine("无法做菜,资源不够");
        //                 this.reportToParent(Result.Fail);
        //                 return;
        //             }
        // 
        //             if (context.dish < 3)
        //             {
        //                 context.dish += 1;
        //                 context.tools -= 1;
        //                 context.foods -= 2;
        //                 Console.WriteLine(string.Format("做菜{0}", context.dish));
        //                 this.reportToParent(Result.Running);
        //             }
        //             else
        //             {
        //                 Console.WriteLine("菜做好了!");
        //                 this.reportToParent(Result.Success);
        //             }
        //         }

        public override Result imdExecute()
        {
            var context = this.tree.getContext<Context>();
            if (context.dish < 3)
            {
                if (context.tools < 1 || context.foods < 2)
                {
                    Console.WriteLine("无法做菜,资源不够");
                    return Result.Fail;
                }

                context.dish += 1;
                context.tools -= 1;
                context.foods -= 2;
                Console.WriteLine(string.Format("做菜{0}", context.dish));
                return Result.Running;
            }
            else
            {
                Console.WriteLine("菜做好了!");
                return Result.Success;
            }
        }
    }

    [TezBTRegister(name = "Eating")]
    class Eating : TezBTAction
    {
        public override void init()
        {

        }

        //         public override void execute()
        //         {
        //             var context = this.tree.getContext<Context>();
        //             if (context.dish == 0 && context.rice == 0)
        //             {
        //                 Console.WriteLine("饭吃完了");
        //                 this.reportToParent(Result.Success);
        //                 return;
        //             }
        // 
        //             if (context.dish > 0)
        //             {
        //                 context.dish -= 1;
        //                 Console.WriteLine("吃菜");
        //             }
        // 
        //             if (context.rice > 0)
        //             {
        //                 context.rice -= 1;
        //                 Console.WriteLine("吃饭");
        //             }
        // 
        //             this.reportToParent(Result.Running);
        //         }

        public override Result imdExecute()
        {
            var context = this.tree.getContext<Context>();
            if (context.dish == 0 && context.rice == 0)
            {
                Console.WriteLine("饭吃完了");
                return Result.Success;
            }

            if (context.dish > 0)
            {
                context.dish -= 1;
                Console.WriteLine(string.Format("吃菜{0}", context.dish));
            }

            if (context.rice > 0)
            {
                context.rice -= 1;
                Console.WriteLine(string.Format("吃饭{0}", context.rice));
            }

            return Result.Running;
        }
    }

    public class TestBehaviorTree : TezBaseTest
    {
        TezBehaviorTree mTree = new TezBehaviorTree();

        public TestBehaviorTree() : base("BehaviorTree")
        {
            TezBehaviorTree.register<NeedFoods>();
            TezBehaviorTree.register<NeedTools>();

            TezBehaviorTree.register<GetFoods>();
            TezBehaviorTree.register<SteamRice>();
            TezBehaviorTree.register<Cooking>();
            TezBehaviorTree.register<Eating>();

            this.buildTree();
        }

        public void buildTree()
        {
            ///做饭
            ///如果没有食物,先取得食物
            ///如果没有工具,取得工具,
            ///先蒸饭,再炒菜,最后吃饭
            var root = mTree.createRoot<TezBTSequence>();

            var get_foods = root.createNode<TezBTSequence>();

            get_foods.addNode(TezBehaviorTree.create("NeedFoods"));
            get_foods.addNode(TezBehaviorTree.create("GetFoods"));

            var get_tools = root.createNode<TezBTSequence>();
            get_tools.createNode<NeedTools>();
            get_tools.createNode<GetTools>();

            var cook = root.createNode<TezBTParallel>();
            cook.createNode<SteamRice>();
            cook.createNode<Cooking>();

            root.createNode<Eating>();

            mTree.init();
            mTree.setContext(new Context());
        }


        public override void run()
        {
            Console.WriteLine("Press Any key to run, Press SpaceBar to exit");
            while (Console.ReadKey(true).Key != ConsoleKey.Spacebar)
            {
                mTree.imdExecute();
            }
        }
    }
}

