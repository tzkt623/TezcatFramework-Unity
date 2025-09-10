using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LitJson;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// Json数据读取器
    /// </summary>
    public class TezJsonReader : TezFileReader
    {
        /// <summary>
        /// 用于抛出错误
        /// </summary>
        private class Data : ITezCloseable
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

        private JsonData mCurrent = null;
        private JsonData mRootObject = null;
        private Stack<Data> mPreRoot = new Stack<Data>();
        private string mPath;

        public ICollection<string> keys
        {
            get { return mCurrent.Keys; }
        }

        public override int count
        {
            get { return mCurrent.Count; }
        }

        protected override void onClose()
        {
            base.onClose();

            foreach (var item in mPreRoot)
            {
                item.close();
            }
            mPreRoot.Clear();
            mRootObject.Clear();

            mRootObject = null;
            mCurrent = null;
            mPreRoot = null;
        }

        public bool loadContent(string jsonString)
        {
            var temp = JsonMapper.ToObject(jsonString);
            if (temp.IsArray || temp.IsObject)
            {
                mCurrent = temp;
                mRootObject = temp;
                return true;
            }

            mCurrent = null;
            mRootObject = null;
            return false;
        }

        public override bool load(string path)
        {
            if (!File.Exists(path))
            {
                return false;
            }

            mPath = path;
            bool result = false;
            foreach (var item in mPreRoot)
            {
                item.close();
            }
            mPreRoot.Clear();
            string content = null;
            try
            {
                content = File.ReadAllText(mPath);
                result = true;
            }
            catch(Exception e)
            {
                result = false;
            }

            if (result)
            {
                result = this.loadContent(content);
            }
            return result;
        }

        private string throwInfo(string functionName, string position)
        {
            StringBuilder content = new StringBuilder();
            content.AppendLine($"{functionName}:{position} Error");
            if (mPreRoot.Count > 0)
            {
                content.AppendLine("From This Path:");
                foreach (var data in mPreRoot)
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
            content.AppendLine(mPath);

            return content.ToString();
        }

        private JsonData readAny(int key, string functionName)
        {
            JsonData data = null;
            if (mCurrent.tryGet(key, out data))
            {
                return data;
            }

            throw new Exception(this.throwInfo(functionName, key.ToString()));
        }

        private JsonData readAny(string key, string functionName)
        {
            JsonData data = null;
            if (mCurrent.tryGet(key, out data))
            {
                return data;
            }

            throw new Exception(this.throwInfo(functionName, key.ToString()));
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
            return mCurrent[index].GetJsonType();
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
            return mCurrent[name].GetJsonType();
        }

        protected override bool hasArray(int key)
        {
            JsonData data = null;
            if (mCurrent.tryGet(key, out data))
            {
                return data.IsArray;
            }

            return false;
        }

        protected override void onBeginArray(int key)
        {
            var temp = mCurrent[key];
            if (temp.IsArray)
            {
                mPreRoot.Push(new Data(mCurrent, key));
                mCurrent = temp;
            }
            else
            {
                throw new System.ArgumentException(string.Format("This is not Array : {0}", key));
            }
        }

        protected override void onEndArray(int key)
        {
            if (!mCurrent.IsArray)
            {
                throw new System.ArgumentException(string.Format("This End Array : {0}", key));
            }

            var data = mPreRoot.Pop();
            mCurrent = data.jsonData;
            data.close();
        }

        protected override bool hasArray(string key)
        {
            JsonData data = null;
            if (mCurrent.tryGet(key, out data))
            {
                return data.IsArray;
            }

            return false;
        }

        protected override void onBeginArray(string key)
        {
            var temp = mCurrent[key];
            if (temp.IsArray)
            {
                mPreRoot.Push(new Data(mCurrent, key));
                mCurrent = temp;
            }
            else
            {
                throw new System.ArgumentException(string.Format("This is not Array : {0}", key));
            }
        }

        protected override void onEndArray(string key)
        {
            if (!mCurrent.IsArray)
            {
                throw new System.ArgumentException(string.Format("Error End Array : {0}", key));
            }

            var data = mPreRoot.Pop();
            mCurrent = data.jsonData;
            data.close();
        }

        protected override bool hasObject(int key)
        {
            JsonData data = null;
            if (mCurrent.tryGet(key, out data))
            {
                return data.IsObject;
            }

            return false;
        }

        protected override void onBeginObject(int key)
        {
            var temp = mCurrent[key];
            if (temp.IsObject)
            {
                mPreRoot.Push(new Data(mCurrent, key));
                mCurrent = temp;
            }
            else
            {
                throw new System.ArgumentException(string.Format("This is not Object : {0}", key));
            }
        }

        protected override void onEndObject(int key)
        {
            if (!mCurrent.IsObject)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }


            var data = mPreRoot.Pop();
            mCurrent = data.jsonData;
            data.close();
        }

        protected override bool hasObject(string key)
        {
            JsonData data = null;
            if (mCurrent.tryGet(key, out data))
            {
                return data.IsObject;
            }

            return false;
        }

        protected override void onBeginObject(string key)
        {
            var temp = mCurrent[key];
            if (temp.IsObject)
            {
                mPreRoot.Push(new Data(mCurrent, key));
                mCurrent = temp;
            }
            else
            {
                throw new System.ArgumentException(string.Format("This is not Object : {0}", key));
            }
        }

        protected override void onEndObject(string key)
        {
            if (!mCurrent.IsObject)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }

            var data = mPreRoot.Pop();
            mCurrent = data.jsonData;
            data.close();
        }

        /// <summary>
        /// 尝试读取bool
        /// 失败则赋值为 false
        /// </summary>
        public override bool tryRead(int key, out bool result)
        {
            JsonData data = null;
            if (mCurrent.tryGet(key, out data))
            {
                result = (bool)data;
                return true;
            }

            result = false;
            return false;
        }

        /// <summary>
        /// 尝试读取int
        /// 失败则赋值为 init.MinValue
        /// </summary>
        public override bool tryRead(int key, out int result)
        {
            JsonData data = null;
            if (mCurrent.tryGet(key, out data))
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
            if (mCurrent.tryGet(key, out data))
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
            if (mCurrent.tryGet(key, out data))
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
            if (mCurrent.tryGet(key, out data))
            {
                result = (bool)data;
                return true;
            }

            result = false;
            return false;
        }

        /// <summary>
        /// 尝试读取int
        /// 失败则赋值为 init.MinValue
        /// </summary>
        public override bool tryRead(string key, out int result)
        {
            JsonData data = null;
            if (mCurrent.tryGet(key, out data))
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
            if (mCurrent.tryGet(key, out data))
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
            if (mCurrent.tryGet(key, out data))
            {
                result = (string)data;
                return true;
            }

            result = string.Empty;
            return false;
        }


        public override ICollection<string> getKeys()
        {
            if (!mCurrent.IsObject)
            {
                throw new System.ArgumentException();
            }

            return mCurrent.Keys;
        }

        public override ICollection getValues()
        {
            if (!mCurrent.IsArray)
            {
                throw new System.ArgumentException();
            }

            return mCurrent;
        }

        public override ValueType getValueType(int index)
        {
            if (mCurrent.tryGet(index, out JsonData data))
            {
                switch (data.GetJsonType())
                {
                    case JsonType.None:
                        break;
                    case JsonType.Object:
                        break;
                    case JsonType.Array:
                        break;
                    case JsonType.String:
                        return ValueType.String;
                    case JsonType.Int:
                        return ValueType.Int;
                    case JsonType.Long:
                        return ValueType.Int;
                    case JsonType.Double:
                        return ValueType.Float;
                    case JsonType.Boolean:
                        return ValueType.Bool;
                    default:
                        break;
                }
            }

            throw new Exception();
        }

        public override ValueType getValueType(string key)
        {
            if (mCurrent.tryGet(key, out JsonData data))
            {
                switch (data.GetJsonType())
                {
                    case JsonType.None:
                        break;
                    case JsonType.Object:
                        break;
                    case JsonType.Array:
                        break;
                    case JsonType.String:
                        return ValueType.String;
                    case JsonType.Int:
                        return ValueType.Int;
                    case JsonType.Long:
                        return ValueType.Int;
                    case JsonType.Double:
                        return ValueType.Float;
                    case JsonType.Boolean:
                        return ValueType.Bool;
                    default:
                        break;
                }
            }

            throw new Exception();
        }
    }
}