using System;
using System.Collections.Generic;
using tezcat.Serialization;
using UnityEngine;

namespace tezcat.Utility
{
    public static class TezLocalization
    {
        class Package
        {
            public readonly string key;
            public string value;

            public Package(string key, string value)
            {
                this.key = key;
                this.value = value;
            }
        }

        static Dictionary<string, int> m_NameDic = new Dictionary<string, int>();
        static List<Package> m_NameList = new List<Package>();
        public static int nameCount
        {
            get { return m_NameList.Count; }
        }

        static Dictionary<string, int> m_DescriptionDic = new Dictionary<string, int>();
        static List<Package> m_DescriptionList = new List<Package>();
        public static int descriptionCount
        {
            get { return m_DescriptionList.Count; }
        }

        public static void init()
        {
            var data = LoadJson.loadJson(Application.dataPath + "/GameData/Localization/name.json");

            var keys = data.Keys;
            foreach (var key in keys)
            {
                m_NameDic.Add(key, m_NameList.Count);
                m_NameList.Add(new Package(key, data[key].ToString()));
            }

            data = LoadJson.loadJson(Application.dataPath + "/GameData/Localization/description.json");

            keys = data.Keys;
            foreach (var key in keys)
            {
                m_DescriptionDic.Add(key, m_DescriptionList.Count);
                m_DescriptionList.Add(new Package(key, data[key].ToString()));
            }
        }

        public static void deserialization(TezReader reader)
        {
            reader.beginObject("name");
            foreach (var key in reader.getKeys())
            {
                m_NameDic.Add(key, m_NameList.Count);
                m_NameList.Add(new Package(key, reader.readString(key)));
            }
            reader.endObject("name");


            reader.beginObject("description");
            foreach (var key in reader.getKeys())
            {
                m_DescriptionDic.Add(key, m_DescriptionList.Count);
                m_DescriptionList.Add(new Package(key, reader.readString(key)));
            }
            reader.endObject("description");
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

        public static bool tryAddName(string key, string value)
        {
            int index = -1;
            if(!m_NameDic.TryGetValue(key, out index))
            {
                index = m_NameList.Count;
                m_NameDic.Add(key, index);
                m_NameList.Add(new Package(key, value));
                return true;
            }

            return false;
        }

        public static int addName(string key, string value)
        {
            int index = m_NameList.Count;
            m_NameDic.Add(key, index);
            m_NameList.Add(new Package(key, value));
            return index;
        }

        public static void removeName(int index)
        {
            var pack = m_NameList[index];
            m_NameList.Remove(index);
            m_NameDic.Remove(pack.key);
        }

        public static void removeDescription(int index)
        {
            var pack = m_DescriptionList[index];
            m_DescriptionList.Remove(index);
            m_DescriptionDic.Remove(pack.key);
        }

        public static bool tryAddDescription(string key, string value)
        {
            int index = -1;
            if (!m_DescriptionDic.TryGetValue(key, out index))
            {
                index = m_DescriptionList.Count;
                m_DescriptionDic.Add(key, index);
                m_DescriptionList.Add(new Package(key, value));
                return true;
            }

            return false;
        }

        public static int addDescription(string key, string value)
        {
            int index = m_DescriptionList.Count;
            m_DescriptionDic.Add(key, index);
            m_DescriptionList.Add(new Package(key, value));
            return index;
        }

        public static void foreachName(TezEventBus.Action<int, string, string> action)
        {
            for (int i = 0; i < m_NameList.Count; i++)
            {
                action(i, m_NameList[i].key, m_NameList[i].value);
            }
        }

        public static void foreachName(TezEventBus.Action<int, string, string> action, int begin, int end)
        {
            end = Mathf.Min(end, m_NameList.Count);
            for (int i = begin; i < end; i++)
            {
                action(i, m_NameList[i].key, m_NameList[i].value);
            }
        }

        public static void foreachDescription(TezEventBus.Action<int, string, string> action)
        {
            for (int i = 0; i < m_DescriptionList.Count; i++)
            {
                action(i, m_DescriptionList[i].key, m_DescriptionList[i].value);
            }
        }

        public static void foreachDescription(TezEventBus.Action<int, string, string> action, int begin, int end)
        {
            end = Mathf.Min(end, m_DescriptionList.Count);
            for (int i = begin; i < end; i++)
            {
                action(i, m_DescriptionList[i].key, m_DescriptionList[i].value);
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
            int index = -1;
            if (m_NameDic.TryGetValue(key, out index))
            {
                m_NameList[index].value = value;
            }
            else
            {
                m_NameDic.Add(key, m_NameList.Count);
                m_NameList.Add(new Package(key, value));
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
            int index = -1;
            if (m_DescriptionDic.TryGetValue(key, out index))
            {
                m_DescriptionList[index].value = value;
            }
            else
            {
                m_DescriptionDic.Add(key, m_DescriptionList.Count);
                m_DescriptionList.Add(new Package(key, value));
            }
        }

        public static bool getName(int index, out string key, out string value)
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

        public static bool getName(string key, out string value, out int index)
        {
            index = -1;
            value = null;
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            if (m_NameDic.TryGetValue(key, out index))
            {
                value = m_NameList[index].value;
                return true;
            }

            return false;
        }

        public static string getName(string key, string value = "$error_name")
        {
            if (string.IsNullOrEmpty(key))
            {
                return value;
            }

            int index = -1;
            if (m_NameDic.TryGetValue(key, out index))
            {
                return m_NameList[index].value;
            }

            return value;
        }

        public static bool getDescription(int index, out string key, out string value)
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

        public static bool getDescription(string key, out string value, out int index)
        {
            index = -1;
            value = null;
            if (string.IsNullOrEmpty(key))
            {
                return false;
            }

            if (m_DescriptionDic.TryGetValue(key, out index))
            {
                value = m_DescriptionList[index].value;
                return true;
            }

            return false;
        }

        public static string getDescription(string key, string value = "$error_description")
        {
            if (string.IsNullOrEmpty(key))
            {
                return value;
            }

            int index = -1;
            if (m_DescriptionDic.TryGetValue(key, out index))
            {
                return m_DescriptionList[index].value;
            }

            return value;
        }
    }
}

