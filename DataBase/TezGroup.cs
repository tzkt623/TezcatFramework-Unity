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

    public interface ITezSubgroup
        : ITezDefinitionToken
        , IEquatable<ITezSubgroup>
    {
        TezDataBaseGameItem create();
    }

    public class TezGroupManager
    {
        public class Pair
        {
            public ITezGroup group;
            public List<ITezSubgroup> subgroupList = new List<ITezSubgroup>();
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

        public static void registerSubGroup(ITezGroup group, ITezSubgroup subgroup)
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
        : TezDefinitionToken<TEnum, TValue>
        , ITezGroup
        where TEnum : TezGroup<TEnum, TValue>
        where TValue : struct, IComparable
    {
        public override TezDefinitionTokenType tokenType => TezDefinitionTokenType.Root;

        protected TezGroup(TValue value) : base(value)
        {
            TezGroupManager.registerGroup(this);
        }

        public bool Equals(ITezGroup other)
        {
            return this.toID == other.toID;
        }
    }

    public abstract class TezSubgroup<TEnum, TValue>
        : TezDefinitionToken<TEnum, TValue>
        , ITezSubgroup
        where TEnum : TezSubgroup<TEnum, TValue>
        where TValue : struct, IComparable
    {
        TezEventExtension.Function<TezDataBaseGameItem> m_Creator = null;

        public override TezDefinitionTokenType tokenType => TezDefinitionTokenType.Leaf;

        protected TezSubgroup(ITezGroup group, TValue value, TezEventExtension.Function<TezDataBaseGameItem> creator) : base(value)
        {
            m_Creator = creator;
            TezGroupManager.registerSubGroup(group, this);
        }

        public TezDataBaseGameItem create()
        {
            return m_Creator();
        }

        public bool Equals(ITezSubgroup other)
        {
            return this.toID == other.toID;
        }
    }
}