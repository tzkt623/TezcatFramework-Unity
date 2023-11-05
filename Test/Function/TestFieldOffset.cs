using System;
using System.Runtime.InteropServices;

namespace tezcat.Framework.Test
{
    class TestFieldOffset : TezBaseTest
    {
        [StructLayout(LayoutKind.Explicit)]
        class Field
        {
            const long Empty = (-1L << 32 | -1L);

            [FieldOffset(0)]
            public long mItemID = 4234423533424;

            /// <summary>
            /// 数据库ID
            /// </summary>
            [FieldOffset(0)]
            public int mDBID = 20;

            /// <summary>
            /// 重定义ID
            /// 用于当物品被修改时用于确定物品是否相同
            /// </summary>
            [FieldOffset(4)]
            public int mModifiedID = 30;
        }

        Field mField = null;

        public TestFieldOffset() : base("FieldOffset")
        {

        }

        public override void run()
        {
            Console.WriteLine("ItemID = -1");
            mField.mItemID = -1;
            Console.WriteLine($"DBID:{mField.mDBID}");
            Console.WriteLine($"ModifiedID:{mField.mModifiedID}");
            Console.WriteLine($"ItemID:{mField.mItemID}");

            Console.WriteLine("");

            Console.WriteLine("DBID = 1, ModifiedID = 1");
            mField.mDBID = 1;
            mField.mModifiedID = 1;
            Console.WriteLine($"DBID:{mField.mDBID}");
            Console.WriteLine($"ModifiedID:{mField.mModifiedID}");
            Console.WriteLine($"ItemID:{mField.mItemID}");
            Console.WriteLine($"##UInt32.MaxValue:{UInt32.MaxValue}##");
        }

        public override void init()
        {
            mField = new Field();
        }

        public override void close()
        {
            mField = null;
        }
    }
}
