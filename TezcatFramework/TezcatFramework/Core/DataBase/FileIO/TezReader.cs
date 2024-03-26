using System.Collections;
using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public abstract class TezReader
    {
        public enum ValueType
        {
            UnKown,
            Byte,
            Bool,
            Int,
            Float,
            String,
        }

        private Stack<string> mCheckStringKey = new Stack<string>();
        private Stack<int> mCheckIntKey = new Stack<int>();

        public abstract int count { get; }


        #region Array
        public bool tryBeginArray(string key)
        {
            if(this.hasArray(key))
            {
                this.beginArray(key);
                return true;
            }

            return false;
        }

        protected virtual bool hasArray(string key)
        {
            return true;
        }

        public void beginArray(string key)
        {
            mCheckStringKey.Push(key);
            this.onBeginArray(key);
        }

        protected abstract void onBeginArray(string key);

        public void endArray(string key)
        {
            if (mCheckStringKey.Peek() != key)
            {
                throw new System.ArgumentException(string.Format("Error End Array : {0}", key));
            }
            else
            {
                mCheckStringKey.Pop();
                this.onEndArray(key);
            }
        }

        protected abstract void onEndArray(string key);

        //======================================================//
        public bool tryBeginArray(int key)
        {
            if (this.hasArray(key))
            {
                this.beginArray(key);
                return true;
            }

            return false;
        }

        protected virtual bool hasArray(int key)
        {
            return true;
        }

        public void beginArray(int key)
        {
            mCheckIntKey.Push(key);
            this.onBeginArray(key);
        }

        protected abstract void onBeginArray(int key);

        public void endArray(int key)
        {
            if (mCheckIntKey.Peek() != key)
            {
                throw new System.ArgumentException(string.Format("Error End Array : {0}", key));
            }
            else
            {
                mCheckIntKey.Pop();
                this.onEndArray(key);
            }
        }

        protected abstract void onEndArray(int key);
        #endregion

        #region Object
        public bool tryBeginObject(string key)
        {
            if (this.hasObject(key))
            {
                this.beginObject(key);
                return true;
            }

            return false;
        }

        protected virtual bool hasObject(string key)
        {
            return true;
        }

        public void beginObject(string key)
        {
            mCheckStringKey.Push(key);
            this.onBeginObject(key);
        }

        protected abstract void onBeginObject(string key);

        public void endObject(string key)
        {
            if (mCheckStringKey.Peek() != key)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }
            else
            {
                mCheckStringKey.Pop();
                this.onEndObject(key);
            }
        }

        protected abstract void onEndObject(string key);

        //===============================================//
        public bool tryBeginObject(int key)
        {
            if (this.hasObject(key))
            {
                this.beginObject(key);
                return true;
            }

            return false;
        }

        protected virtual bool hasObject(int key)
        {
            return true;
        }

        public void beginObject(int key)
        {
            mCheckIntKey.Push(key);
            this.onBeginObject(key);
        }

        protected abstract void onBeginObject(int key);

        public void endObject(int key)
        {
            if (mCheckIntKey.Peek() != key)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }
            else
            {
                mCheckIntKey.Pop();
                this.onEndObject(key);
            }
        }

        protected abstract void onEndObject(int key);
        #endregion


        public abstract bool readBool(int key);
        public abstract int readInt(int key);
        public abstract float readFloat(int key);
        public abstract string readString(int key);

        public abstract bool readBool(string key);
        public abstract int readInt(string key);
        public abstract float readFloat(string key);
        public abstract string readString(string key);

        public abstract bool tryRead(int key, out bool result);
        public abstract bool tryRead(int key, out int result);
        public abstract bool tryRead(int key, out float result);
        public abstract bool tryRead(int key, out string result);

        public abstract bool tryRead(string key, out bool result);
        public abstract bool tryRead(string key, out int result);
        public abstract bool tryRead(string key, out float result);
        public abstract bool tryRead(string key, out string result);

        /// <summary>
        /// 获得当前层级下所有的Key
        /// </summary>
        public abstract ICollection<string> getKeys();
        public abstract ICollection getValues();

        public abstract ValueType getValueType(string key);
        public abstract ValueType getValueType(int index);

        public virtual void close()
        {
            mCheckStringKey.Clear();
            mCheckIntKey.Clear();

            mCheckIntKey = null;
            mCheckStringKey = null;
        }
    }


    public abstract class TezFileReader : TezReader
    {
        public abstract bool load(string path);
    }

    public abstract class TezReaderObject : ITezCloseable
    {
        public abstract bool readBool(int key);
        public abstract int readInt(int key);
        public abstract float readFloat(int key);
        public abstract string readString(int key);

        public abstract bool readBool(string key);
        public abstract int readInt(string key);
        public abstract float readFloat(string key);
        public abstract string readString(string key);

        public abstract bool tryRead(int key, out bool result);
        public abstract bool tryRead(int key, out int result);
        public abstract bool tryRead(int key, out float result);
        public abstract bool tryRead(int key, out string result);

        public abstract bool tryRead(string key, out bool result);
        public abstract bool tryRead(string key, out int result);
        public abstract bool tryRead(string key, out float result);
        public abstract bool tryRead(string key, out string result);

        public abstract void close();
    }
}