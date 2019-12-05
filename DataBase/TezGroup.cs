using System;
using System.Collections.Generic;
using tezcat.Framework.Definition;
using tezcat.Framework.Extension;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.Database
{
    /// <summary>
    /// Object的组分类
    /// </summary>
    public interface ITezGroup
        : ITezEnumeration
        , IEquatable<ITezGroup>
    {

    }

    /// <summary>
    /// Object的子组分类
    /// </summary>
    public interface ITezSubgroup
        : ITezEnumeration
        , IEquatable<ITezSubgroup>
    {
        TezDatabaseGameItem create();
    }

    public static class TezGroupManager
    {
        public class Linker
        {
            public ITezGroup group { get; set; }

            ITezEnumeration[] m_List = null;
            Dictionary<string, ITezEnumeration> m_Dic = null;

            public ITezSubgroup getSubgroup(string name)
            {
                return (ITezSubgroup)m_Dic[name];
            }

            public ITezSubgroup getSubgroup(int index)
            {
                return (ITezSubgroup)m_List[index];
            }

            public void register(Dictionary<string, ITezEnumeration> enumWithName, ITezEnumeration[] enumArray)
            {
                if (m_Dic == null)
                {
                    m_Dic = enumWithName;
                }

                if (m_List == null)
                {
                    m_List = enumArray;
                }
            }
        }

        static Dictionary<string, Linker> m_Dic = new Dictionary<string, Linker>();
        static List<Linker> m_List = new List<Linker>();

        public static void registerGroup(ITezGroup group)
        {
            while (m_List.Count <= group.toID)
            {
                m_List.Add(new Linker());
            }

            m_List[group.toID].group = group;
            m_Dic[group.toName] = new Linker() { group = group };
        }

        public static Linker get(ITezGroup group)
        {
            return m_List[group.toID];
        }

        public static Linker get(string name)
        {
            return m_Dic[name];
        }

        public static void registerSubgroup(ITezGroup group,
            Dictionary<string, ITezEnumeration> enumWithName,
            ITezEnumeration[] enumArray)
        {
            int id = group.toID;
            while (id >= m_List.Count)
            {
                m_List.Add(null);
            }

            if (m_List[id] == null)
            {
                var linker = new Linker()
                {
                    group = group
                };
                linker.register(enumWithName, enumArray);
                m_List[id] = linker;
                m_Dic.Add(group.toName, linker);
            }
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
            TezGroupManager.registerSubgroup(group, EnumWithName, EnumArray);
            m_Creator = creator;
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