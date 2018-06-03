using System.Collections.Generic;
using UnityEngine.Assertions;

namespace tezcat.Debug
{
    /// <summary>
    /// 用法
    /// 
    /// #if UNITY_EDITOR
    /// TezDebug.do()
    /// #endif
    /// 
    /// </summary>
    public static class TezDebug
    {
        public enum Type
        {
            Info,
            Waring,
            Error,
            Assert
        }

        public class Pack
        {
            public Type type;
            public string content;
        }

        static Stack<Pack> m_FreePack = new Stack<Pack>();

        static Queue<Pack> m_InfoList = new Queue<Pack>();
        public static Queue<Pack> infoList
        {
            get
            {
                return m_InfoList;
            }
        }

        public static void checkQueue()
        {
            if (m_InfoList.Count > 99)
            {
                m_FreePack.Push(m_InfoList.Dequeue());
            }
        }

        static Pack create(Type type, string content)
        {
            if(m_FreePack.Count > 0)
            {
                return m_FreePack.Pop();
            }
            else
            {
                return new Pack() { type = type, content = content };
            }
        }

        public static void info(string _title, object _object)
        {
            m_InfoList.Enqueue(create(Type.Info, "[" + _title + "] : " + _object));
            checkQueue();
        }

        public static void clear()
        {
            m_InfoList.Clear();
        }

        public static void info(string _title, string _content)
        {
            m_InfoList.Enqueue(create(Type.Info, "[" + _title + "] : " + _content));
            checkQueue();
        }

        public static void info(string _class, string _method, string _content)
        {
            m_InfoList.Enqueue(create(Type.Info, "[" + _class + "](" + _method + ") : " + _content));
            checkQueue();
        }

        public static void waring(string _title, object _object)
        {
            m_InfoList.Enqueue(create(Type.Waring, "[" + _title + "] : " + _object));
            checkQueue();
        }

        public static void waring(string _title, string _content)
        {
            m_InfoList.Enqueue(create(Type.Waring, "[" + _title + "] : " + _content));
            checkQueue();
        }

        public static void waring(string _class, string _method, string _content)
        {
            m_InfoList.Enqueue(create(Type.Waring, "[" + _class + "](" + _method + ") : " + _content));
            checkQueue();
        }

        public static void error(string _title, object _object)
        {
            m_InfoList.Enqueue(create(Type.Error, "[" + _title + "] : " + _object));
            checkQueue();
            Assert.IsTrue(false);
        }

        public static void error(string _title, string _content)
        {
            m_InfoList.Enqueue(create(Type.Error, "[" + _title + "] : " + _content));
            checkQueue();
            Assert.IsTrue(false);
        }

        public static void error(string _class, string _method, string _content)
        {
            m_InfoList.Enqueue(create(Type.Error, "[" + _class + "](" + _method + ") : " + _content));
            checkQueue();
            Assert.IsTrue(false);
        }

        public static void isNotNull(object _object, string _title, string _content)
        {
            if(_object == null)
            {
                m_InfoList.Enqueue(create(Type.Assert, "[" + _title + "] : " + _content));
                checkQueue();
                Assert.IsTrue(false);
            }
        }

        public static void isNull(object _object, string _title, string _content)
        {
            if (_object != null)
            {
                m_InfoList.Enqueue(create(Type.Assert, "[" + _title + "] : " + _content));
                checkQueue();
                Assert.IsTrue(false);
            }
        }

        public static void isTrue(bool _condition, string _title, string _content)
        {
            if (!_condition)
            {
                m_InfoList.Enqueue(create(Type.Assert, "[" + _title + "] : " + _content));
                checkQueue();
                Assert.IsTrue(false);
            }
        }

        public static void isFalse(bool _condition, string _title, string _content)
        {
            if (_condition)
            {
                m_InfoList.Enqueue(create(Type.Assert, "[" + _title + "] : " + _content));
                checkQueue();
                Assert.IsTrue(false);
            }
        }
    }
}

