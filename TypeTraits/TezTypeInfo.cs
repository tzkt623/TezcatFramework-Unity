using System;

namespace tezcat.TypeTraits
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
    /// <typeparam name="Belong">被萃取的类型的归属类 此类负责分配ID(一个类型可能存在于多个归属类中 在每个归属类中的ID都有可能不同)</typeparam>
    public class TezTypeInfo<Type, Belong> : TezTypeInfo where Belong : class
    {
        public static int ID { get; private set; } = ErrorID;
        public static string Name { get; } = typeof(Type).Name;

        /// <summary>
        /// 设置唯一ID
        /// 自带重复检测
        /// </summary>
        /// <param name="id"></param>
        public static void setID(int id)
        {
            if (ID == ErrorID)
            {
                ID = id;
            }
            else
            {
                throw new ArgumentException(string.Format("{0} : this type is registered Or invoked in error position", typeof(Type).Name));
            }
        }

        protected TezTypeInfo() { }
    }
}

