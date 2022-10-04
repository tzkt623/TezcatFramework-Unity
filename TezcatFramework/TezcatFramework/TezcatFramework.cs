using tezcat.Framework.Event;
using tezcat.Framework.Threading;

namespace tezcat.Framework
{
    public class TezcatFramework
    {
        #region Tools
        static TezThread m_Thread = null;
        static TezEventDispatcher m_EventDispatcher = null;

        public static TezEventDispatcher eventDispatcher => m_EventDispatcher;
        public static TezThread thread => m_Thread;
        #endregion

        public static void initService()
        {
            m_Thread = new TezThread();
            m_EventDispatcher = new TezEventDispatcher();
        }
    }
}