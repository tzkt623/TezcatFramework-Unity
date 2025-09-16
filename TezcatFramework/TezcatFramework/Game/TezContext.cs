using System.Collections.Generic;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 保存全局单例的上下文
    /// 用于替代传统的单例模式
    /// </summary>
    public class TezContext
    {
        class TypeInstance<T>
        {
            public static int index = -1;
        }

        static List<object> mInstances = new List<object>();

        public static void register<T>() where T : new()
        {
            TypeInstance<T>.index = mInstances.Count;
            mInstances.Add(new T());
        }

        public static T resolve<T>()
        {
            return (T)mInstances[TypeInstance<T>.index];
        }
    }
}