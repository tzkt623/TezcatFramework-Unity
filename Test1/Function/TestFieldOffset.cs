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
            public int m0_3 = 20;

            /// <summary>
            /// 重定义ID
            /// 用于当物品被修改时用于确定物品是否相同
            /// </summary>
            [FieldOffset(4)]
            public int m4_7 = 30;
        }

        Field mField = null;

        public TestFieldOffset() : base("FieldOffset")
        {

        }

        public override void run()
        {
            void show()
            {
                Console.WriteLine($"0-3:{mField.m0_3}");
                Console.WriteLine($"4-7:{mField.m4_7}");
                Console.WriteLine($"ID:{mField.mItemID}");
                Console.WriteLine("");
            }
            Console.WriteLine($"##UInt32.MaxValue:{UInt32.MaxValue}##");

            mField.mItemID = -1;
            show();

            mField.m0_3 = 1;
            mField.m4_7 = 0;
            show();

            mField.m0_3 = 0;
            mField.m4_7 = 1;
            show();
        }

        public override void init()
        {
            mField = new Field();
        }

        protected override void onClose()
        {
            mField = null;
        }
    }
}
