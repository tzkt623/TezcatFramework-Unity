using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tezcat.Framework.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            {
//             TestAwaitable testAwaitable = new TestAwaitable();
//             testAwaitable.test();
//             Console.ReadKey();
            }

            {
                TestBehaviorTree behaviorTree = new TestBehaviorTree();
                behaviorTree.buildTree();
                behaviorTree.run();
            }

            {
//                 TestSystemAttribute test = new TestSystemAttribute();
//                 test.run();
            }

        }
    }
}
