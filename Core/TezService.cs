using System.Collections.Generic;
using tezcat.Framework.TypeTraits;
using UnityEngine;

namespace tezcat.Framework.Core
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
            public static ITezService service;
        }

        static List<ITezService> m_List = new List<ITezService>();

        /// <summary>
        /// 注册一个服务
        /// <para>使用接口或抽象类型作为类型参数 可以获得替换同类服务的功能</para>
        /// <para>使用具体实现类型作为类型参数 则无法替换同类型服务</para>
        /// </summary>
        public static void register<IService>(IService service) where IService : ITezService
        {
            switch (ServiceID<IService>.ID)
            {
                case TezTypeInfo.ErrorID:
                    ServiceID<IService>.setID(m_List.Count);
                    ServiceID<IService>.service = service;
                    m_List.Add(service);
                    Debug.Log(string.Format("Service : [{0}] is registered!", service.GetType().Name));
                    break;
                default:
                    ServiceID<IService>.service = service;
                    m_List[ServiceID<IService>.ID] = service;
                    break;
            }
        }

        /// <summary>
        /// 获得抽象服务
        /// </summary>
        public static IService get<IService>()
            where IService : ITezService
        {
            return (IService)ServiceID<IService>.service;
        }

        /// <summary>
        /// 通过抽象服务对象获得具体服务
        /// </summary>
        public static IService get<IService, IImplement>()
            where IService : ITezService
            where IImplement : IService
        {
            return (IImplement)ServiceID<IService>.service;
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