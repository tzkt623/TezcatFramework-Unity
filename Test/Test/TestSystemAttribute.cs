using System;

namespace tezcat.Framework.Test
{

    class MyFlagAttribute : Attribute
    {
        public string name;
    }

    class MyIndexAttribute : Attribute
    {
        static int ID = 0;

        public int index = ID++;
    }


    [MyFlag(name = "Class1")]
    [MyIndex()]
    class MyClass1
    {

    }

    [MyFlag(name = "Class2")]
    [MyIndex()]
    class MyClass2
    {

    }

    public class TestSystemAttribute
    {
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

            Console.WriteLine(((MyFlagAttribute)objs[0]).name);
            Console.WriteLine(((MyIndexAttribute)objs[1]).index);
        }

        public void run()
        {
            this.check<MyClass1>();
            this.check<MyClass2>();

            Console.ReadKey();
        }
    }
}