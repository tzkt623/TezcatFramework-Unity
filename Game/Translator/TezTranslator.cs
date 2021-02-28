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
        Dictionary<string, string> m_Dic = new Dictionary<string, string>();

        public void add(string key, string value)
        {
            m_Dic.Add(key, value);
        }

        public string translate(string key)
        {
            if (m_Dic.TryGetValue(key, out string result))
            {
                return result;
            }

            return string.Format("${0}$", key);
        }

        public string translate(string key, string defaultValue)
        {
            if (m_Dic.TryGetValue(key, out string result))
            {
                return result;
            }

            return defaultValue;
        }
    }

    public class TezTranslator111
    {
        #region DataCenter
        public static class DataCenter
        {
            static int m_Capacity = 0;
            static List<string[]> m_List = new List<string[]>();

            public static void setCapacity(int capacity)
            {
                m_Capacity = capacity;
            }

            private static void grow(int id)
            {
                while (id >= m_List.Count)
                {
                    m_List.Add(new string[m_Capacity]);
                }
            }

            public static string[] getOrCreateData(TezStaticString<TezTranslator111> idString)
            {
                DataCenter.grow(idString.ID);
                return m_List[idString.ID];
            }

            public static string[] get(TezStaticString<TezTranslator111> idString)
            {
                return m_List[idString.ID];
            }
        }
        #endregion

        /// <summary>
        /// 设置翻译容器的容量
        /// </summary>
        public static int dataCapacity
        {
            set { DataCenter.setCapacity(value); }
        }

        int m_Index = -1;

        /// <summary>
        /// 输入当前翻译器对应的容器Index
        /// </summary>
        public TezTranslator111(int tranIndex)
        {
            m_Index = tranIndex;
        }

        public void add(TezStaticString<TezTranslator111> idString, string value)
        {
            DataCenter.getOrCreateData(idString)[m_Index] = value;
        }

        public string translate(TezStaticString<TezTranslator111> idString)
        {
            return DataCenter.get(idString)[m_Index];
        }

        public string translate(TezStaticString<TezTranslator111> idString, string defaultValue)
        {
            if (idString.ID == 0)
            {
                return defaultValue;
            }

            return DataCenter.get(idString)[m_Index];
        }
    }

    /// <summary>
    /// 翻译机
    /// </summary>
    public class TezTranslatorOOO
    {
        class Text
        {
            public readonly string key;
            public string value;
            public int ID = -1;

            public Text(string key, string value)
            {
                this.key = key;
                this.value = value;
            }

            public void clear()
            {
                value = null;
            }
        }

        class Translator
        {
            Dictionary<string, Text> m_Dic = new Dictionary<string, Text>();
            List<Text> m_List = new List<Text>();
            public int count
            {
                get { return m_List.Count; }
            }

            public void sort()
            {
                m_List.Sort((Text p1, Text p2) =>
                {
                    return string.CompareOrdinal(p1.key, p2.key);
                });

                for (int i = 0; i < m_List.Count; i++)
                {
                    m_List[i].ID = i;
                }
            }

            public bool tryAdd(string key, string value)
            {
                Text package = null;
                if (!m_Dic.TryGetValue(key, out package))
                {
                    this.add(key, value);
                    return true;
                }

                return false;
            }

            public void add(string key, string value)
            {
                Text package = new Text(key, value);
                package.ID = m_List.Count;
                m_Dic.Add(key, package);
                m_List.Add(package);
            }

            public bool remove(string key)
            {
                Text package = null;
                if (m_Dic.TryGetValue(key, out package))
                {
                    var ID = package.ID;
                    if (ID != m_Dic.Count - 1)
                    {
                        m_List[ID] = m_List[m_List.Count - 1];
                        m_List[ID].ID = ID;
                        ID = m_Dic.Count - 1;
                    }

                    m_List.RemoveAt(ID);
                    m_Dic.Remove(key);
                    package.clear();
                    this.sort();
                    return true;
                }

                return false;
            }

            public bool translate(int index, out string key, out string value)
            {
                if (index >= m_Dic.Count || index < 0)
                {
                    key = null;
                    value = null;
                    return false;
                }

                key = m_List[index].key;
                value = m_List[index].value;
                return true;
            }

            public bool translate(string key, out string value)
            {
                value = null;
                if (string.IsNullOrEmpty(key))
                {
                    return false;
                }

                Text package = null;
                if (m_Dic.TryGetValue(key, out package))
                {
                    value = package.value;
                    return true;
                }

                return false;
            }

            public string translate(string key, string value = "$error_description")
            {
                if (string.IsNullOrEmpty(key))
                {
                    return value;
                }

                Text package = null;
                if (m_Dic.TryGetValue(key, out package))
                {
                    return package.value;
                }

                return value;
            }

            public string translate(string key)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return string.Format("##Error Null Or Empty Key##", key);
                }

                Text package = null;
                if (m_Dic.TryGetValue(key, out package))
                {
                    return package.value;
                }

                return string.Format("#{0}", key);
            }

            public void foreachText(TezEventExtension.Action<string, string> action)
            {
                for (int i = 0; i < m_List.Count; i++)
                {
                    action(m_List[i].key, m_List[i].value);
                }
            }

            public void foreachText(TezEventExtension.Action<string, string> action, int begin, int end)
            {
                end = Mathf.Min(end, m_List.Count);
                for (int i = begin; i < end; i++)
                {
                    action(m_List[i].key, m_List[i].value);
                }
            }

            public void close()
            {
                for (int i = 0; i < m_List.Count; i++)
                {
                    m_List[i].clear();
                }
                m_List.Clear();
                m_Dic.Clear();

                m_List = null;
                m_Dic = null;
            }
        }

        Translator m_Name = new Translator();
        Translator m_Description = new Translator();
        Translator m_Extra = new Translator();

        public int nameCount
        {
            get { return m_Name.count; }
        }

        public int descriptionCount
        {
            get { return m_Description.count; }
        }

        public void deserialization(TezReader reader)
        {

        }

        public void serialization(TezWriter writer)
        {

        }

        #region Name
        public void sortName()
        {
            m_Name.sort();
        }

        public bool tryAddName(string key, string value)
        {
            return m_Name.tryAdd(key, value);
        }

        public void addName(string key, string value)
        {
            m_Name.add(key, value);
        }

        public bool removeName(string key)
        {
            return m_Name.remove(key);
        }

        public void foreachName(TezEventExtension.Action<string, string> action)
        {
            m_Name.foreachText(action);
        }

        public void foreachName(TezEventExtension.Action<string, string> action, int begin, int end)
        {
            m_Name.foreachText(action, begin, end);
        }

        public void saveName(int key, string value)
        {

        }

        public void saveName(string key, string value)
        {

        }

        public bool translateName(int index, out string key, out string value)
        {
            return m_Name.translate(index, out key, out value);
        }

        public bool translateName(string key, out string value)
        {
            return m_Name.translate(key, out value);
        }

        public string translateName(string key, string value)
        {
            return m_Name.translate(key, value);
        }

        public string translateName(string key)
        {
            return m_Name.translate(key);
        }
        #endregion

        #region Description
        public void sortDescription()
        {
            m_Description.sort();
        }

        public bool tryAddDescription(string key, string value)
        {
            return m_Description.tryAdd(key, value);
        }

        public void addDescription(string key, string value)
        {
            m_Description.add(key, value);
        }

        public bool removeDescription(string key)
        {
            return m_Description.remove(key);
        }

        public void foreachDescription(TezEventExtension.Action<string, string> action)
        {
            m_Description.foreachText(action);
        }

        public void foreachDescription(TezEventExtension.Action<string, string> action, int begin, int end)
        {
            m_Description.foreachText(action, begin, end);
        }

        public void saveDescription(int key, string value)
        {

        }

        public void saveDescription(string key, string value)
        {

        }

        public bool translateDescription(int index, out string key, out string value)
        {
            return m_Description.translate(index, out key, out value);
        }

        public bool translateDescription(string key, out string value)
        {
            return m_Description.translate(key, out value);
        }

        public string translateDescription(string key, string value = "$error_description")
        {
            return m_Description.translate(key, value);
        }

        public string translateDescription(string key)
        {
            return m_Description.translate(key);
        }
        #endregion

        #region Extra
        public void sortExtra()
        {
            m_Extra.sort();
        }

        public bool tryAddExtra(string key, string value)
        {
            return m_Extra.tryAdd(key, value);
        }

        public void addExtra(string key, string value)
        {
            m_Extra.add(key, value);
        }

        public bool removeExtra(string key)
        {
            return m_Extra.remove(key);
        }

        public void foreachExtra(TezEventExtension.Action<string, string> action)
        {
            m_Extra.foreachText(action);
        }

        public void foreachExtra(TezEventExtension.Action<string, string> action, int begin, int end)
        {
            m_Extra.foreachText(action, begin, end);
        }

        public void saveExtra(int key, string value)
        {

        }

        public void saveExtra(string key, string value)
        {

        }

        public bool translateExtra(int index, out string key, out string value)
        {
            return m_Extra.translate(index, out key, out value);
        }

        public bool translateExtra(string key, out string value)
        {
            return m_Extra.translate(key, out value);
        }

        public string translateExtra(string key, string value = "$error_description")
        {
            return m_Extra.translate(key, value);
        }

        public string translateExtra(string key)
        {
            return m_Extra.translate(key);
        }
        #endregion

        public void close()
        {
            m_Name.close();
            m_Description.close();
            m_Extra.close();

            m_Name = null;
            m_Description = null;
            m_Extra = null;
        }
    }
}

