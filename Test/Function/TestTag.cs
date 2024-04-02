using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Test
{
    class TestTag : TezBaseTest
    {
        public static readonly int Tag1 = TezTagManager.register("Tag1");
        public static readonly int Tag2 = TezTagManager.register("Tag2");
        public static readonly int Tag3 = TezTagManager.register("Tag3");
        public static readonly int Tag4 = TezTagManager.register("Tag4");
        public static readonly int Tag5 = TezTagManager.register("Tag5");
        public static readonly int Tag6 = TezTagManager.register("Tag6");

        TezTags mTags = new TezTags();

        public TestTag() : base("Tags")
        {
        }

        protected override void onClose()
        {
            mTags.close();
        }

        public override void init()
        {
            mTags = new TezTags();
        }

        public override void run()
        {
            mTags.add(Tag1);
            mTags.add(Tag2);
            mTags.add(Tag4);
            mTags.add(Tag5);

            Console.WriteLine("Current Tags:");
            mTags.foreachTag((int id) =>
            {
                Console.Write(TezTagManager.get(id).name);
                Console.Write(",");
            });
            Console.Write("\n\n");

            Console.WriteLine($"OneOf\t[Tag1]: {mTags.oneOf(Tag1)}");
            Console.WriteLine($"OneOf\t[Tag6]: {mTags.oneOf(Tag6)}");

            Console.WriteLine($"NoneOf\t[Tag4]: {mTags.noneOf(Tag4)}");
            Console.WriteLine($"NoneOf\t[Tag6]: {mTags.noneOf(Tag6)}");

            Console.WriteLine($"AnyOf\t[Tag1,Tag2,Tag3]: {mTags.anyOf(new int[] { Tag1, Tag2, Tag3 })}");
            Console.WriteLine($"AnyOf\t[Tag3,Tag6]: {mTags.anyOf(new int[] { Tag3, Tag6})}");

            Console.WriteLine($"NoneOf\t[Tag1,Tag2,Tag3]: {mTags.noneOf(new int[] { Tag1, Tag2, Tag3 })}");
            Console.WriteLine($"NoneOf\t[Tag3,Tag6]: {mTags.noneOf(new int[] { Tag3, Tag6 })}");


            Console.WriteLine($"AllOf\t[Tag2,Tag4,Tag5]: {mTags.allOf(new int[] { Tag2, Tag4, Tag5 })}");
            Console.WriteLine($"AllOf\t[Tag3,Tag2,Tag4]: {mTags.allOf(new int[] { Tag3, Tag2, Tag4 })}");


            Console.WriteLine($"EqualOf\t[Tag1,Tag2,Tag4,Tag5]: {mTags.equalOf(new int[] { Tag1, Tag2, Tag4, Tag5 })}");
            Console.WriteLine($"EqualOf\t[Tag2,Tag4,Tag5]: {mTags.equalOf(new int[] { Tag2, Tag4, Tag5 })}");
        }
    }
}