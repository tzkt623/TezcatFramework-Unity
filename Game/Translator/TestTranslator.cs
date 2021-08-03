using System.Collections.Generic;

namespace tezcat.Framework.Game
{
    public class TestTranslator
    {
        public void init()
        {
            TezTranslator.register("English");
            TezTranslator.register("Chinese");
            TezTranslator.register("German");
            TezTranslator.register("Japanese");
        }

        #region 多文本单独翻译模式/Translate All In Multi-Files
        public Dictionary<string, string> getLanguageFromYourDatabase(string language)
        {
            return null;
        }

        private void loadSingleText(TezTranslator translator, string language)
        {
            TezTranslator.beginLoad(language);

            foreach (var item in this.getLanguageFromYourDatabase(language))
            {
                translator.loadSingle(item.Key, item.Value);
            }

            TezTranslator.endLoad(language);
        }

        public void load1()
        {
            this.loadSingleText(TestTranslatorCenter.tranName, "English");
            this.loadSingleText(TestTranslatorCenter.tranDescription, "English");
            this.loadSingleText(TestTranslatorCenter.tranStory, "English");
            this.loadSingleText(TestTranslatorCenter.tranExtra, "English");

            this.loadSingleText(TestTranslatorCenter.tranName, "Chinese");
            this.loadSingleText(TestTranslatorCenter.tranDescription, "Chinese");
            this.loadSingleText(TestTranslatorCenter.tranStory, "Chinese");
            this.loadSingleText(TestTranslatorCenter.tranExtra, "Chinese");

            this.loadSingleText(TestTranslatorCenter.tranName, "German");
            this.loadSingleText(TestTranslatorCenter.tranDescription, "German");
            this.loadSingleText(TestTranslatorCenter.tranStory, "German");
            this.loadSingleText(TestTranslatorCenter.tranExtra, "German");

            this.loadSingleText(TestTranslatorCenter.tranName, "Japanese");
            this.loadSingleText(TestTranslatorCenter.tranDescription, "Japanese");
            this.loadSingleText(TestTranslatorCenter.tranStory, "Japanese");
            this.loadSingleText(TestTranslatorCenter.tranExtra, "Japanese");

            ///Set Language
            TezTranslator.switchLanguage("Chinese");
        }
        #endregion

        #region 单文本翻译所有语言模式/Translate All In One File
        public Dictionary<string, string[]> getAllFromYourDatabase()
        {
            return null;
        }

        private void loadAllText(TezTranslator translator)
        {
            foreach (var item in this.getAllFromYourDatabase())
            {
                translator.loadAll(item.Key, item.Value);
            }
        }

        public void load2()
        {
            this.loadAllText(TestTranslatorCenter.tranName);
            this.loadAllText(TestTranslatorCenter.tranDescription);
            this.loadAllText(TestTranslatorCenter.tranStory);
            this.loadAllText(TestTranslatorCenter.tranExtra);

            ///Set Language
            TezTranslator.switchLanguage("Chinese");
        }
        #endregion

        public void test()
        {
            var n1 = TestTranslatorCenter.tranName.translate("$Name1");
            var n2 = TestTranslatorCenter.tranName.translate("$Name2");
            var n3 = TestTranslatorCenter.tranName.translate("$Name3");

            var d1 = TestTranslatorCenter.tranDescription.translate("$Description1");
            var d2 = TestTranslatorCenter.tranDescription.translate("$Description2");
            var d3 = TestTranslatorCenter.tranDescription.translate("$Description3");
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

