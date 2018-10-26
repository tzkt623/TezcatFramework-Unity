using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine.Assertions;

namespace tezcat.Framework.Core
{
    public interface ITezLog : ITezService
    {
        void info(string format);

        void info(string format, object obj);

        void info(string format, object obj1, object obj2);

        void info(string format, object obj1, object obj2, object obj3);

        void info(string format, params object[] objs);

        void assert(string format);

        void assert(string format, object obj);

        void assert(string format, object obj1, object obj2);

        void assert(string format, object obj1, object obj2, object obj3);

        void assert(string format, params object[] objs);
    }

    public enum TezDebugType
    {
        Info,
        Waring,
        Error,
        Assert
    }

    public class TezDebugInfo
    {
        public TezDebugType type;
        public string content;
    }

    public interface ITezDebug : ITezService
    {
        Queue<TezDebugInfo> infoQueue { get; }

        void clear();

        void info(string _title, object _object);

        void info(string _title, string _content);

        void info(string _class, string _method, string _content);

        void waring(string _title, object _object);

        void waring(string _title, string _content);

        void waring(string _class, string _method, string _content);

        void error(string _title, object _object);

        void error(string _title, string _content);

        void error(string _class, string _method, string _content);

        void isNotNull(object _object, string _title, string _content);

        void isNull(object _object, string _title, string _content);

        void isTrue(bool _condition, string _title, string _content);

        void isFalse(bool _condition, string _title, string _content);
    }

    /// <summary>
    /// 用法
    /// 
    /// #if UNITY_EDITOR
    /// TezDebug.do()
    /// #endif
    /// 
    /// </summary>
    public class TezDebug : ITezDebug
    {
        Stack<TezDebugInfo> m_FreePack = new Stack<TezDebugInfo>();
        public Queue<TezDebugInfo> infoQueue { get; private set; } = new Queue<TezDebugInfo>();

        Stack<string> m_Trace = new Stack<string>();
        StringBuilder m_Builder = new StringBuilder();
        public void checkQueue()
        {
            if (infoQueue.Count > 99)
            {
                m_FreePack.Push(infoQueue.Dequeue());
            }
        }

        private string trace(string content)
        {
            m_Builder.Clear();
            m_Builder.AppendLine(content);
            StackTrace st = new StackTrace(1, true);
            var frames = st.GetFrames();
            for (int i = 0; i < st.FrameCount; i++)
            {
                var frame = frames[i];
                m_Builder.AppendLine(string.Format(
                    "{0}>{1}>{2}",
                    frame.GetFileName(),
                    frame.GetMethod(),
                    frame.GetFileLineNumber()));
            }

            return m_Builder.ToString();
        }

        TezDebugInfo create(TezDebugType type, string content)
        {
            if (m_FreePack.Count > 0)
            {
                var temp = m_FreePack.Pop();
                temp.type = type;
                temp.content = content;
                return temp;
            }
            else
            {
                return new TezDebugInfo() { type = type, content = content };
            }
        }

        public void info(string _title, object _object)
        {
            infoQueue.Enqueue(create(TezDebugType.Info, "[" + _title + "] : " + _object));
            checkQueue();
        }

        public void clear()
        {
            infoQueue.Clear();
        }

        public void info(string _title, string _content)
        {
            infoQueue.Enqueue(create(TezDebugType.Info, "[" + _title + "] : " + _content));
            checkQueue();
        }

        public void info(string _class, string _method, string _content)
        {
            infoQueue.Enqueue(create(TezDebugType.Info, "[" + _class + "](" + _method + ") : " + _content));
            checkQueue();
        }

        public void waring(string _title, object _object)
        {
            infoQueue.Enqueue(create(TezDebugType.Waring, "[" + _title + "] : " + _object));
            checkQueue();
        }

        public void waring(string _title, string _content)
        {
            infoQueue.Enqueue(create(TezDebugType.Waring, "[" + _title + "] : " + _content));
            checkQueue();
        }

        public void waring(string _class, string _method, string _content)
        {
            infoQueue.Enqueue(create(TezDebugType.Waring, "[" + _class + "](" + _method + ") : " + _content));
            checkQueue();
        }

        public void error(string _title, object _object)
        {
            infoQueue.Enqueue(create(TezDebugType.Error, "[" + _title + "] : " + _object));
            checkQueue();
            Assert.IsTrue(false);
        }

        public void error(string _title, string _content)
        {
            infoQueue.Enqueue(create(TezDebugType.Error, "[" + _title + "] : " + _content));
            checkQueue();
            Assert.IsTrue(false);
        }

        public void error(string _class, string _method, string _content)
        {
            infoQueue.Enqueue(create(TezDebugType.Error, "[" + _class + "](" + _method + ") : " + _content));
            checkQueue();
            Assert.IsTrue(false);
        }

        public void isNotNull(object _object, string _title, string _content)
        {
            if (_object == null)
            {
                infoQueue.Enqueue(create(TezDebugType.Assert, "[" + _title + "] : " + _content));
                checkQueue();
                Assert.IsTrue(false);
            }
        }

        public void isNull(object _object, string _title, string _content)
        {
            if (_object != null)
            {
                infoQueue.Enqueue(create(TezDebugType.Assert, "[" + _title + "] : " + _content));
                checkQueue();
                Assert.IsTrue(false);
            }
        }

        public void isTrue(bool _condition, string _title, string _content)
        {
            if (!_condition)
            {
                infoQueue.Enqueue(create(TezDebugType.Assert, "[" + _title + "] : " + _content));
                checkQueue();
                Assert.IsTrue(false);
            }
        }

        public void isFalse(bool _condition, string _title, string _content)
        {
            if (_condition)
            {
                infoQueue.Enqueue(create(TezDebugType.Assert, "[" + _title + "] : " + _content));
                checkQueue();
                Assert.IsTrue(false);
            }
        }

        public void close()
        {

        }
    }
}

