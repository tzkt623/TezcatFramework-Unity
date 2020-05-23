using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace tezcat.Framework.Database
{
    public class TezJsonReader : TezReader
    {
        private class Data
        {
            public int index = -1;
            public string name = string.Empty;
            public JsonData jsonData;

            public Data(JsonData data, string name)
            {
                this.name = name;
                this.jsonData = data;
            }

            public Data(JsonData data, int index)
            {
                this.index = index;
                this.jsonData = data;
            }

            public void close()
            {
                this.name = null;
                this.jsonData = null;
            }
        }

        private JsonData m_Current = null;
        private Stack<Data> m_PreRoot = new Stack<Data>();
        private string m_Path;

        public ICollection<string> keys
        {
            get { return m_Current.Keys; }
        }

        public override int count
        {
            get { return m_Current.Count; }
        }

        public bool loadContent(string json_string)
        {
            m_Current = JsonMapper.ToObject(json_string);
            return m_Current.IsArray | m_Current.IsObject;
        }

        public override bool load(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            m_Path = path;
            bool result = false;
            m_PreRoot.Clear();
            string content = null;
            try
            {
                content = File.ReadAllText(m_Path);
                result = true;
            }
            catch
            {
                result = false;
            }

            if (result)
            {
                result = this.loadContent(content);
            }
            return result;
        }

        private string throwInfo(string function_name, string position)
        {
            StringBuilder content = new StringBuilder();
            content.AppendLine(string.Format("{0}:{1} Error", function_name, position));
            if (m_PreRoot.Count > 0)
            {
                content.AppendLine("From This Path:");
                foreach (var data in m_PreRoot)
                {
                    var json = data.jsonData;
                    if (json.IsObject)
                    {
                        content.AppendLine(data.name);
                    }
                    else if (json.IsArray)
                    {
                        content.AppendLine(data.index.ToString());
                    }
                }
            }
            content.AppendLine(m_Path);

            return content.ToString();
        }

        private JsonData readAny(int key, string function_name)
        {
            JsonData data = null;
            if (m_Current.tryGet(key, out data))
            {
                return data;
            }

            throw new Exception(this.throwInfo(function_name, key.ToString()));
        }

        private JsonData readAny(string key, string function_name)
        {
            JsonData data = null;
            if (m_Current.tryGet(key, out data))
            {
                return data;
            }

            throw new Exception(this.throwInfo(function_name, key.ToString()));
        }

        public override bool readBool(int key)
        {
            return (bool)this.readAny(key, "readBool");
        }

        public override int readInt(int key)
        {
            return (int)this.readAny(key, "readInt");
        }

        public override float readFloat(int key)
        {
            return (float)this.readAny(key, "readFloat");
        }

        public override string readString(int key)
        {
            return (string)this.readAny(key, "readString");
        }

        public JsonType getType(int index)
        {
            return m_Current[index].GetJsonType();
        }

        public override bool readBool(string key)
        {
            return (bool)this.readAny(key, "readBool");
        }

        public override float readFloat(string key)
        {
            return (float)this.readAny(key, "readFloat");
        }

        public override int readInt(string key)
        {
            return (int)this.readAny(key, "readInt");
        }

        public override string readString(string key)
        {
            return (string)this.readAny(key, "readString");
        }

        public JsonType getType(string name)
        {
            return m_Current[name].GetJsonType();
        }

        protected override bool hasArray(int key)
        {
            JsonData data = null;
            if (m_Current.tryGet(key, out data))
            {
                return data.IsArray;
            }

            return false;
        }

        protected override void onBeginArray(int key)
        {
            var temp = m_Current[key];
            if (temp.IsArray)
            {
                m_PreRoot.Push(new Data(m_Current, key));
                m_Current = temp;
            }
            else
            {
                throw new System.ArgumentException(string.Format("This is not Array : {0}", key));
            }
        }

        protected override void onEndArray(int key)
        {
            if (!m_Current.IsArray)
            {
                throw new System.ArgumentException(string.Format("This End Array : {0}", key));
            }

            var data = m_PreRoot.Pop();
            m_Current = data.jsonData;
            data.close();
        }

        protected override bool hasArray(string key)
        {
            JsonData data = null;
            if (m_Current.tryGet(key, out data))
            {
                return data.IsArray;
            }

            return false;
        }

        protected override void onBeginArray(string key)
        {
            var temp = m_Current[key];
            if (temp.IsArray)
            {
                m_PreRoot.Push(new Data(m_Current, key));
                m_Current = temp;
            }
            else
            {
                throw new System.ArgumentException(string.Format("This is not Array : {0}", key));
            }
        }

        protected override void onEndArray(string key)
        {
            if (!m_Current.IsArray)
            {
                throw new System.ArgumentException(string.Format("Error End Array : {0}", key));
            }

            var data = m_PreRoot.Pop();
            m_Current = data.jsonData;
            data.close();
        }

        protected override bool hasObject(int key)
        {
            JsonData data = null;
            if (m_Current.tryGet(key, out data))
            {
                return data.IsObject;
            }

            return false;
        }

        protected override void onBeginObject(int key)
        {
            var temp = m_Current[key];
            if (temp.IsObject)
            {
                m_PreRoot.Push(new Data(m_Current, key));
                m_Current = temp;
            }
            else
            {
                throw new System.ArgumentException(string.Format("This is not Object : {0}", key));
            }
        }

        protected override void onEndObject(int key)
        {
            if (!m_Current.IsObject)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }


            var data = m_PreRoot.Pop();
            m_Current = data.jsonData;
            data.close();
        }

        protected override bool hasObject(string key)
        {
            JsonData data = null;
            if (m_Current.tryGet(key, out data))
            {
                return data.IsObject;
            }

            return false;
        }

        protected override void onBeginObject(string key)
        {
            var temp = m_Current[key];
            if (temp.IsObject)
            {
                m_PreRoot.Push(new Data(m_Current, key));
                m_Current = temp;
            }
            else
            {
                throw new System.ArgumentException(string.Format("This is not Object : {0}", key));
            }
        }

        protected override void onEndObject(string key)
        {
            if (!m_Current.IsObject)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }

            var data = m_PreRoot.Pop();
            m_Current = data.jsonData;
            data.close();
        }

        /// <summary>
        /// 尝试读取bool
        /// 失败则赋值为 false
        /// </summary>
        public override bool tryRead(int key, out bool result)
        {
            JsonData data = null;
            if (m_Current.tryGet(key, out data))
            {
                result = (bool)data;
                return true;
            }

            result = false;
            return false;
        }

        /// <summary>
        /// 尝试读取int
        /// 失败则赋值为 int.MinValue
        /// </summary>
        public override bool tryRead(int key, out int result)
        {
            JsonData data = null;
            if (m_Current.tryGet(key, out data))
            {
                result = (int)data;
                return true;
            }

            result = int.MinValue;
            return false;
        }

        /// <summary>
        /// 尝试读取float
        /// 失败则赋值为 float.MinValue
        /// </summary>
        public override bool tryRead(int key, out float result)
        {
            JsonData data = null;
            if (m_Current.tryGet(key, out data))
            {
                result = (float)data;
                return true;
            }

            result = float.MinValue;
            return false;
        }

        /// <summary>
        /// 尝试读取string
        /// 失败则赋值为 string.Empty
        /// </summary>
        public override bool tryRead(int key, out string result)
        {
            JsonData data = null;
            if (m_Current.tryGet(key, out data))
            {
                result = (string)data;
                return true;
            }

            result = string.Empty;
            return false;
        }

        /// <summary>
        /// 尝试读取bool
        /// 失败则赋值为 false
        /// </summary>
        public override bool tryRead(string key, out bool result)
        {
            JsonData data = null;
            if (m_Current.tryGet(key, out data))
            {
                result = (bool)data;
                return true;
            }

            result = false;
            return false;
        }

        /// <summary>
        /// 尝试读取int
        /// 失败则赋值为 int.MinValue
        /// </summary>
        public override bool tryRead(string key, out int result)
        {
            JsonData data = null;
            if (m_Current.tryGet(key, out data))
            {
                result = (int)data;
                return true;
            }

            result = int.MinValue;
            return false;
        }

        /// <summary>
        /// 尝试读取float
        /// 失败则赋值为 float.MinValue
        /// </summary>
        public override bool tryRead(string key, out float result)
        {
            JsonData data = null;
            if (m_Current.tryGet(key, out data))
            {
                result = (float)data;
                return true;
            }

            result = float.MinValue;
            return false;
        }

        /// <summary>
        /// 尝试读取string
        /// 失败则赋值为 string.Empty
        /// </summary>
        public override bool tryRead(string key, out string result)
        {
            JsonData data = null;
            if (m_Current.tryGet(key, out data))
            {
                result = (string)data;
                return true;
            }

            result = string.Empty;
            return false;
        }


        public override ICollection<string> getKeys()
        {
            if (!m_Current.IsObject)
            {
                throw new System.ArgumentException();
            }

            return m_Current.Keys;
        }
    }
}