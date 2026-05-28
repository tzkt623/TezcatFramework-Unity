using System;
using System.Collections.Generic;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.Core
{
    public interface ITezService : ITezCloseable
    {

    }

    /// <summary>
    /// 服务管理类
    /// </summary>
    public struct TezService
    {
        class ServiceID<T> : TezTypeInfo<T, TezService> where T : ITezService
        {
            private ServiceID() { }
            public static T service;
        }

        static List<ITezService> mList = new List<ITezService>();

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
                    ServiceID<IService>.setID(mList.Count);
                    ServiceID<IService>.service = service;
                    mList.Add(service);
                    Console.WriteLine(string.Format("Service : [{0}] is registered!", service.GetType().Name));
                    break;
                default:
                    ServiceID<IService>.service = service;
                    mList[ServiceID<IService>.ID] = service;
                    break;
            }
        }

        /// <summary>
        /// 获得抽象服务
        /// </summary>
        public static IService get<IService>()
            where IService : ITezService
        {
            return ServiceID<IService>.service;
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
            foreach (var service in mList)
            {
                service.close();
            }

            mList.Clear();
            mList = null;
        }
    }
}