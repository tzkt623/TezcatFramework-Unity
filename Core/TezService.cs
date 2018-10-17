using System.Collections.Generic;
using tezcat.TypeTraits;

namespace tezcat.Core
{
    public interface ITezService : ITezCloseable
    {

    }

    /// <summary>
    /// 服务管理类
    /// </summary>
    public sealed class TezService
    {
        sealed class ServiceID<T> : TezTypeInfo<T, TezService> where T : ITezService
        {
            private ServiceID() { }
        }

        static List<ITezService> m_List = new List<ITezService>();

        /// <summary>
        /// 注册一个服务
        /// </summary>
        /// <typeparam name="IService">
        /// 使用接口或抽象类型 可以获得替换同类服务的功能
        /// 使用实现类型 则无法替换同类型服务
        /// </typeparam>
        /// <param name="service">创建服务</param>
        public static void register<IService>(IService service) where IService : ITezService
        {
            switch (ServiceID<IService>.ID)
            {
                case TezTypeInfo.ErrorID:
                    ServiceID<IService>.setID(m_List.Count);
                    m_List.Add(service);
                    break;
                default:
                    m_List[ServiceID<IService>.ID] = service;
                    break;
            }
        }

        public static T get<T>() where T : ITezService
        {
            return (T)m_List[ServiceID<T>.ID];
        }

        public static void close()
        {
            foreach (var service in m_List)
            {
                service.close();
            }

            m_List.Clear();
            m_List = null;
        }
    }
}