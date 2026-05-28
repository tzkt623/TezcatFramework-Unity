using System;

namespace tezcat.Framework.Test
{
    public class TestDB : TezBaseTest
    {
        public TestDB() : base("ProtoDB")
        {

        }

        public override void init()
        {

        }

        public override void run()
        {
            TezcatFramework.protoDB.debug(
                (data) =>
                {
                    Console.WriteLine($"Proto Name: {data.protoInfo.NID}");
                    Console.WriteLine($"ProtoID [TID: {data.protoInfo.itemID.TID}] [IID: {data.protoInfo.itemID.IID}] [RID: {data.protoInfo.itemID.RedefineID}]");
                    Console.WriteLine($"RefCount: {data.protoInfo.refCount}");
                },
                () => 
                { 
                    Console.WriteLine("Type"); 
                },
                ()=>
                {
                    Console.WriteLine();
                });
        }

        protected override void onClose()
        {

        }
    }
}