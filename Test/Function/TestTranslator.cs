using System;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    public class TestTranslator : TezBaseTest
    {
        public TestTranslator() : base("Translator")
        {
            TezTranslator.loadConfig(Path.root + "Localization");
        }

        public override void run()
        {
            Console.Write("Choose: en, zh, jp)\n");
            Console.Write("Language:");
            string language = Console.ReadLine();
            TezcatFramework.translator.loadLanguage(language, "en");

            Console.WriteLine($"HP: {TezcatFramework.translator.translate("HP")}");
            Console.WriteLine($"MP: {TezcatFramework.translator.translate("MP")}");
            Console.WriteLine($"Armor: {TezcatFramework.translator.translate("Armor")}");
        }
    }
}

