using System;
using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Core
{
    public interface ITezValueDescriptor
        : IEquatable<ITezValueDescriptor>
        , IComparable<ITezValueDescriptor>
    {
        int ID { get; }
        string name { get; }
    }

    /// <summary>
    /// 值系统的描述类
    /// 用于描述这个值的特征信息
    /// 这种描述整个程序里面只应该生成一份用于共享
    /// 
    /// <para></para>
    /// 
    /// 这里的模板参数用于产生多种不同归属的描述信息
    /// </summary>
    public sealed class TezValueDescriptor<Descriptor>
        : ITezValueDescriptor
    {
        public int ID { get; }
        public string name { get; }

        TezValueDescriptor(int id, string name)
        {
            this.name = name;
            this.ID = id;
        }

        public bool Equals(ITezValueDescriptor other)
        {
            return this.ID == other.ID;
        }

        public int CompareTo(ITezValueDescriptor other)
        {
            return this.ID.CompareTo(other.ID);
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        #region 注册
        static Dictionary<string, ITezValueDescriptor> m_NameDic = new Dictionary<string, ITezValueDescriptor>();
        static List<ITezValueDescriptor> m_NameList = new List<ITezValueDescriptor>();

        public static ITezValueDescriptor register(string name)
        {
            if (m_NameDic.TryGetValue(name, out ITezValueDescriptor descriptor))
            {
                throw new ArgumentException();
            }
            else
            {
                descriptor = new TezValueDescriptor<Descriptor>(m_NameList.Count, name);
                m_NameDic.Add(name, descriptor);
                m_NameList.Add(descriptor);
            }

            return descriptor;
        }

        public static ITezValueDescriptor get(string name)
        {
            ITezValueDescriptor descriptor;
            if (!m_NameDic.TryGetValue(name, out descriptor))
            {
                throw new Exception(string.Format("This Value[{0}] is not registered!!", name));
            }
            return descriptor;
        }

        public static ITezValueDescriptor get(int id)
        {
            return m_NameList[id];
        }

        public static void foreachName(TezEventExtension.Action<ITezValueDescriptor> action)
        {
            foreach (var pair in m_NameDic)
            {
                action(pair.Value);
            }
        }
        #endregion
    }
}