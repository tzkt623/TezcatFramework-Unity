using System;
using System.Collections;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Database;
using tezcat.Framework.ECS;
using tezcat.Framework.Event;
using tezcat.Framework.GraphicSystem;
using tezcat.Framework.Threading;
using tezcat.Framework.Utility;
using UnityEngine;

namespace tezcat.Framework
{
    public class TezcatFramework
    {
        #region Tools
        static TezGraphicSystem m_GraphicSystem = null;
        static TezPrefabDatabase m_PrefabDatabase = null;
        static TezThread m_Thread = null;
        static TezEventDispatcher m_EventDispatcher = null;
        static UnityKeyConfigSystem m_UnityKeyConfigSystem = null;

        public static TezGraphicSystem graphicSystem => m_GraphicSystem;
        public static TezPrefabDatabase prefabDatabase => m_PrefabDatabase;
        public static TezEventDispatcher eventDispatcher => m_EventDispatcher;
        public static TezThread thread => m_Thread;
        public static UnityKeyConfigSystem unityKeyConfigSystem => m_UnityKeyConfigSystem;
        #endregion

        public static void initService()
        {
            m_GraphicSystem = new TezGraphicSystem();
            m_PrefabDatabase = new TezPrefabDatabase();
            m_Thread = new TezThread();
            m_EventDispatcher = new TezEventDispatcher();
            m_UnityKeyConfigSystem = new UnityKeyConfigSystem();
        }
    }
}