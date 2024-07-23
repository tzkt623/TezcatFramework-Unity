using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 翻译器
    /// </summary>
    public class TezTranslator
    {
        #region Config

        class LanguageInfo : ITezCloseable
        {
            public string name;
            public string path;

            void ITezCloseable.closeThis()
            {
                this.name = null;
                this.path = null;
            }
        }

        static Dictionary<string, LanguageInfo> sLanguageInfoList = new Dictionary<string, LanguageInfo>();

        public static void loadConfig(string path)
        {
            var dirs = TezFilePath.getDirs(path);
            Dictionary<string, string> dir_dict = new Dictionary<string, string>();
            for (int i = 0; i < dirs.Length; i++)
            {
                dir_dict.Add(dirs[i].Substring(dirs[i].LastIndexOf("/") + 1), dirs[i]);
            }

            var files = TezFilePath.getFiles(path);
            string config_path = null;

            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].EndsWith("config.json"))
                {
                    config_path = files[i];
                    break;
                }
            }

            if (config_path == null)
            {
                throw new Exception(nameof(config_path));
            }

            TezJsonReader reader = new TezJsonReader();
            reader.load(config_path);
            foreach (var key in reader.keys)
            {
                sLanguageInfoList.Add(key, new LanguageInfo()
                {
                    name = reader.readString(key),
                    path = dir_dict[key]
                });
            }
            reader.close();
        }

        public static void clearConfig()
        {
            foreach (var item in sLanguageInfoList)
            {
                item.Value.close();
            }
            sLanguageInfoList.Clear();
        }
        #endregion

        Dictionary<string, string> mDataDict = new Dictionary<string, string>();

        public void loadLanguage(string language, string def)
        {
            mDataDict.Clear();
            if (!sLanguageInfoList.TryGetValue(language, out var info))
            {
                info = sLanguageInfoList[def];
            }

            var files = TezFilePath.getFiles(info.path);
            for (int i = 0; i < files.Length; i++)
            {
                TezFileReader reader = new TezJsonReader();
                reader.load(files[i]);
                foreach (var key in reader.getKeys())
                {
                    mDataDict.Add(key, reader.readString(key));
                }
                reader.close();
            }
        }

        public string translate(string key)
        {
            if (mDataDict.TryGetValue(key, out var value))
            {
                return value;
            }

            return $"#${key}$#";
        }

        public string translate(string key, string defaultValue)
        {
            if (mDataDict.TryGetValue(key, out var value))
            {
                return value;
            }

            return defaultValue;
        }
    }
}

