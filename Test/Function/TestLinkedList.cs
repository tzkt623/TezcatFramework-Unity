using System;
using tezcat.Framework.Game;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Test
{
    public class TestLinkedList : TezBaseTest
    {
        public TestLinkedList() : base("LinkedList")
        {

        }

        public override void init()
        {
            //TezTrigger trigger1 = new TezTrigger();
            //trigger1.addPhase(/*do something*/);
            //trigger1.addPhase(/*do something*/);
            //trigger1.addPhase(/*do something*/);
            //trigger1.addPhase(/*do something*/);
            //trigger1.run();

            TezLinkedList<int> list = new TezLinkedList<int>();

            list.addBack(1);
            list.addBack(2);
            var node = list.addBack(3);
            list.addBack(4);

            list.remove(2);
            list.remove(4);
            list.remove(1);
            list.removeAt(node);

            list.foreachList((int x) =>
            {
                Console.Write(x);
                Console.Write(" ");
            });
            Console.WriteLine();

            list.addBack(7);
            list.addBack(8);
            list.addBack(9);
            list.addFront(1);
            list.addFront(2);
            list.addFront(3);

            list.foreachList((int x) =>
            {
                Console.Write(x);
                Console.Write(" ");
            });
            Console.WriteLine();

            list.foreachList((ITezLinkedListNode<int> x) =>
            {
                if (x.value == 1)
                {
                    x.removeSlef();
                }
            });

            list.foreachList((ITezLinkedListNode<int> x) =>
            {
                Console.Write(x.value);
                Console.Write(" ");
            });
            Console.WriteLine();

            list.foreachList((ITezLinkedListNode<int> x) =>
            {
                x.removeSlef();
            });

            list.addFront(1);
            list.addFront(2);
            list.addFront(3);
            list.addFront(4);

            list.foreachList((ITezLinkedListNode<int> x) =>
            {
                Console.Write(x.value);
                Console.Write(" ");
            });
            Console.WriteLine();

            list.popFront();
            list.foreachList((ITezLinkedListNode<int> x) =>
            {
                Console.Write(x.value);
                Console.Write(" ");
            });
            Console.WriteLine();

            list.popBack();
            list.foreachList((ITezLinkedListNode<int> x) =>
            {
                Console.Write(x.value);
                Console.Write(" ");
            });
            Console.WriteLine();

            list.clear();
            list.addFront(200);
            list.addFront(300);
            list.addFront(400);
            list.foreachList((ITezLinkedListNode<int> x) =>
            {
                Console.Write(x.value);
                Console.Write(" ");
            });
            Console.WriteLine();
        }

        public override void run()
        {
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                TezTriggerSystem.update();
            }
        }

        protected override void onClose()
        {

        }
    }
}
