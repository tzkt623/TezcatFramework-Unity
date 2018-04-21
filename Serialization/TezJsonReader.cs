using LitJson;
using System.Collections.Generic;
using System.IO;

namespace tezcat.Serialization
{
    public class TezJsonReader : TezReader
    {
        private JsonData m_Current = null;
        private Stack<JsonData> m_PreRoot = new Stack<JsonData>();

        public ICollection<string> keys
        {
            get { return m_Current.Keys; }
        }

        public override int count
        {
            get { return m_Current.Count; }
        }

        public override bool load(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            m_PreRoot.Clear();
            m_Current = JsonMapper.ToObject(File.ReadAllText(path));
            return true;
        }

        public override bool readBool(int key)
        {
            return (bool)m_Current[key];
        }

        public override int readInt(int key)
        {
            return (int)m_Current[key];
        }

        public override float readFloat(int key)
        {
            return (float)m_Current[key];
        }

        public override string readString(int key)
        {
            return (string)m_Current[key];
        }

        public JsonType getType(int index)
        {
            return m_Current[index].GetJsonType();
        }

        public override bool readBool(string key)
        {
            return (bool)m_Current[key];
        }

        public override float readFloat(string key)
        {
            return (float)m_Current[key];
        }

        public override int readInt(string key)
        {
            return (int)m_Current[key];
        }

        public override string readString(string key)
        {
            return (string)m_Current[key];
        }

        public JsonType getType(string name)
        {
            return m_Current[name].GetJsonType();
        }

        protected override void onBeginArray(int key)
        {
            var temp = m_Current[key];
            if(temp.IsArray)
            {
                m_PreRoot.Push(m_Current);
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

            m_Current = m_PreRoot.Pop();
        }

        protected override void onBeginArray(string key)
        {
            var temp = m_Current[key];
            if(temp.IsArray)
            {
                m_PreRoot.Push(m_Current);
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

            m_Current = m_PreRoot.Pop();
        }


        protected override void onBeginObject(int key)
        {
            var temp = m_Current[key];
            if (temp.IsObject)
            {
                m_PreRoot.Push(m_Current);
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

            m_Current = m_PreRoot.Pop();
        }


        protected override void onBeginObject(string key)
        {
            var temp = m_Current[key];
            if (temp.IsObject)
            {
                m_PreRoot.Push(m_Current);
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

            m_Current = m_PreRoot.Pop();
        }

        public override bool tryRead(int key, out bool result)
        {
            if(m_Current.IsArray && m_Current.Count > key)
            {
                result = (bool)m_Current[key];
                return true;
            }

            result = false;
            return false;
        }

        public override bool tryRead(int key, out int result)
        {
            if (m_Current.IsArray && m_Current.Count > key)
            {
                result = (int)m_Current[key];
                return true;
            }

            result = int.MinValue;
            return false;
        }

        public override bool tryRead(int key, out float result)
        {
            if (m_Current.IsArray && m_Current.Count > key)
            {
                result = (float)m_Current[key];
                return true;
            }

            result = float.MinValue;
            return false;
        }

        public override bool tryRead(int key, out string result)
        {
            if (m_Current.IsArray && m_Current.Count > key)
            {
                result = (string)m_Current[key];
                return true;
            }

            result = null;
            return false;
        }

        public override bool tryRead(string key, out bool result)
        {
            JsonData data = null;
            if (m_Current.IsObject && m_Current.tryGet(key, out data))
            {
                result = (bool)data;
                return true;
            }

            result = false;
            return false;
        }

        public override bool tryRead(string key, out int result)
        {
            JsonData data = null;
            if (m_Current.IsObject && m_Current.tryGet(key, out data))
            {
                result = (int)data;
                return true;
            }

            result = int.MinValue;
            return false;
        }

        public override bool tryRead(string key, out float result)
        {
            JsonData data = null;
            if (m_Current.IsObject && m_Current.tryGet(key, out data))
            {
                result = (float)data;
                return true;
            }

            result = float.MinValue;
            return false;
        }

        public override bool tryRead(string key, out string result)
        {
            JsonData data = null;
            if (m_Current.IsObject && m_Current.tryGet(key, out data))
            {
                result = (string)data;
                return true;
            }

            result = null;
            return false;
        }
    }
}