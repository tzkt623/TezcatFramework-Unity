using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 翻译器(默认AllInOne模式)
    /// 
    /// <para>
    /// 提供两种翻译加载模式
    /// </para>
    /// 
    /// <para>
    /// -AllInOne
    /// 一次性加载所有的翻译文本
    /// 适合文本量较小的游戏
    /// </para>
    /// 
    /// <para>
    /// -Single
    /// 只加载一种语言的文本
    /// 适合文本量较大游戏
    /// </para>
    /// 
    /// <para>
    /// 不管哪种模式
    /// 都可以在运行时切换语言
    /// </para>
    /// 
    /// </summary>
    public class TezTranslator
    {
        #region Register
        class InfoSlot
        {
            public int index;
            public string name;
            public string path;
        }

        /// <summary>
        /// 翻译切换时,通知文本刷新变化
        /// </summary>
        public static event TezEventExtension.Action eventLanguageSwitched;

        /// <summary>
        /// Single时
        /// 需要调用此方法重新从磁盘里加载翻译文本
        /// </summary>
        public static event TezEventExtension.Action<string> eventLanguageReload;

        public static int languageCount => m_LanguageList.Count;

        static int m_CurrentLanguage = 0;
        static int m_LoadingLanguageIndex = -1;
        static List<InfoSlot> m_LanguageList = new List<InfoSlot>();
        static TezTranslationSlot.Category m_Mode = TezTranslationSlot.Category.AllInOne;

        /// <summary>
        /// 设置文本保存模式
        /// 
        /// <para>- AllInOne一次性加载所有翻译到内存中</para>
        /// <para>- Single只加载一种翻译到内存中</para>
        /// </summary>
        public static void setMode(TezTranslationSlot.Category category)
        {
            m_Mode = category;
        }

        public static int register(string language)
        {
            if (m_Mode != TezTranslationSlot.Category.AllInOne)
            {
                throw new Exception(string.Format("TezTranslator : AllInOne Mode Only"));
            }

            if (findInfoSlot(language) != null)
            {
                throw new Exception(string.Format("TezTranslator : {0} Already Exist", language));
            }

            var index = m_LanguageList.Count;
            m_LanguageList.Add(new InfoSlot()
            {
                index = m_LanguageList.Count,
                name = language,
                path = null
            });
            return index;
        }

        public static int register(string language, string path)
        {
            if (m_Mode != TezTranslationSlot.Category.Single)
            {
                throw new Exception(string.Format("TezTranslator : Single Mode Only"));
            }

            if (findInfoSlot(language) != null)
            {
                throw new Exception(string.Format("TezTranslator : {0} Already Exist", language));
            }

            var index = m_LanguageList.Count;
            m_LanguageList.Add(new InfoSlot()
            {
                index = m_LanguageList.Count,
                name = language,
                path = path
            });
            return index;
        }

        public static bool switchLanguage(string language)
        {
            var result = findInfoSlot(language);

            if (result != null)
            {
                if (m_CurrentLanguage != result.index)
                {
                    m_CurrentLanguage = result.index;

                    if (m_Mode == TezTranslationSlot.Category.Single)
                    {
                        eventLanguageReload?.Invoke(result.name);
                    }

                    eventLanguageSwitched?.Invoke();
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

            var info = findInfoSlot(language);
            if (info != null)
            {
                m_LoadingLanguageIndex = info.index;
                return;
            }

            throw new Exception(string.Format("TezTranslator : No translation for [0]", language));
        }

        public static void endLoad(string language)
        {
            var info = findInfoSlot(language);
            if (m_LoadingLanguageIndex != info.index)
            {
                throw new Exception("TezTranslator : endLoad Error");
            }

            m_LoadingLanguageIndex = -1;
        }

        private static InfoSlot findInfoSlot(string language)
        {
            return m_LanguageList.Find((InfoSlot slot) =>
            {
                return slot.name == language;
            });
        }

        private TezTranslationSlot createTranslationSlot()
        {
            TezTranslationSlot slot = null;
            switch (m_Mode)
            {
                case TezTranslationSlot.Category.AllInOne:
                    slot = new TezTranslationSlot_AllInOne();
                    break;
                case TezTranslationSlot.Category.Single:
                    slot = new TezTranslationSlot_Single();
                    break;
                default:
                    break;
            }


            slot.initContent(languageCount);
            return slot;
        }

        #endregion

        Dictionary<string, TezTranslationSlot> m_SlotDic = new Dictionary<string, TezTranslationSlot>();

        /// <summary>
        /// 一次性加载所有语言
        /// 不需要Begin/End函数配合
        /// </summary>
        public void loadAll(string key, string[] values)
        {
            if (m_Mode != TezTranslationSlot.Category.AllInOne)
            {
                throw new Exception("TezTranslator : AllInOne Mode Only");
            }

            if (!m_SlotDic.TryGetValue(key, out var slot))
            {
                slot = createTranslationSlot();
                m_SlotDic.Add(key, slot);
            }

            for (int i = 0; i < values.Length; i++)
            {
                slot.setContent(i, values[i]);
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
                slot = createTranslationSlot();
                m_SlotDic.Add(key, slot);
            }

            slot.setContent(m_LoadingLanguageIndex, value);
        }

        public string translate(string key)
        {
            if (m_SlotDic.TryGetValue(key, out var slot))
            {
                return slot.getContent(m_CurrentLanguage);
            }

            return string.Format("${0}$", key);
        }

        public string translate(string key, string defaultValue)
        {
            if (m_SlotDic.TryGetValue(key, out var slot))
            {
                return slot.getContent(m_CurrentLanguage);
            }

            return defaultValue;
        }
    }
}

