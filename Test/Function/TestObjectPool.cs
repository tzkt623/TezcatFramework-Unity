using System;
using System.Collections.Generic;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Test
{
    public abstract class MyBasePoolItem : ITezObjectPoolItem
    {
        ITezObjectPool ITezObjectPoolItem.objectPool { get; set; }

        public int index;

        void ITezObjectPoolItem.onDestroyThis()
        {

        }

        bool ITezObjectPoolItem.tryRecycleThis()
        {
            return true;
        }
    }

    public class MyPoolItem1 : MyBasePoolItem { }
    public class MyPoolItem2 : MyBasePoolItem { }
    public class MyPoolItem3 : MyBasePoolItem { }

    public class TestObjectPool : TezBaseTest
    {
        public TestObjectPool() : base("ObjectPool")
        {

        }

        public override void init()
        {
            TezObjectPool.evtPoolCreated += onPoolCreated;
            TezObjectPool.evtPoolDestroyed += onPoolDestroyed;
        }

        private void onPoolDestroyed(ITezObjectPool pool, int percent)
        {
            Console.WriteLine($"Pool Destroyed : {pool.poolName}/{percent}%");
        }

        private void onPoolCreated(ITezObjectPool pool)
        {
            Console.WriteLine($"Pool Created : {pool.poolName}/{pool.poolType}");
        }

        public override void run()
        {
            List<MyBasePoolItem> list = new List<MyBasePoolItem>();

            for (int i = 0; i < 100; i++)
            {
                var item1 = TezObjectPool.create<MyPoolItem1>();
                item1.index = i;
                list.Add(item1);

                var item2 = TezObjectPool.create<MyPoolItem2>();
                item2.index = i;
                list.Add(item2);

                var item3 = TezObjectPool.create<MyPoolItem3>();
                item3.index = i;
                list.Add(item3);
            }

            foreach (var item in list)
            {
                item.recycleToPool();
                //TezObjectPool.recycle<MyBasePoolItem>(item);
            }

            this.showFreeCount();

            TezObjectPool.destroyAllPool(36);

            this.showFreeCount();

            TezObjectPool.destroyAllPool();

            this.showFreeCount();
        }

        private void showFreeCount()
        {
            Console.WriteLine("===============Free Count Begin===============");
            Console.WriteLine($"Item1 FreeCount : {TezObjectPool.freeCount<MyPoolItem1>()}");
            Console.WriteLine($"Item2 FreeCount : {TezObjectPool.freeCount<MyPoolItem2>()}");
            Console.WriteLine($"Item3 FreeCount : {TezObjectPool.freeCount<MyPoolItem3>()}");
            Console.WriteLine("===============Free Count End===============");
        }

        protected override void onClose()
        {
            TezObjectPool.evtPoolCreated -= onPoolCreated;
        }
    }
}