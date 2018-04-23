using System.IO;
using tezcat.Utility;
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
                return rootPath + localizationFile;
            }
        }

        public static string databaseFile { get; private set; } = "/Database.json";
        public static string databasePath
        {
            get
            {
                return rootPath + databaseFile;
            }
        }

        public static string saveFile { get; private set; } = "/Save.json";
        public static string savePath
        {
            get
            {
                return rootPath + saveFile;
            }
        }

        public static void checkNeedFile()
        {
            if (!TezFileTool.directoryExist(TezcatFramework.rootPath))
            {
                var info = TezFileTool.createDirectory(TezcatFramework.rootPath);
            }

            checkFile(
                TezcatFramework.localizationPath,

                (StreamWriter writer)=>
                {
                    writer.Write(
                        "{" +
                        "\"name\":{}," +
                        "\"description\":{}" +
                        "}");
                });

            checkFile(
                TezcatFramework.databasePath,

                (StreamWriter writer) =>
                {
                    writer.Write("[]");
                });

            checkFile(
                TezcatFramework.savePath,

                (StreamWriter writer) =>
                {

                });
        }

        private static void checkFile(string path, TezEventBus.Action<StreamWriter> action)
        {
            if (!TezFileTool.fileExist(path))
            {
                var writer = TezFileTool.createTextFile(path);
                action(writer);
                writer.Close();
            }
        }
    }
}