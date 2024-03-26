using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace tezcat.Framework.Core
{
    public class TezJsonWriter : TezWriter
    {
        private JsonData mRoot = null;
        private Stack<JsonData> mPreRoot = new Stack<JsonData>();

        public TezJsonWriter(bool array = false)
        {
            mRoot = new JsonData();
            mRoot.SetJsonType(array ? JsonType.Array : JsonType.Object);
        }

        public override void save(string path)
        {
            if(mPreRoot.Count > 0)
            {
                throw new Exception(mPreRoot.Count.ToString());
            }
            File.WriteAllText(path, JsonMapper.ToJson(mRoot), Encoding.UTF8);
            mRoot.Clear();
            this.clear();
        }

        public override string ToString()
        {
            return mRoot.ToJson();
        }

        #region Object
        protected override void onBeginObject(int key)
        {
            mPreRoot.Push(mRoot);
            while (mRoot.Count <= key)
            {
                mRoot.Add(new JsonData());
            }

            mRoot = mRoot[key];
            mRoot.SetJsonType(JsonType.Object);
        }

        protected override void onEndObject(int key)
        {
            if (!mRoot.IsObject)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }

            mRoot = mPreRoot.Pop();
        }

        protected override void onBeginObject(string key)
        {
            mPreRoot.Push(mRoot);
            if (!mRoot.contains(key))
            {
                var data = new JsonData();
                data.SetJsonType(JsonType.Object);
                mRoot[key] = data;
            }

            mRoot = mRoot[key];
        }

        protected override void onEndObject(string key)
        {
            if (!mRoot.IsObject)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }

            mRoot = mPreRoot.Pop();
        }


        public override void write(string name, bool value)
        {
            mRoot[name] = value;
        }

        public override void write(string name, int value)
        {
            mRoot[name] = value;
        }

        public override void write(string name, float value)
        {
            mRoot[name] = value;
        }

        public override void write(string name, string value)
        {
            mRoot[name] = value;
        }
        #endregion

        #region Array
        protected override void onBeginArray(int key)
        {
            mPreRoot.Push(mRoot);

            while (mRoot.Count <= key)
            {
                mRoot.Add(new JsonData());
            }

            mRoot = mRoot[key];
            mRoot.SetJsonType(JsonType.Array);
        }

        protected override void onEndArray(int key)
        {
            if (!mRoot.IsArray)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }

            mRoot = mPreRoot.Pop();
        }

        protected override void onBeginArray(string key)
        {
            mPreRoot.Push(mRoot);
            if (!mRoot.contains(key))
            {
                var data = new JsonData();
                data.SetJsonType(JsonType.Array);
                mRoot[key] = data;
            }
            mRoot = mRoot[key];
        }

        protected override void onEndArray(string key)
        {
            if (!mRoot.IsArray)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }

            mRoot = mPreRoot.Pop();
        }

        /// <summary>
        /// 添加一个数组字段
        /// </summary>
        /// <param name="value"></param>
        public override void write(bool value)
        {
            mRoot.Add(value);
        }

        /// <summary>
        /// 添加一个数组字段
        /// </summary>
        /// <param name="value"></param>
        public override void write(int value)
        {
            mRoot.Add(value);
        }

        /// <summary>
        /// 添加一个数组字段
        /// </summary>
        /// <param name="value"></param>
        public override void write(float value)
        {
            mRoot.Add(value);
        }

        /// <summary>
        /// 添加一个数组字段
        /// </summary>
        /// <param name="value"></param>
        public override void write(string value)
        {
            mRoot.Add(value);
        }
        #endregion
    }
}