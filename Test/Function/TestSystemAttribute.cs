using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Test
{
    class MyNameAttribute : Attribute
    {
        public string name;
    }

    class MyIndexAttribute : Attribute
    {
        static int ID = 0;
        public int index = ID++;
    }

    [MyName(name = "Class1")]
    [MyIndex]
    class MyClass1
    {

    }

    [MyName(name = "Class2")]
    [MyIndex]
    class MyClass2
    {

    }

    public class TestSystemAttribute : TezBaseTest
    {
        public TestSystemAttribute() : base("SystemAttribute")
        {

        }

        public void check<T>()
        {
            var objs = typeof(T).GetCustomAttributes(true);
            if (objs.Length == 0)
            {
                return;
            }

            foreach (var item in objs)
            {
                Console.WriteLine(item);
            }

            Console.WriteLine(((MyNameAttribute)objs[0]).name);
            Console.WriteLine(((MyIndexAttribute)objs[1]).index);
            Console.WriteLine();
        }

        protected override void onClose()
        {
        }

        public override void init()
        {
        }

        public override void run()
        {
            this.check<MyClass1>();
            this.check<MyClass2>();
        }
    }
}