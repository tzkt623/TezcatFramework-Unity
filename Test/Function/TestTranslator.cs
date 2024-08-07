﻿using System;
using tezcat.Framework.Game;

namespace tezcat.Framework.Test
{
    public class TestTranslator : TezBaseTest
    {
        public TestTranslator() : base("Translator")
        {
        }

        protected override void onClose()
        {
            TezTranslator.clearConfig();
        }

        public override void init()
        {
            TezTranslator.loadConfig($"{Path.root}Res/Localization");
        }

        public override void run()
        {
            Console.WriteLine("Input: (en, zh, jp)");
            Console.Write("Language:");
            string language = Console.ReadLine();
            TezcatFramework.translator.loadLanguage(language, "en");

            Console.WriteLine($"HP: {TezcatFramework.translator.translate("HP")}");
            Console.WriteLine($"MP: {TezcatFramework.translator.translate("MP")}");
            Console.WriteLine($"Armor: {TezcatFramework.translator.translate("Armor")}");
        }
    }
}