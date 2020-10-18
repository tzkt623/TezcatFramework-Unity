using System;

namespace tezcat.Framework.TypeTraits
{
    /// <summary>
    /// 此类只是用来包装ErrorID
    /// 避免产生不同类型的多个重复ErrorID
    /// </summary>
    public abstract class TezTypeInfo
    {
        public const int ErrorID = -1;

        protected TezTypeInfo() { }
    }

    /// <summary>
    /// 类型数据萃取类
    /// 此类以及子类不能实例化
    /// 使用他的Belong类来分配ID
    /// </summary>
    /// <typeparam name="Type">被萃取的类型</typeparam>
    /// <typeparam name="Belong">
    /// 被萃取的类型的归属类
    /// 此类负责分配ID(一个类型可能存在于多个归属类中 在每个归属类中的ID都有可能不同)
    /// Belong类型也用于在编译时生成不同的类类型
    /// </typeparam>
    public class TezTypeInfo<Type, Belong> : TezTypeInfo
    {
        protected static int m_ID = ErrorID;
        public static int ID
        {
            get { return m_ID; }
        }
        public static string Name { get; } = typeof(Type).Name;
        public static System.Type systemType { get; } = typeof(Type);

        public static Action onInit { get; set; }

        /// <summary>
        /// 设置唯一ID
        /// 自带重复检测
        /// </summary>
        /// <param name="id"></param>
        public static void setID(int id)
        {
            if (m_ID == ErrorID)
            {
                m_ID = id;
                onInit?.Invoke();
            }
            else
            {
                throw new ArgumentException(string.Format("{0} : this type is registered Or invoked in error position", typeof(Type).Name));
            }
        }

        protected TezTypeInfo() { }
    }

    public sealed class TezTypeInfoID
    {
        static int m_ID = 0;

        public static int giveID()
        {
            return m_ID++;
        }
    }

    /// <summary>
    /// 在Tez管理器下的类型ID
    /// </summary>
    /// <typeparam name="Type"></typeparam>
    public sealed class TezTypeOf<Type> : TezTypeInfo<Type, TezTypeInfoID>
    {
        /// <summary>
        /// 全局类型ID
        /// </summary>
        public static int TID
        {
            get
            {
                switch (m_ID)
                {
                    case TezTypeInfo.ErrorID:
                        m_ID = TezTypeInfoID.giveID();
                        break;
                }

                return m_ID;
            }
        }

        private TezTypeOf() { }
    }
}

