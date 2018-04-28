using System.Collections.Generic;
using tezcat.String;

namespace tezcat.DataBase
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

            public Group createType(TezStaticString type_name, int type_id, TezEventBus.Function<TezItem> function)
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
            TezEventBus.Function<TezItem> m_Function = null;

            public void register(TezStaticString name, TezEventBus.Function<TezItem> function)
            {
                m_Function = function;
                this.name = name;
            }

            public TezItem create()
            {
#if UNITY_EDITOR
                TezDebug.isTrue(m_Function != null
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

        public static TezItem create(int group_id, int type_id)
        {
            return m_List[group_id][type_id].create();
        }

        public static T create<T>(int group_id, int type_id) where T : TezItem
        {
            var item = m_List[group_id][type_id].create();

            if(!(item is T))
            {
                throw new System.ArgumentException();
            }

            return (T)item;
        }

        public static TezItem create(TezStaticString group_name, TezStaticString type_name)
        {
            return m_Dic[group_name][type_name].create();
        }

        public static T create<T>(TezStaticString group_name, TezStaticString type_name) where T : TezItem
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