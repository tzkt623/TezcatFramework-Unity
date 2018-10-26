using System.Collections.Generic;
using tezcat.Framework.DataBase;
using tezcat.Framework.Extension;
using UnityEngine;

namespace tezcat.Framework.Core
{
    public static class TezTranslator
    {
        public class Text
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

        static Dictionary<string, Text> m_NameDic = new Dictionary<string, Text>();
        static List<Text> m_NameList = new List<Text>();
        public static int nameCount
        {
            get { return m_NameList.Count; }
        }

        static Dictionary<string, Text> m_DescriptionDic = new Dictionary<string, Text>();
        static List<Text> m_DescriptionList = new List<Text>();
        public static int descriptionCount
        {
            get { return m_DescriptionList.Count; }
        }

        public static void deserialization(TezReader reader)
        {
            reader.beginObject("name");
            foreach (var key in reader.getKeys())
            {
                var package = new Text(key, reader.readString(key));
                package.ID = m_NameList.Count;
                m_NameDic.Add(key, package);
                m_NameList.Add(package);
            }
            reader.endObject("name");

            reader.beginObject("description");
            foreach (var key in reader.getKeys())
            {
                var package = new Text(key, reader.readString(key));
                package.ID = m_DescriptionList.Count;
                m_DescriptionDic.Add(key, package);
                m_DescriptionList.Add(package);
            }
            reader.endObject("description");

            sortName();
            sortDescription();
        }

        public static void serialization(TezWriter writer)
        {
            writer.beginObject("name");
            for (int i = 0; i < m_NameList.Count; i++)
            {
                writer.write(m_NameList[i].key, m_NameList[i].value);
            }
            writer.endObject("name");

            writer.beginObject("description");
            for (int i = 0; i < m_DescriptionList.Count; i++)
            {
                writer.write(m_DescriptionList[i].key, m_DescriptionList[i].value);
            }
            writer.endObject("description");
        }

        #region Name
        public static void sortName()
        {
            m_NameList.Sort((Text p1, Text p2) =>
            {
                return string.CompareOrdinal(p1.key, p2.key);
            });

            for (int i = 0; i < m_NameList.Count; i++)
            {
                m_NameList[i].ID = i;
            }
        }

        public static bool tryAddName(string key, string value)
        {
            Text package = null;
            if (!m_NameDic.TryGetValue(key, out package))
            {
                addName(key, value);
                return true;
            }

            return false;
        }

        public static void addName(string key, string value)
        {
            var package = new Text(key, value);
            package.ID = m_NameList.Count;
            m_NameDic.Add(key, package);
            m_NameList.Add(package);

            sortName();
        }

        public static bool removeName(string key)
        {
            Text package = null;
            if (m_NameDic.TryGetValue(key, out package))
            {
                var ID = package.ID;
                if (ID != m_NameList.Count - 1)
                {
                    m_NameList[ID] = m_NameList[m_NameList.Count - 1];
                    m_NameList[ID].ID = ID;
                    ID = m_NameList.Count - 1;
                }

                m_NameList.RemoveAt(ID);
                m_NameDic.Remove(key);
                package.clear();
                sortName();
                return true;
            }

            return false;
        }

        public static void foreachName(TezEventExtension.Action<string, string> action)
        {
            for (int i = 0; i < m_NameList.Count; i++)
            {
                action(m_NameList[i].key, m_NameList[i].value);
            }
        }

        public static void foreachName(TezEventExtension.Action<string, string> action, int begin, int end)
        {
            end = Mathf.Min(end, m_NameList.Count);
            for (int i = begin; i < end; i++)
            {
                action(m_NameList[i].key, m_NameList[i].value);
            }
        }

        public static void saveName(int key, string value)
        {
            if (key >= m_NameList.Count || key < 0)
            {
                return;
            }

            m_NameList[key].value = value;
        }

        public static void saveName(string key, string value)
        {
            Text package = null;
            if (m_NameDic.TryGetValue(key, out package))
            {
                package.value = value;
            }
            else
            {
                addName(key, value);
            }
        }

        public static bool translateName(int index, out string key, out string value)
        {
            if (index >= m_NameList.Count || index < 0)
            {
                key = null;
                value = null;
                return false;
            }

            key = m_NameList[index].key;
            value = m_NameList[index].value;
            return true;
        }

        public static bool translateName(string key, out string value)
        {
            value = null;
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            Text package = null;
            if (m_NameDic.TryGetValue(key, out package))
            {
                value = package.value;
                return true;
            }

            return false;
        }

        public static string translateName(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                return value;
            }

            Text package = null;
            if (m_NameDic.TryGetValue(key, out package))
            {
                return package.value;
            }

            return value;
        }

        public static string translateName(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Format("#{0}", key);
            }

            Text package = null;
            if (m_NameDic.TryGetValue(key, out package))
            {
                return package.value;
            }

            return string.Format("#{0}", key);
        }
        #endregion

        #region Description
        public static void sortDescription()
        {
            m_DescriptionList.Sort((Text p1, Text p2) =>
            {
                return string.CompareOrdinal(p1.key, p2.key);
            });

            for (int i = 0; i < m_DescriptionList.Count; i++)
            {
                m_DescriptionList[i].ID = i;
            }
        }

        public static bool tryAddDescription(string key, string value)
        {
            Text package = null;
            if (!m_DescriptionDic.TryGetValue(key, out package))
            {
                addDescription(key, value);
                return true;
            }

            return false;
        }

        public static void addDescription(string key, string value)
        {
            Text package = new Text(key, value);
            package.ID = m_NameList.Count;
            m_DescriptionDic.Add(key, package);
            m_DescriptionList.Add(package);

            sortDescription();
        }

        public static void removeDescription(string key)
        {
            Text package = null;
            if (m_DescriptionDic.TryGetValue(key, out package))
            {
                var ID = package.ID;
                if (ID != m_DescriptionList.Count - 1)
                {
                    m_DescriptionList[ID] = m_DescriptionList[m_DescriptionList.Count - 1];
                    m_DescriptionList[ID].ID = ID;
                    ID = m_DescriptionList.Count - 1;
                }

                m_DescriptionList.RemoveAt(ID);
                m_DescriptionDic.Remove(key);
                package.clear();
                sortDescription();
            }
        }

        public static void foreachDescription(TezEventExtension.Action<string, string> action)
        {
            for (int i = 0; i < m_DescriptionList.Count; i++)
            {
                action(m_DescriptionList[i].key, m_DescriptionList[i].value);
            }
        }

        public static void foreachDescription(TezEventExtension.Action<string, string> action, int begin, int end)
        {
            end = Mathf.Min(end, m_DescriptionList.Count);
            for (int i = begin; i < end; i++)
            {
                action(m_DescriptionList[i].key, m_DescriptionList[i].value);
            }
        }

        public static void saveDescription(int key, string value)
        {
            if (key >= m_DescriptionList.Count || key < 0)
            {
                return;
            }

            m_DescriptionList[key].value = value;
        }

        public static void saveDescription(string key, string value)
        {
            Text package = null;
            if (m_DescriptionDic.TryGetValue(key, out package))
            {
                package.value = value;
            }
            else
            {
                addDescription(key, value);
            }
        }

        public static bool translateDescription(int index, out string key, out string value)
        {
            if (index >= m_DescriptionList.Count || index < 0)
            {
                key = null;
                value = null;
                return false;
            }

            key = m_DescriptionList[index].key;
            value = m_DescriptionList[index].value;
            return true;
        }

        public static bool translateDescription(string key, out string value)
        {
            value = null;
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            Text package = null;
            if (m_DescriptionDic.TryGetValue(key, out package))
            {
                value = package.value;
                return true;
            }

            return false;
        }

        public static string translateDescription(string key, string value = "$error_description")
        {
            if (string.IsNullOrEmpty(key))
            {
                return value;
            }

            Text package = null;
            if (m_DescriptionDic.TryGetValue(key, out package))
            {
                return package.value;
            }

            return value;
        }

        public static string translateDescription(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                return string.Format("#{0}", key);
            }

            Text package = null;
            if (m_DescriptionDic.TryGetValue(key, out package))
            {
                return package.value;
            }

            return string.Format("#{0}", key);
        }
        #endregion
    }
}

