﻿using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using tezcat.Framework.String;

namespace tezcat.Framework.Database
{
    public static class TezDatabaseItemFactory
    {
        public class Group
        {
            public List<Container> containers { get; private set; } = new List<Container>();
            public TezStaticString name { get; private set; }

            Dictionary<TezStaticString, Container> m_Dic = new Dictionary<TezStaticString, Container>();

            public Group(TezStaticString name)
            {
                this.name = name;
            }

            public Group create(TezStaticString type_name, int type_id, TezEventExtension.Function<TezDatabaseItem> function)
            {
                Container container = null;
                if(!m_Dic.TryGetValue(type_name, out container))
                {
                    while (containers.Count <= type_id)
                    {
                        containers.Add(null);
                    }

                    container = new Container();
                    containers[type_id] = container;
                    m_Dic.Add(type_name, container);
                }

                container.register(type_name, function);
                return this;
            }

            public Container this[int type_id]
            {
                get { return containers[type_id]; }
            }

            public Container this[TezStaticString type_name]
            {
                get { return m_Dic[type_name]; }
            }

            public Container convertToType(int type_id)
            {
                return this.containers[type_id];
            }
        }

        public class Container
        {
            public TezStaticString name { get; private set; }
            TezEventExtension.Function<TezDatabaseItem> m_Function = null;

            public void register(TezStaticString name, TezEventExtension.Function<TezDatabaseItem> function)
            {
                m_Function = function;
                this.name = name;
            }

            public TezDatabaseItem create()
            {
#if UNITY_EDITOR
                TezService.get<TezDebug>().isTrue(m_Function != null
                    , "TezDatabaseItemFactory"
                    , string.Format("{0}`s Create Function is null", this.name.convertToString()));
#endif
                return m_Function();
            }
        }

        static List<Group> m_List = new List<Group>();
        static Dictionary<TezStaticString, Group> m_Dic = new Dictionary<TezStaticString, Group>();

        public static Group convertToGroup(int group_id)
        {
            return m_List[group_id];
        }

        public static Group createGroup(TezStaticString group_name, int group_id)
        {
            Group group = null;
            if (!m_Dic.TryGetValue(group_name, out group))
            {
                while (m_List.Count <= group_id)
                {
                    m_List.Add(null);
                }

                group = new Group(group_name);
                m_Dic.Add(group_name, group);
                m_List[group_id] = group;
            }

            return group;
        }

        public static Group getGroup(int group_id)
        {
            return m_List[group_id];
        }

        public static Group getGroup(TezStaticString group_name)
        {
            return m_Dic[group_name];
        }

        public static TezDatabaseItem create(int group_id, int type_id)
        {
            return m_List[group_id][type_id].create();
        }

        public static T create<T>(int group_id, int type_id) where T : TezDatabaseItem
        {
            var item = m_List[group_id][type_id].create();

            if(!(item is T))
            {
                throw new System.ArgumentException();
            }

            return (T)item;
        }

        public static TezDatabaseItem create(TezStaticString group_name, TezStaticString type_name)
        {
            return m_Dic[group_name][type_name].create();
        }

        public static T create<T>(TezStaticString group_name, TezStaticString type_name) where T : TezDatabaseItem
        {
            var item = m_Dic[group_name][type_name].create();

            if(!(item is T))
            {
                throw new System.ArgumentException();
            }

            return (T)item;
        }
    }
}