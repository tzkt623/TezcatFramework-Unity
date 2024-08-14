using tezcat.Framework.Core;
using tezcat.Framework.Game;
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
        static TezProtoDatabase mProtoDB = null;
        public static TezProtoDatabase protoDB => mProtoDB;
        public static void set(TezProtoDatabase protoDB)
        {
            mProtoDB = protoDB;
        }

        /// <summary>
        /// 运行时数据库
        /// </summary>
        //static TezRunTimeDatabase mRTDB = null;
        //public static TezRunTimeDatabase runtimeDB => mRTDB;
        //public static void set(TezRunTimeDatabase runtimeDB)
        //{
        //    mRTDB = runtimeDB;
        //}
        #endregion
    }
}