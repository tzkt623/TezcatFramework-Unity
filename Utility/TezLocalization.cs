using System.Collections.Generic;
using UnityEngine;

namespace tezcat.Utility
{
    public static class TezLocalization
    {
        static Dictionary<string, string> m_Name = new Dictionary<string, string>();
        static Dictionary<string, string> m_Description = new Dictionary<string, string>();

        static Dictionary<string, string> m_CustomName = new Dictionary<string, string>();
        static Dictionary<string, string> m_CustomDescription = new Dictionary<string, string>();

        public static void init()
        {
            var data = LoadJson.loadJson(Application.dataPath + "/GameData/Localization/name.json");

            var keys = data.Keys;
            foreach (var key in keys)
            {
                m_Name.Add(key, data[key].ToString());
            }

            data = LoadJson.loadJson(Application.dataPath + "/GameData/Localization/description.json");

            keys = data.Keys;
            foreach (var key in keys)
            {
                m_Description.Add(key, data[key].ToString());
            }
        }

        public static void saveName(string key, string value)
        {
            if (m_Name.ContainsKey(key))
            {
                m_Name[key] = value;
            }
            else
            {
                m_Name.Add(key, value);
            }
        }

        public static void saveDescription(string key, string value)
        {
            if (m_Description.ContainsKey(key))
            {
                m_Description[key] = value;
            }
            else
            {
                m_Description.Add(key, value);
            }
        }

        public static string getName(string key, string value = "$error_name")
        {
            if(string.IsNullOrEmpty(key))
            {
                return value;
            }

            string temp = null;
            if (!m_Name.TryGetValue(key, out temp))
            {
                temp = value;
                m_Name.Add(key, temp);
            }

            return temp;
        }

        public static string getDescription(string key, string value = "$error_description")
        {
            if (string.IsNullOrEmpty(key))
            {
                return value;
            }

            string temp = null;
            if(!m_Description.TryGetValue(key, out temp))
            {
                temp = value;
                m_Description.Add(key, temp);
            }

            return temp;
        }
    }
}

