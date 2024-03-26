using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using LitJson;

namespace tezcat.Framework.Core
{
    public class TezJsonObjectReader : TezReader
    {
        private JsonData mCurrent;
        private Stack<JsonData> mPreRoot = new Stack<JsonData>();

        public override int count => mCurrent.Count;

        public TezJsonObjectReader(JsonData jsonData)
        {
            mCurrent = jsonData;
        }

        public override void close()
        {
            mCurrent.Clear();
            mCurrent = null;
        }

        private JsonData readAny(int key)
        {
            if (mCurrent.tryGet(key, out var data))
            {
                return data;
            }

            throw new Exception($"{MethodBase.GetCurrentMethod().Name}, {key}");
        }

        private JsonData readAny(string key)
        {
            if (mCurrent.tryGet(key, out var data))
            {
                return data;
            }

            throw new Exception($"{MethodBase.GetCurrentMethod().Name}, {key}");
        }

        public override bool readBool(int key)
        {
            return (bool)this.readAny(key);
        }

        public override bool readBool(string key)
        {
            return (bool)this.readAny(key);
        }

        public override float readFloat(int key)
        {
            return (float)this.readAny(key);
        }

        public override float readFloat(string key)
        {
            return (float)this.readAny(key);
        }

        public override int readInt(int key)
        {
            //            return (int)mCurrent[key];
            return (int)this.readAny(key);
        }

        public override int readInt(string key)
        {
//            return (int)mCurrent[key];
            return (int)this.readAny(key);
        }

        public override string readString(int key)
        {
            return (string)this.readAny(key);
        }

        public override string readString(string key)
        {
            return (string)this.readAny(key);
        }

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

        protected override void onBeginArray(string key)
        {
            var temp = mCurrent[key];
            if (temp.IsArray)
            {
                mPreRoot.Push(mCurrent);
                mCurrent = temp;
            }
        }

        protected override void onEndArray(string key)
        {
            mCurrent = mPreRoot.Pop();
        }

        protected override void onBeginArray(int key)
        {
            var temp = mCurrent[key];
            if (temp.IsArray)
            {
                mPreRoot.Push(mCurrent);
                mCurrent = temp;
            }
        }

        protected override void onEndArray(int key)
        {
            mCurrent = mPreRoot.Pop();
        }

        protected override void onBeginObject(string key)
        {
            var temp = mCurrent[key];
            if (temp.IsObject)
            {
                mPreRoot.Push(mCurrent);
                mCurrent = temp;
            }
        }

        protected override void onEndObject(string key)
        {
            mCurrent = mPreRoot.Pop();
        }

        protected override void onBeginObject(int key)
        {
            var temp = mCurrent[key];
            if (temp.IsObject)
            {
                mPreRoot.Push(mCurrent);
                mCurrent = temp;
            }
        }

        protected override void onEndObject(int key)
        {
            mCurrent = mPreRoot.Pop();
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