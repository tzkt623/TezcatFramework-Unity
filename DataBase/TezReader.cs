using System.Collections.Generic;

namespace tezcat.Framework.DataBase
{
    public abstract class TezReader
    {
        private Stack<string> m_CheckStringKey = new Stack<string>();
        private Stack<int> m_CheckIntKey = new Stack<int>();

        public abstract int count { get; }

        public abstract bool load(string path);

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
            m_CheckStringKey.Push(key);
            this.onBeginArray(key);
        }

        protected abstract void onBeginArray(string key);

        public void endArray(string key)
        {
            if (m_CheckStringKey.Peek() != key)
            {
                throw new System.ArgumentException(string.Format("Error End Array : {0}", key));
            }
            else
            {
                m_CheckStringKey.Pop();
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
            m_CheckIntKey.Push(key);
            this.onBeginArray(key);
        }

        protected abstract void onBeginArray(int key);

        public void endArray(int key)
        {
            if (m_CheckIntKey.Peek() != key)
            {
                throw new System.ArgumentException(string.Format("Error End Array : {0}", key));
            }
            else
            {
                m_CheckIntKey.Pop();
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
            m_CheckStringKey.Push(key);
            this.onBeginObject(key);
        }

        protected abstract void onBeginObject(string key);

        public void endObject(string key)
        {
            if (m_CheckStringKey.Peek() != key)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }
            else
            {
                m_CheckStringKey.Pop();
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
            m_CheckIntKey.Push(key);
            this.onBeginObject(key);
        }

        protected abstract void onBeginObject(int key);

        public void endObject(int key)
        {
            if (m_CheckIntKey.Peek() != key)
            {
                throw new System.ArgumentException(string.Format("Error End Object : {0}", key));
            }
            else
            {
                m_CheckIntKey.Pop();
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

        public abstract ICollection<string> getKeys();

        public void close()
        {
            m_CheckStringKey.Clear();
            m_CheckIntKey.Clear();

            m_CheckIntKey = null;
            m_CheckStringKey = null;
        }
    }
}