using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class TezClassFactoryRegisterAttribute : Attribute
    {
        public string name;
    }
    // 
    // 
    //     [TezClassFactoryRegister(name = "Test32", creator = ()=> { return new Test32(); }]
    //     public class Test32
    //     {
    // 
    //     }

    /// <summary>
    /// 类型生成器
    /// 
    /// 可以使用类名称生成指定类型
    /// 需要在使用前先注册指定的实现类型
    /// 
    /// 主要使用在反序列化时
    /// 不同子类共用父类存储器导致无法获得实际类型时
    /// </summary>
    public class TezClassFactory : ITezCloseable
    {
        public delegate T Creator<out T>();

        Dictionary<string, Creator<object>> mDictWithName = new Dictionary<string, Creator<object>>();

        public void registerByAttribute<T>() where T : class, new()
        {
            registerByAttribute(() => new T());
        }

        public void registerByAttribute<T>(Creator<T> function) where T : class
        {
            var attributes = typeof(T).GetCustomAttributes(false);
            if (attributes.Length == 0)
            {
                throw new Exception("TezClassFactory>> This Function Must has Class Attribute");
            }

            var re_t = typeof(TezClassFactoryRegisterAttribute);
            foreach (var item in attributes)
            {
                if (item.GetType() == re_t)
                {
                    var temp = item as TezClassFactoryRegisterAttribute;
                    if (mDictWithName.ContainsKey(temp.name))
                    {
                        throw new Exception($"TezClassFactory>> {temp.name} : this type is registered");
                    }
                    else
                    {
                        mDictWithName.Add(temp.name, function);
                    }
                }
            }
        }

        public void register<T>(string name, Creator<T> function) where T : class
        {
            if (mDictWithName.ContainsKey(name))
            {
                throw new Exception($"{name} : this type is registered");
            }
            else
            {
                mDictWithName.Add(name, function);
            }
        }

        public void register<T>(string name) where T : class, new()
        {
            register(name, () => new T());
        }

        public T create<T>(string name) where T : class
        {
            if (mDictWithName.TryGetValue(name, out Creator<object> creator))
            {
                return (T)creator();
            }
            else
            {
                throw new Exception($"ClassFactory : {name} this type is not registered");
            }
        }

        public Creator<object> getCreator(string name)
        {
            return mDictWithName[name];
        }

        public Creator<T> getCreator<T>(string name) where T : class
        {
            return (Creator<T>)mDictWithName[name];
        }

        public void close()
        {
            mDictWithName.Clear();
            mDictWithName = null;
        }
    }
}