using System;
using System.Collections.Generic;
using tezcat.Framework.Extension;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.DataBase
{
    public interface ITezGroup
        : ITezEnumeration
        , IEquatable<ITezGroup>
    {

    }

    public interface ITezSubGroup
        : ITezEnumeration
        , IEquatable<ITezSubGroup>
    {
        TezDataBaseGameItem create();
    }

    public class TezGroupManager
    {
        public class Pair
        {
            public ITezGroup group;
            public List<ITezSubGroup> subgroupList = new List<ITezSubGroup>();
        }

        static Dictionary<string, int> m_GroupDic = new Dictionary<string, int>();
        static List<Pair> m_GroupList = new List<Pair>();

        public static void registerGroup(ITezGroup group)
        {
            while (m_GroupList.Count <= group.toID)
            {
                m_GroupList.Add(new Pair());
            }

            m_GroupList[group.toID].group = group;
            m_GroupDic[group.NID] = group.toID;
        }

        public static void registerSubGroup(ITezGroup group, ITezSubGroup subgroup)
        {
            var list = m_GroupList[group.toID].subgroupList;
            while (list.Count <= subgroup.toID)
            {
                list.Add(null);
            }

            list[subgroup.toID] = subgroup;
        }

        public static Pair get(ITezGroup group)
        {
            return m_GroupList[group.toID];
        }

        public static Pair get(string name)
        {
            return m_GroupList[m_GroupDic[name]];
        }
    }

    public abstract class TezGroup<TEnum, TValue>
        : TezEnumeration<TEnum, TValue>
        , ITezGroup
        where TEnum : TezGroup<TEnum, TValue>
        where TValue : IComparable
    {
        protected TezGroup(TValue value, string name) : base(value, name)
        {
            TezGroupManager.registerGroup(this);
        }

        public bool Equals(ITezGroup other)
        {
            return this.toID == other.toID;
        }
    }

    public abstract class TezSubGroup<TEnum, TValue>
        : TezEnumeration<TEnum, TValue>
        , ITezSubGroup
        where TEnum : TezSubGroup<TEnum, TValue>
        where TValue : IComparable
    {
        TezEventExtension.Function<TezDataBaseGameItem> m_Creator = null;

        protected TezSubGroup(ITezGroup group, TValue value, string name, TezEventExtension.Function<TezDataBaseGameItem> creator) : base(value, name)
        {
            m_Creator = creator;
            TezGroupManager.registerSubGroup(group, this);
        }

        public TezDataBaseGameItem create()
        {
            return m_Creator();
        }

        public bool Equals(ITezSubGroup other)
        {
            return this.toID == other.toID;
        }
    }
}