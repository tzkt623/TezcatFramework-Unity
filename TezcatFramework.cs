using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public class TezcatFramework
    {
        static string m_RootDir = null;
        public static string rootPath
        {
            get
            {
                if (string.IsNullOrEmpty(m_RootDir))
                {
                    m_RootDir = Application.dataPath;
                    int index = m_RootDir.IndexOf(Application.productName);
                    m_RootDir = m_RootDir.Substring(0, index + Application.productName.Length) + "/TezSave";
                }

                return m_RootDir;
            }
        }
        public static string localizationFile { get; private set; } = "/Localization.json";
        public static string localizationPath
        {
            get
            {
                return m_RootDir + localizationFile;
            }
        }

        public static string databaseFile { get; private set; } = "/Database.json";
        public static string databasePath
        {
            get
            {
                return m_RootDir + databaseFile;
            }
        }

        public static string saveFile { get; private set; } = "/Save.json";
        public static string savePath
        {
            get
            {
                return m_RootDir + saveFile;
            }
        }
    }
}