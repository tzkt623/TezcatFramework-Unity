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

        Field mField = new Field();

        public TestFieldOffset() : base("FieldOffset")
        {

        }

        public override void run()
        {
            //             mItemID = (1L << 32) | 2L;
            // 
            //             mDBID = -1;
            //             mModifiedID = -1;


            Console.WriteLine(mField.mItemID);
            Console.WriteLine(mField.mDBID);
            Console.WriteLine(mField.mModifiedID);
//            Console.WriteLine((1L << 32) | 1L);
        }

        public override void init()
        {
            mField.mItemID = -1;
        }

        public override void close()
        {
            mField = null;
        }
    }
}
