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
            return this.ID;
        }

        public static implicit operator int(TezValueDescriptor<Descriptor> vn)
        {
            return vn.ID;
        }

        #region 注册
        static Dictionary<string, ITezValueDescriptor> m_NameDic = new Dictionary<string, ITezValueDescriptor>();
        static List<ITezValueDescriptor> m_NameList = new List<ITezValueDescriptor>();
        public static ITezValueDescriptor register(string name)
        {
            ITezValueDescriptor property;
            if (!m_NameDic.TryGetValue(name, out property))
            {
                property = new TezValueDescriptor<Descriptor>(m_NameList.Count, name);
                m_NameDic.Add(name, property);
                m_NameList.Add(property);
            }

            return property;
        }

        public static ITezValueDescriptor get(string name)
        {
            ITezValueDescriptor pn;
            if(!m_NameDic.TryGetValue(name, out pn))
            {
                throw new Exception(string.Format("This Value[{0}] is not registered!!", name));
            }
            return pn;
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