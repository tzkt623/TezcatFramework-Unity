using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;
using UnityEngine;

namespace tezcat.Framework.Game
{
    public class TezTranslator
    {
        #region Register
        public static event TezEventExtension.Action onLanguageSwitched;
        public static int languageCount => m_LanguageDic.Count;

        static int m_CurrentLanguage = 0;
        static int m_LoadingLanguageIndex = -1;
        static Dictionary<string, int> m_LanguageDic = new Dictionary<string, int>();

        public static int register(string language)
        {
            var index = m_LanguageDic.Count;
            m_LanguageDic.Add(language, index);
            return index;
        }

        public static bool switchLanguage(string language)
        {
            if (m_LanguageDic.TryGetValue(language, out var index))
            {
                if (m_CurrentLanguage != index)
                {
                    m_CurrentLanguage = index;
                    onLanguageSwitched?.Invoke();
                    return true;
                }
            }

            return false;
        }

        public static void beginLoad(string language)
        {
            if (m_LoadingLanguageIndex != -1)
            {
                throw new Exception("TezTranslator : beginLoad Error");
            }

            if (m_LanguageDic.TryGetValue(language, out var index))
            {
                m_LoadingLanguageIndex = index;
                if (m_LoadingLanguageIndex != -1)
                {
                    return;
                }
            }

            throw new Exception(string.Format("TezTranslator : No translation for [0]", language));
        }

        public static void endLoad(string language)
        {
            m_LanguageDic.TryGetValue(language, out var index);

            if (m_LoadingLanguageIndex != index)
            {
                throw new Exception("TezTranslator : endLoad Error");
            }

            m_LoadingLanguageIndex = -1;
        }
        #endregion

        public class Slot
        {
            public string[] contents;
        }

        Dictionary<string, Slot> m_SlotDic = new Dictionary<string, Slot>();

        /// <summary>
        /// 一次性加载所有语言
        /// 不需要Begin/End函数配合
        /// </summary>
        public void loadAll(string key, string[] values)
        {
            if (m_SlotDic.TryGetValue(key, out var slot))
            {
                slot.contents = values;
                throw new Exception("TezTranslator : Warning!!! Load Language Twice");
            }
            else
            {
                slot = new Slot()
                {
                    contents = values
                };

                m_SlotDic.Add(key, slot);
            }
        }

        /// <summary>
        /// 单语言加载模式
        /// 需要Begin/End函数配合
        /// </summary>
        public void loadSingle(string key, string value)
        {
            if (m_LoadingLanguageIndex == -1)
            {
                throw new Exception("TezTranslator : Try Load Error Translation");
            }

            if (!m_SlotDic.TryGetValue(key, out var slot))
            {
                slot = new Slot()
                {
                    contents = new string[languageCount]
                };
                m_SlotDic.Add(key, slot);
            }

            slot.contents[m_LoadingLanguageIndex] = value;
        }

        public string translate(string key)
        {
            if (m_SlotDic.TryGetValue(key, out var slot))
            {
                return slot.contents[m_CurrentLanguage];
            }

            return string.Format("${0}$", key);
        }

        public string translate(string key, string defaultValue)
        {
            if (m_SlotDic.TryGetValue(key, out var slot))
            {
                return slot.contents[m_CurrentLanguage];
            }

            return defaultValue;
        }
    }
}

