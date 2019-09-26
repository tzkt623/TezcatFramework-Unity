using LitJson;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace tezcat.Framework.Database
{
    public class TezJsonWriter : TezWriter
    {
        private JsonData m_Root = null;
        private Stack<JsonData> m_PreRoot = new Stack<JsonData>();

        public TezJsonWriter(bool array = false)
        {
            m_Root = new JsonData();
            m_Root.SetJsonType(array ? JsonType.Array : JsonType.Object);
        }

        public override void save(string path)
        {
            File.WriteAllText(path, JsonMapper.ToJson(m_Root), Encoding.ASCII);
        }

        public TezJsonWriter()
        {

        }

        public override string ToString()
        {
            return m_Root.ToJson();
        }

        #region Object
        protected override void onBeginObject(int key)
        {
            m_PreRoot.Push(m_Root);
            if (m_Root.Count > key)
            {
                m_Root = m_Root[key];
            }
            else
            {
                while (m_Root.Count <= key)
                {
                    m_Root.Add(new JsonData());
                }

                m_Root[key].SetJsonType(JsonType.Object);
                m_Root = m_Root[key];
            }
        }

        protected override void onEndObject(int key)
        {
            if (!m_Root.IsObject)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }

            m_Root = m_PreRoot.Pop();
        }

        protected override void onBeginObject(string key)
        {
            m_PreRoot.Push(m_Root);
            if (m_Root.Keys.Contains(key))
            {
                m_Root = m_Root[key];
            }
            else
            {
                var data = new JsonData();
                data.SetJsonType(JsonType.Object);
                m_Root[key] = data;

                m_Root = data;
            }
        }

        protected override void onEndObject(string key)
        {
            if (!m_Root.IsObject)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }

            m_Root = m_PreRoot.Pop();
        }


        public override void write(string name, bool value)
        {
            m_Root[name] = value;
        }

        public override void write(string name, int value)
        {
            m_Root[name] = value;
        }

        public override void write(string name, float value)
        {
            m_Root[name] = value;
        }

        public override void write(string name, string value)
        {
            m_Root[name] = value;
        }
        #endregion

        #region Array
        protected override void onBeginArray(int key)
        {
            m_PreRoot.Push(m_Root);

            if (m_Root.Count > key)
            {
                m_Root = m_Root[key];
            }
            else
            {
                while (m_Root.Count <= key)
                {
                    m_Root.Add(new JsonData());
                }

                m_Root[key].SetJsonType(JsonType.Array);

                m_Root = m_Root[key];
            }
        }

        protected override void onEndArray(int key)
        {
            if (!m_Root.IsArray)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }

            m_Root = m_PreRoot.Pop();
        }

        protected override void onBeginArray(string key)
        {
            m_PreRoot.Push(m_Root);
            if (m_Root.Keys.Contains(key))
            {
                m_Root = m_Root[key];
            }
            else
            {
                var data = new JsonData();
                data.SetJsonType(JsonType.Array);
                m_Root[key] = data;

                m_Root = data;
            }
        }

        protected override void onEndArray(string key)
        {
            if (!m_Root.IsArray)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }

            m_Root = m_PreRoot.Pop();
        }

        /// <summary>
        /// 添加一个数组字段
        /// </summary>
        /// <param name="value"></param>
        public override void write(bool value)
        {
            m_Root.Add(value);
        }

        /// <summary>
        /// 添加一个数组字段
        /// </summary>
        /// <param name="value"></param>
        public override void write(int value)
        {
            m_Root.Add(value);
        }

        /// <summary>
        /// 添加一个数组字段
        /// </summary>
        /// <param name="value"></param>
        public override void write(float value)
        {
            m_Root.Add(value);
        }

        /// <summary>
        /// 添加一个数组字段
        /// </summary>
        /// <param name="value"></param>
        public override void write(string value)
        {
            m_Root.Add(value);
        }
        #endregion
    }
}