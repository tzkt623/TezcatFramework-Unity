using System.Collections.Generic;

namespace tezcat.Serialization
{
    public abstract class TezWriter
    {
        private Stack<string> m_CheckStringKey = new Stack<string>();
        private Stack<int> m_CheckIntKey = new Stack<int>();

        public abstract void save(string path);

        #region Array
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


        //===============================================//


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

        public abstract void write(bool value);
        public abstract void write(int value);
        public abstract void write(float value);
        public abstract void write(string value);

        public abstract void write(string name, bool value);
        public abstract void write(string name, int value);
        public abstract void write(string name, float value);
        public abstract void write(string name, string value);

        public void close()
        {
            m_CheckStringKey.Clear();
            m_CheckIntKey.Clear();

            m_CheckIntKey = null;
            m_CheckStringKey = null;
        }
    }
}

