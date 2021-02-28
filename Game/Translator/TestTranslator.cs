namespace tezcat.Framework.Game
{
    public class TestTranslator
    {
        public void init()
        {
//            TezTranslator.dataCapacity = 4;
        }

        public void load()
        {
            TestTranslatorCenter.tranName.add("$A1", "HAHA1");
            TestTranslatorCenter.tranName.add("$A2", "HAHA2");
            TestTranslatorCenter.tranName.add("$A3", "HAHA3");

            TestTranslatorCenter.tranDescription.add("$A1", "HAHA1`s Description");
            TestTranslatorCenter.tranDescription.add("$A2", "HAHA2`s Description");
            TestTranslatorCenter.tranDescription.add("$A3", "HAHA3`s Description");

            TestTranslatorCenter.tranStory.add("$A1", "HAHA1`s Story");
            TestTranslatorCenter.tranStory.add("$A2", "HAHA2`s Story");
            TestTranslatorCenter.tranStory.add("$A3", "HAHA3`s Story");

            TestTranslatorCenter.tranExtra.add("$A1", "HAHA1`s Extra");
            TestTranslatorCenter.tranExtra.add("$A2", "HAHA2`s Extra");
            TestTranslatorCenter.tranExtra.add("$A3", "HAHA3`s Extra");
        }

        public void test()
        {
            var an1 = TestTranslatorCenter.tranName.translate("$A1");
            var an2 = TestTranslatorCenter.tranName.translate("$A2");
            var an3 = TestTranslatorCenter.tranName.translate("$A3");

            var ad1 = TestTranslatorCenter.tranDescription.translate("$A1");
            var ad2 = TestTranslatorCenter.tranDescription.translate("$A2");
            var ad3 = TestTranslatorCenter.tranDescription.translate("$A3");
        }
    }

    public class TestTranslatorCenter
    {
        public static TezTranslator tranName = new TezTranslator();
        public static TezTranslator tranDescription = new TezTranslator();
        public static TezTranslator tranStory = new TezTranslator();
        public static TezTranslator tranExtra = new TezTranslator();
    }
}

