using LitJson;
using System.Collections.Generic;
using System.IO;

namespace tezcat
{
    public abstract class TezJson
    {
        protected JsonData m_Root = null;

        protected Stack<JsonData> m_DataStack = new Stack<JsonData>();

        /// <summary>
        /// 弹出栈顶保存的对象
        /// </summary>
        public void exit()
        {
            m_Root = m_DataStack.Pop();
        }

        public bool containsKey(string name)
        {
            return m_Root.Keys.Contains(name);
        }

        public int count()
        {
            return m_Root.Count;
        }

        /// <summary>
        /// 从一个json对象中取出一个子对象 并将当前对象压栈保存
        /// </summary>
        /// <param name="name"></param>
        public void enter(string name)
        {
            m_DataStack.Push(m_Root);
            m_Root = m_Root[name];
        }

        /// <summary>
        /// 从一个json数组中取出一个子对象 并将当前对象压栈保存
        /// </summary>
        /// <param name="index"></param>
        public void enter(int index)
        {
            m_DataStack.Push(m_Root);
            m_Root = m_Root[index];
        }

        public bool tryEnter(string name)
        {
            if (this.containsKey(name))
            {
                this.enter(name);
                return true;
            }

            return false;
        }
    }

    public class TezJsonReader : TezJson
    {
        public ICollection<string> keys
        {
            get { return m_Root.Keys; }
        }

        public void load(string path)
        {
            m_DataStack.Clear();
            m_Root = null;
            m_Root = JsonMapper.ToObject(File.ReadAllText(path));
        }

        public bool getBool(int index)
        {
            return (bool)m_Root[index];
        }

        public int getInt(int index)
        {
            return (int)m_Root[index];
        }

        public float getFloat(int index)
        {
            return (float)m_Root[index];
        }

        public string getString(int index)
        {
            return (string)m_Root[index];
        }

        public JsonType getType(int index)
        {
            return m_Root[index].GetJsonType();
        }

        public bool getBool(string name)
        {
            return (bool)m_Root[name];
        }

        public bool tryGetBool(string name, bool dvalue = false)
        {
            JsonData data = null;
            if (m_Root.tryGet(name, out data))
            {
                return (bool)data;
            }

            return dvalue;
        }

        public float getFloat(string name)
        {
            return (float)m_Root[name];
        }

        public int getInt(string name)
        {
            return (int)m_Root[name];
        }

        public int tryGetInt(string name, int dvalue = 0)
        {
            JsonData data = null;
            if(m_Root.tryGet(name, out data))
            {
                return (int)data;
            }

            return dvalue;
        }

        public string getString(string name)
        {
            return (string)m_Root[name];
        }

        public JsonType getType(string name)
        {
            return m_Root[name].GetJsonType();
        }
    }

    public class TezJsonWriter : TezJson
    {
        public TezJsonWriter(bool array = false)
        {
            m_Root = new JsonData();
            m_Root.SetJsonType(array ? JsonType.Array : JsonType.Object);
        }

        public void save(string path)
        {
            File.WriteAllText(path, JsonMapper.ToJson(m_Root));
        }

        public void newData(JsonType type)
        {
            m_DataStack.Clear();
            m_Root = new JsonData();
            m_Root.SetJsonType(type);
        }

        public void beginObject(string name)
        {
            m_DataStack.Push(m_Root);
            if(m_Root.Keys.Contains(name))
            {
                m_Root = m_Root[name];
            }
            else
            {
                var data = new JsonData();
                data.SetJsonType(JsonType.Object);
                m_Root[name] = data;
                m_Root = data;
            }
        }

        public void endObject(string name)
        {
            if (!m_Root.IsObject)
            {
                throw new System.Exception();
            }
            m_Root = m_DataStack.Pop();
        }

        public void beginObject()
        {
            m_DataStack.Push(m_Root);
            var data = new JsonData();
            data.SetJsonType(JsonType.Object);
            m_Root.Add(data);
            m_Root = data;
        }

        public void endObject()
        {
            if (!m_Root.IsObject)
            {
                throw new System.Exception();
            }
            m_Root = m_DataStack.Pop();
        }

        public void pushValue(string name, bool value)
        {
            m_Root[name] = value;
        }

        public void pushValue(string name, int value)
        {
            m_Root[name] = value;
        }

        public void pushValue(string name, float value)
        {
            m_Root[name] = value;
        }

        public void pushValue(string name, string value)
        {
            m_Root[name] = value;
        }

        public void beginArray(string name)
        {
            m_DataStack.Push(m_Root);
            if(m_Root.Keys.Contains(name))
            {
                m_Root = m_Root[name];
            }
            else
            {
                var data = new JsonData();
                data.SetJsonType(JsonType.Array);
                m_Root[name] = data;
                m_Root = data;
            }
        }

        public void beginArray()
        {
            m_DataStack.Push(m_Root);
            var data = new JsonData();
            data.SetJsonType(JsonType.Array);
            m_Root.Add(data);
            m_Root = data;
        }

        public void endArray()
        {
            if (!m_Root.IsArray)
            {
                throw new System.Exception();
            }
            m_Root = m_DataStack.Pop();
        }

        public void addValue(bool value)
        {
            m_Root.Add(value);
        }

        public void addValue(int value)
        {
            m_Root.Add(value);
        }

        public void addValue(float value)
        {
            m_Root.Add(value);
        }

        public void addValue(string value)
        {
            m_Root.Add(value);
        }
    }

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


