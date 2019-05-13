using System;
using System.Collections.Generic;
using tezcat.Framework.Definition;
using tezcat.Framework.Extension;

namespace tezcat.Framework.DataBase
{
    public interface ITezGroup
        : ITezDefinitionToken
        , IEquatable<ITezGroup>
    {

    }

    public interface ITezDetailedGroup
        : ITezDefinitionToken
        , IEquatable<ITezDetailedGroup>
    {
        TezDataBaseGameItem create();
    }

    public class TezGroupManager
    {
        public class Pair
        {
            public ITezGroup group { get; set; }

            public List<ITezDetailedGroup> detailedGroupList { get; private set; } = new List<ITezDetailedGroup>();

            public ITezDetailedGroup getDetailedGroup(string name)
            {
                return detailedGroupList.Find((ITezDetailedGroup DG) =>
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

        public static void registerDetailedGroup(ITezGroup group, ITezDetailedGroup subgroup)
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
        : TezDefinitionToken<TEnum, TValue>
        , ITezGroup
        where TEnum : TezGroup<TEnum, TValue>
        where TValue : struct, IComparable
    {
        public sealed override TezDefinitionTokenType tokenType => TezDefinitionTokenType.Root;

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
    public abstract class TezDetailedGroup<TEnum, TValue>
        : TezDefinitionToken<TEnum, TValue>
        , ITezDetailedGroup
        where TEnum : TezDetailedGroup<TEnum, TValue>
        where TValue : struct, IComparable
    {
        TezEventExtension.Function<TezDataBaseGameItem> m_Creator = null;

        public sealed override TezDefinitionTokenType tokenType => TezDefinitionTokenType.Leaf;

        protected TezDetailedGroup(ITezGroup group, TValue value, TezEventExtension.Function<TezDataBaseGameItem> creator) : base(value)
        {
            m_Creator = creator;
            TezGroupManager.registerDetailedGroup(group, this);
        }

        public TezDataBaseGameItem create()
        {
            return m_Creator();
        }

        public bool Equals(ITezDetailedGroup other)
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
        protected TezTokenGroup(TEnumValue value) : base(value)
        {

        }
    }
}