using System;
using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public interface ITezValueName
        : IEquatable<ITezValueName>
        , IComparable<ITezValueName>
    {
        int ID { get; }
        string name { get; }
    }

    public class TezValueName
        : ITezValueName
    {
        public int ID { get; }
        public string name { get; }

        protected TezValueName(int id, string name)
        {
            this.name = name;
            this.ID = id;
        }

        public bool Equals(ITezValueName other)
        {
            return this.ID == other.ID;
        }

        public int CompareTo(ITezValueName other)
        {
            return this.ID.CompareTo(other.ID);
        }

        public override int GetHashCode()
        {
            return this.ID.GetHashCode();
        }

        public static implicit operator int(TezValueName vn)
        {
            return vn.ID;
        }

        #region 注册
        static Dictionary<string, TezValueName> m_NameDic = new Dictionary<string, TezValueName>();
        public static TezValueName register(string name)
        {
            TezValueName property;
            if (!m_NameDic.TryGetValue(name, out property))
            {
                property = new TezValueName(m_NameDic.Count, name);
                m_NameDic.Add(name, property);
            }

            return property;
        }

        public static TezValueName get(string name)
        {
            TezValueName pn;
            m_NameDic.TryGetValue(name, out pn);
            return pn;
        }

        public static void foreachName(TezEventExtension.Action<TezValueName> action)
        {
            foreach (var pair in m_NameDic)
            {
                action(pair.Value);
            }
        }
        #endregion
    }
}