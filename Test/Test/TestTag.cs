using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tezcat.Framework.Core;

namespace tezcat.Framework.Test
{
    class TestTag
    {
        public static readonly int Tag1 = TezTagGenerator.register("Tag1");
        public static readonly int Tag2 = TezTagGenerator.register("Tag2");
        public static readonly int Tag3 = TezTagGenerator.register("Tag3");
        public static readonly int Tag4 = TezTagGenerator.register("Tag4");
        public static readonly int Tag5 = TezTagGenerator.register("Tag5");

        TezTags manager = new TezTags();

        public void test()
        {
            manager.add(Tag1);
            manager.add(Tag2);
            manager.add(Tag3);
            manager.add(Tag4);
            manager.add(Tag5);


            manager.oneOf(Tag1);
            manager.noneOf(Tag4);
            manager.anyOf(new int[] { Tag1, Tag2, Tag3 });
            manager.noneOf(new int[] { Tag1, Tag2, Tag3 });
            manager.allOf(new int[] { Tag2, Tag3, Tag4 });
        }
    }
}