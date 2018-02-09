using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace tezcat
{
    public static class TezDebug
    {
        public static void info(string _title, string _content)
        {
            Debug.Log("[" + _title + "] : " + _content);
        }

        public static void info(string _class, string _method, string _content)
        {
            Debug.Log("[" + _class + "](" + _method + ") : " + _content);
        }

        public static void waring(string title, string content)
        {
            Debug.LogWarning("[" + title + "] : " + content);
        }

        public static void waring(string _class, string _method, string _content)
        {
            Debug.LogWarning("[" + _class + "](" + _method + ") : " + _content);
        }

        public static void error(string title, string content)
        {
            Debug.LogError("[" + title + "] : " + content);
        }

        public static void error(string _class, string _method, string _content)
        {
            Debug.LogError ("[" + _class + "](" + _method + ") : " + _content);
        }

        public static void isTrue(bool _condition, string _class, string _method, string _content)
        {
            Assert.IsTrue(_condition, "[" + _class + "](" + _method + ") : " + _content);
        }

        public static void isFalse(bool _condition, string _class, string _method, string _content)
        {
            Assert.IsFalse(_condition, "[" + _class + "](" + _method + ") : " + _content);
        }
    }
}

