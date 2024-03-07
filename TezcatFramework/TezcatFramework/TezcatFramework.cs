using tezcat.Framework.Database;
using tezcat.Framework.Event;
using tezcat.Framework.Game;
using tezcat.Framework.Threading;
using tezcat.Framework.Utility;

namespace tezcat.Framework
{
    public class TezcatFramework
    {
        #region Buildin
        static TezThread mThread = new TezThread();
        public static TezThread thread => mThread;

        static TezEventDispatcher mEventDispatcher = new TezEventDispatcher();
        public static TezEventDispatcher eventDispatcher => mEventDispatcher;

        static TezClassFactory mClassFactory = new TezClassFactory();
        public static TezClassFactory classFactory => mClassFactory;

        static TezTranslator mTranslator = new TezTranslator();
        public static TezTranslator translator => mTranslator;

        public static void initBuildinService()
        {

        }
        #endregion


        #region Customable
        /// <summary>
        /// 主数据库
        /// </summary>
        static TezItemDatabase mMainDB = null;
        public static TezItemDatabase mainDB => mMainDB;
        public static void set(TezItemDatabase mainDB)
        {
            mMainDB = mainDB;
        }
        /// <summary>
        /// 运行时数据库
        /// </summary>
        static TezRunTimeDatabase mRTDB = null;
        public static TezRunTimeDatabase runtimeDB => mRTDB;
        public static void set(TezRunTimeDatabase runtimeDB)
        {
            mRTDB = runtimeDB;
        }
        #endregion
    }
}