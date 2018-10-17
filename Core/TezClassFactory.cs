using System;
using System.Collections.Generic;
using tezcat.TypeTraits;

namespace tezcat.Core
{
    /// <summary>
    /// 类型生成器
    /// 
    /// 可以使用类名称生成指定类型
    /// 需要在使用前先注册指定的实现类型
    /// 
    /// 主要使用在反序列化时
    /// 不同子类共用父类存储器导致无法获得实际类型时
    /// </summary>
    public class TezClassFactory : ITezService
    {
        public delegate T Creator<out T>();

        sealed class ClassID<Type> : TezTypeInfo<Type, TezClassFactory>
        {
            /// <summary>
            /// 萃取类
            /// 不需要实例化
            /// </summary>
            private ClassID() { }
        }
        Dictionary<string, Creator<object>> m_DicWithName = new Dictionary<string, Creator<object>>();
        List<Creator<object>> m_List = new List<Creator<object>>();

        public void register<T>(Creator<T> function) where T : class
        {
            switch (ClassID<T>.ID)
            {
                case TezTypeInfo.ErrorID:
                    ClassID<T>.setID(m_List.Count);
                    m_List.Add(function);
                    m_DicWithName.Add(ClassID<T>.Name, function);
                    break;
                default:
                    throw new Exception(string.Format("{0} : this type is registered in TezClassFactory", ClassID<T>.Name));
            }
        }

        public T create<T>(string name) where T : class
        {
            return (T)m_DicWithName[name]();
        }

        public T create<T>() where T : class
        {
            return (T)m_List[ClassID<T>.ID]();
        }

        public void close()
        {
            m_DicWithName.Clear();
            m_List.Clear();

            m_DicWithName = null;
            m_List = null;
        }
    }
}