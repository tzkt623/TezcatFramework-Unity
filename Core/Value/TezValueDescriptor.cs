using System;
using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezValueDescriptor
        : IEquatable<ITezValueDescriptor>
        , IComparable<ITezValueDescriptor>
    {
        int ID { get; }
        string name { get; }
    }

    public class TezValueDescriptor
        : ITezValueDescriptor
    {
        public int ID { get; }
        public string name { get; }

        protected TezValueDescriptor(int id, string name)
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
            return this.ID;
        }

        public static implicit operator int(TezValueDescriptor vn)
        {
            return vn.ID;
        }

        #region 注册
        static Dictionary<string, TezValueDescriptor> m_NameDic = new Dictionary<string, TezValueDescriptor>();
        static List<TezValueDescriptor> m_NameList = new List<TezValueDescriptor>();
        public static TezValueDescriptor register(string name)
        {
            TezValueDescriptor property;
            if (!m_NameDic.TryGetValue(name, out property))
            {
                property = new TezValueDescriptor(m_NameList.Count, name);
                m_NameDic.Add(name, property);
                m_NameList.Add(property);
            }

            return property;
        }

        public static TezValueDescriptor get(string name)
        {
            TezValueDescriptor pn;
            if(!m_NameDic.TryGetValue(name, out pn))
            {
                throw new Exception(string.Format("This Propoerty[{0}] is not registered!!", name));
            }
            return pn;
        }

        public static TezValueDescriptor get(int id)
        {
            return m_NameList[id];
        }

        public static void foreachName(TezEventExtension.Action<TezValueDescriptor> action)
        {
            foreach (var pair in m_NameDic)
            {
                action(pair.Value);
            }
        }
        #endregion
    }
}