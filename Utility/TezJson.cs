using LitJson;
using System.IO;

namespace tezcat.Framework.Utility
{
    public static class LoadJson
    {
        public static JsonData loadJson(string path)
        {
            return JsonMapper.ToObject(File.ReadAllText(path));
        }

        public static void saveJson(string path, JsonData data)
        {
            File.WriteAllText(path, JsonMapper.ToJson(data));
        }

        public static bool getBool(JsonData data, string name)
        {
            return (bool)data[name];
        }

        public static float getFloat(JsonData data, string name)
        {
            return (float)(double)data[name];
        }

        public static int getInt(JsonData data, string name)
        {
            return (int)data[name];
        }

        public static string getString(JsonData data, string name)
        {
            return (string)data[name];
        }

        public static void pushBool(JsonData data, string name, bool value)
        {
            data[name] = value;
        }

        public static void pushFloat(JsonData data, string name, float value)
        {
            data[name] = value;
        }
        public static void pushInt(JsonData data, string name, int value)
        {
            data[name] = value;
        }
        public static void pushString(JsonData data, string name, string value)
        {
            data[name] = value;
        }
    }
}


