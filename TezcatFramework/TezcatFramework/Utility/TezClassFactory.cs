using System;
using System.Collections.Generic;

namespace tezcat.Framework.Utility
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
    public class TezClassFactory
    {
        public delegate T Creator<out T>();

        Dictionary<string, Creator<object>> mDictWithName = new Dictionary<string, Creator<object>>();

        public void register<T>(Creator<T> function) where T : class
        {
            var name = typeof(T).Name;
            if (mDictWithName.ContainsKey(name))
            {
                throw new Exception($"{name} : this type is registered");
            }
            else
            {
                mDictWithName.Add(name, function);
            }
        }

        public void register<T>() where T : class, new()
        {
            var name = typeof(T).Name;
            if (mDictWithName.ContainsKey(name))
            {
                throw new Exception($"{name} : this type is registered");
            }
            else
            {
                mDictWithName.Add(name, () => new T());
            }
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