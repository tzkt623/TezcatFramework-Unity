using tezcat.Framework.Database;
using tezcat.Framework.Event;
using tezcat.Framework.Threading;
using tezcat.Framework.Utility;

namespace tezcat.Framework
{
    public class TezcatFramework
    {
        #region Buildin
        static TezThread mThread = null;
        public static TezThread thread => mThread;

        static TezEventDispatcher mEventDispatcher = null;
        public static TezEventDispatcher eventDispatcher => mEventDispatcher;

        static TezClassFactory mClassFactory = new TezClassFactory();
        public static TezClassFactory classFactory => mClassFactory;

        /// <summary>
        /// 事件总线
        /// </summary>
        /// <summary>
        /// 多线程
        /// </summary>
        public static void initBuildinService()
        {
            mThread = new TezThread();
            mEventDispatcher = new TezEventDispatcher();
        }
        #endregion


        #region Customable
        static TezBaseItemInfo mEmptyItemInfo = new TezErrorItemInfo();
        public static TezBaseItemInfo emptyItemInfo => mEmptyItemInfo;


        /// <summary>
        /// 文件数据库
        /// </summary>
        static TezFileDatabase mFileDB = null;
        public static TezFileDatabase fileDB => mFileDB;
        public static void set(TezFileDatabase fileDB)
        {
            mFileDB = fileDB;
        }

        /// <summary>
        /// 主数据库
        /// </summary>
        static TezMainDatabase mMainDB = null;
        public static TezMainDatabase mainDB => mMainDB;
        public static void set(TezMainDatabase mainDB)
        {
            mMainDB = mainDB;
        }
        /// <summary>
        /// 运行时数据库
        /// </summary>
        static TezRunTimeDatabase mRTDB = null;
        public static TezRunTimeDatabase rtDB => mRTDB;
        public static void set(TezRunTimeDatabase runtimeDB)
        {
            mRTDB = runtimeDB;
        }
        /// <summary>
        /// 分体式数据库
        /// </summary>
        static TezMultiDatabase mMultiDB = null;
        public static TezMultiDatabase multiDB => mMultiDB;
        public static void set(TezMultiDatabase multiDB)
        {
            mMultiDB = multiDB;
        }
        #endregion
    }
}