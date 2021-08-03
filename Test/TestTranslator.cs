using System.Collections.Generic;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    public class TestTranslator_AllInOne
    {
        public void init()
        {
            TezTranslator.setMode(TezTranslationSlot.Category.AllInOne);
            TezTranslator.eventLanguageSwitched += eventLanguageSwitched;

            TezTranslator.register("English");
            TezTranslator.register("Chinese");
            TezTranslator.register("German");
            TezTranslator.register("Japanese");
        }

        private void eventLanguageSwitched()
        {
            this.refreshText();
        }

        private void refreshText()
        {

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
            this.init();

            this.load1();
//            this.load2();

            var n1 = TestTranslatorCenter.tranName.translate("$Name1");
            var n2 = TestTranslatorCenter.tranName.translate("$Name2");
            var n3 = TestTranslatorCenter.tranName.translate("$Name3");

            var d1 = TestTranslatorCenter.tranDescription.translate("$Description1");
            var d2 = TestTranslatorCenter.tranDescription.translate("$Description2");
            var d3 = TestTranslatorCenter.tranDescription.translate("$Description3");
        }
    }

    /// <summary>
    /// 针对大文本量
    /// </summary>
    public class TestTranslator_Single
    {
        public void init()
        {
            TezTranslator.setMode(TezTranslationSlot.Category.Single);

            TezTranslator.register("English", "X:/Translation/English.trans");
            TezTranslator.register("Chinese", "X:/Translation/Chinese.trans");
            TezTranslator.register("German", "X:/Translation/German.trans");
            TezTranslator.register("Japanese", "X:/Translation/Japanese.trans");

            TezTranslator.eventLanguageReload += onEventLanguageReload;
            TezTranslator.eventLanguageSwitched += eventLanguageSwitched;
        }

        private void eventLanguageSwitched()
        {
            this.refreshText();
        }

        private void refreshText()
        {

        }

        private void onEventLanguageReload(string path)
        {
            this.load(path);
        }

        public void load(string path)
        {
            this.loadSingleText(TestTranslatorCenter.tranName, path);
            this.loadSingleText(TestTranslatorCenter.tranDescription, path);
            this.loadSingleText(TestTranslatorCenter.tranStory, path);
            this.loadSingleText(TestTranslatorCenter.tranExtra, path);
        }

        public Dictionary<string, string> getLanguageFromYourDatabase(string path)
        {
            return null;
        }

        private void loadSingleText(TezTranslator translator, string path)
        {
            foreach (var item in this.getLanguageFromYourDatabase(path))
            {
                translator.loadSingle(item.Key, item.Value);
            }
        }

        public void test()
        {
            this.init();
            TezTranslator.switchLanguage("Chinese");
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

