using System;
using System.Collections.Generic;
using tezcat.Framework.Definition;
using tezcat.Framework.Extension;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.Database
{
    public interface ITezGroup
        : ITezEnumeration
        , IEquatable<ITezGroup>
    {

    }

    public interface ITezSubgroup
        : ITezEnumeration
        , IEquatable<ITezSubgroup>
    {
        TezDatabaseGameItem create();
    }

    public class TezGroupManager
    {
        public class Pair
        {
            public ITezGroup group { get; set; }

            public List<ITezSubgroup> detailedGroupList { get; private set; } = new List<ITezSubgroup>();

            public ITezSubgroup getDetailedGroup(string name)
            {
                return detailedGroupList.Find((ITezSubgroup DG) =>
                {
                    return DG.toName == name;
                });
            }
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
            m_GroupDic[group.toName] = group.toID;
        }

        public static void registerDetailedGroup(ITezGroup group, ITezSubgroup subgroup)
        {
            var list = m_GroupList[group.toID].detailedGroupList;
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

    /// <summary>
    /// 对象大组
    /// </summary>
    public abstract class TezGroup<TEnum, TValue>
        : TezEnumeration<TEnum, TValue>
        , ITezGroup
        where TEnum : TezGroup<TEnum, TValue>
        where TValue : struct, IComparable
    {
        protected TezGroup(TValue value) : base(value)
        {
            TezGroupManager.registerGroup(this);
        }

        public bool Equals(ITezGroup other)
        {
            return this.toID == other.toID;
        }
    }

    /// <summary>
    /// 对象详细类型分组
    /// </summary>
    public abstract class TezSubgroup<TEnum, TValue>
        : TezEnumeration<TEnum, TValue>
        , ITezSubgroup
        where TEnum : TezSubgroup<TEnum, TValue>
        where TValue : struct, IComparable
    {
        TezEventExtension.Function<TezDatabaseGameItem> m_Creator = null;

        protected TezSubgroup(ITezGroup group, TValue value, TezEventExtension.Function<TezDatabaseGameItem> creator) : base(value)
        {
            m_Creator = creator;
            TezGroupManager.registerDetailedGroup(group, this);
        }

        public TezDatabaseGameItem create()
        {
            return m_Creator();
        }

        public bool Equals(ITezSubgroup other)
        {
            return this.toID == other.toID;
        }
    }

    /// <summary>
    /// 对象细分组
    /// </summary>
    public abstract class TezTokenGroup<TEnumeration, TEnumValue>
        : TezDefinitionToken<TEnumeration, TEnumValue>
        where TEnumeration : TezDefinitionToken<TEnumeration, TEnumValue>
        where TEnumValue : struct, IComparable
    {
        protected TezTokenGroup(TEnumValue value, TezDefinitionTokenType token_type)
            : base(value, token_type)
        {

        }
    }
}