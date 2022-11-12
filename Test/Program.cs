using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mu;

namespace tezcat.Framework.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                //              TestAwaitable testAwaitable = new TestAwaitable();
                //              testAwaitable.test();
                //              Console.ReadKey();
            }

            {
                //                 TestBehaviorTree app = new TestBehaviorTree();
                //                 app.buildTree();
                //                 app.run();
            }

            {
                //              TestSystemAttribute test = new TestSystemAttribute();
                //              test.run();
            }

            {
                TestProperty app = new TestProperty();
                app.run();
            }

            {
                //                 Rebuild re = new Rebuild();
                //                 re.run();
            }
        }
    }
}
