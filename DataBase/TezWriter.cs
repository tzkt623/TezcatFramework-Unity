using System.Collections.Generic;

namespace tezcat.Framework.Database
{
    public abstract class TezWriter
    {
        class Checker
        {
            public string name = null;
            public int index = -1;
            public System.Type type = null;
        }
        private Stack<Checker> m_Checker = new Stack<Checker>();

        public abstract void save(string path);

#if false
        public abstract class DataGroup
        {
            private Stack<string> m_CheckStringKey = null;
            private Stack<int> m_CheckIntKey = null;

            public DataGroup(TezWriter writer)
            {
                m_CheckStringKey = writer.m_CheckStringKey;
                m_CheckIntKey = writer.m_CheckIntKey;
            }

            public void begin(string key)
            {
                m_CheckStringKey.Push(key);
                this.onBegin(key);
            }

            protected abstract void onBegin(string key);

            public void end(string key)
            {
                if (m_CheckStringKey.Peek() != key)
                {
                    throw new System.ArgumentException(string.Format("Error End Array : {0}", key));
                }
                else
                {
                    m_CheckStringKey.Pop();
                    this.onEnd(key);
                }
            }

            protected abstract void onEnd(string key);
            //===============================================//


            //===============================================//
            public void begin(int key)
            {
                m_CheckIntKey.Push(key);
                this.onBegin(key);
            }

            protected abstract void onBegin(int key);

            public void end(int key)
            {
                if (m_CheckIntKey.Peek() != key)
                {
                    throw new System.ArgumentException(string.Format("Error End Array : {0}", key));
                }
                else
                {
                    m_CheckIntKey.Pop();
                    this.onEnd(key);
                }
            }

            protected abstract void onEnd(int key);
        }

        public abstract DataGroup myObject { get; }
        public abstract DataGroup myArray { get; }
        public abstract DataGroup myDic { get; }
#endif

        #region Array
        public void beginArray(string key)
        {
            m_Checker.Push(new Checker() { name = key });
            this.onBeginArray(key);
        }

        protected abstract void onBeginArray(string key);

        public void endArray(string key)
        {
            var checker = m_Checker.Pop();
            if (!string.IsNullOrEmpty(key) && checker.name == key)
            {
                this.onEndArray(checker.name);
            }
            else
            {
                throw new System.ArgumentException(string.Format("Error End Array : Current:{0} | Save:{1}", key, checker.name));
            }
        }

        protected abstract void onEndArray(string key);


        //===============================================//


        public void beginArray(int key)
        {
            m_Checker.Push(new Checker() { index = key });
            this.onBeginArray(key);
        }

        protected abstract void onBeginArray(int key);

        public void endArray(int key)
        {
            var checker = m_Checker.Pop();
            if (checker.index == key)
            {
                this.onEndArray(checker.index);
            }
            else
            {
                throw new System.ArgumentException(string.Format("Error End Array : Current:{0} | Save:{1}", key, checker.index));
            }
        }

        protected abstract void onEndArray(int key);
        #endregion

        #region Object
        public void beginObject(string key)
        {
            m_Checker.Push(new Checker() { name = key });
            this.onBeginObject(key);
        }

        protected abstract void onBeginObject(string key);

        public void endObject(string key)
        {
            var checker = m_Checker.Pop();
            if (string.IsNullOrEmpty(key) && checker.name == key)
            {
                this.onEndObject(checker.name);
            }
            else
            {
                throw new System.ArgumentException(string.Format("Error End Object : Current:{0} | Save:{1}", key, checker.name));
            }
        }

        protected abstract void onEndObject(string key);

        //===============================================//

        public void beginObject(int key)
        {
            m_Checker.Push(new Checker() { index = key });
            this.onBeginObject(key);
        }

        protected abstract void onBeginObject(int key);

        public void endObject(int key)
        {
            var checker = m_Checker.Pop();
            if (checker.index == key)
            {
                this.onEndObject(checker.index);
            }
            else
            {
                throw new System.ArgumentException(string.Format("Error End Object : Current:{0} | Save:{1}", key, checker.index));
            }
        }

        protected abstract void onEndObject(int key);
        #endregion

        /*
         * 以固定格式写入数据 
         */
        public abstract void write(bool value);
        public abstract void write(int value);
        public abstract void write(float value);
        public abstract void write(string value);


        /*
         * 以键值对的方式写入数据
         */
        public abstract void write(string name, bool value);
        public abstract void write(string name, int value);
        public abstract void write(string name, float value);
        public abstract void write(string name, string value);

        public void close()
        {
            m_Checker.Clear();
            m_Checker = null;
        }


//         Dictionary<System.Type, CustomWriter> m_Dic = new Dictionary<System.Type, CustomWriter>();
// 
//         public class CustomWriter
//         {
// 
//         }
// 
//         public class CustomWriter<T> : CustomWriter
//         {
//             public delegate void Writer(string name, T value);
//             public Writer writer { get; }
// 
//             public CustomWriter(Writer writer)
//             {
//                 this.writer = writer;
//             }
//         }
// 
//         public void register<T>(CustomWriter<T>.Writer writer) where T : CustomWriter, new()
//         {
//             m_Dic[typeof(T)] = new CustomWriter<T>(writer);
//         }
// 
//         public void write<T>(string name, T value) where T : ITezSerializable
//         {
//             this.write("type", typeof(T).Name);
//             ITezSerializable data = value as ITezSerializable;
//             data.serialize(this);
//         }
    }
}

