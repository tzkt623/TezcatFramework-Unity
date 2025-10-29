using tezcat.Framework.Core;
using tezcat.Framework.Game;
using tezcat.Framework.Utility;

namespace tezcat.Framework
{
    /// <summary>
    /// 
    /// Core核心层:
    /// 提供核心的组件功能
    /// 
    /// Game游戏层:
    /// 提供游戏中需要的各种功能
    /// 
    /// Utility通用层
    /// 提供各种辅助工具
    /// 
    /// Extension扩展层
    /// 提供基于C#类扩展功能的工具
    /// 
    /// 此类用于集中管理各种工具类
    /// 方便提供各种全局功能
    /// 
    /// </summary>
    public class TezcatFramework
    {
        static TezThread mThread = new TezThread();
        /// <summary>
        /// 线程管理
        /// </summary>
        public static TezThread thread => mThread;

        static TezEventDispatcher mEventDispatcher = new TezEventDispatcher();
        /// <summary>
        /// 中心事件
        /// </summary>
        public static TezEventDispatcher eventDispatcher => mEventDispatcher;

        static TezClassFactory mClassFactory = new TezClassFactory();
        /// <summary>
        /// 类工厂
        /// </summary>
        public static TezClassFactory classFactory => mClassFactory;

        static TezTranslator mTranslator = new TezTranslator();
        /// <summary>
        /// 翻译器
        /// </summary>
        public static TezTranslator translator => mTranslator;

        static TezProtoDatabase mProtoDB = new TezProtoDatabase();
        /// <summary>
        /// 原型数据库
        /// </summary>
        public static TezProtoDatabase protoDB => mProtoDB;


        static TezRuntimeDatabase mRumTimeDB = new TezRuntimeDatabase();
        /// <summary>
        /// 原型数据库
        /// </summary>
        public static TezRuntimeDatabase runtimeDB => mRumTimeDB;

        static TezSignalSystem mSignal = new TezSignalSystem();
        public static TezSignalSystem signalSystem => mSignal;


        static TezUpdaterManager mUpdaterManager = new TezUpdaterManager();
        public static TezUpdaterManager updaterManager => mUpdaterManager;

    }
}