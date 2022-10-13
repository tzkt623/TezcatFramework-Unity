using tezcat.Framework.Event;
using tezcat.Framework.Threading;

namespace tezcat.Framework
{
    public class TezcatFramework
    {
        static TezThread mThread = null;
        static TezEventDispatcher mEventDispatcher = null;

        public static TezEventDispatcher eventDispatcher => mEventDispatcher;
        public static TezThread thread => mThread;

        public static void initService()
        {
            mThread = new TezThread();
            mEventDispatcher = new TezEventDispatcher();
        }
    }
}