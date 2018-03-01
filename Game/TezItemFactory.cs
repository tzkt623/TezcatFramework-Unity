using System.Collections;
using System.Collections.Generic;
namespace tezcat
{
    public static class TezItemFactory
    {
        public class Group
        {
            List<Container> m_List = new List<Container>();
            Dictionary<string, Container> m_Dic = new Dictionary<string, Container>();

            public List<Container> containers
            {
                get { return m_List; }
            }

            public Group register(string type_name, int type_id, TezEventBus.Function<TezItem> function)
            {
                Container container = null;
                if(!m_Dic.TryGetValue(type_name, out container))
                {
                    while (m_List.Count <= type_id)
                    {
                        m_List.Add(null);
                    }

                    container = new Container();
                    m_List[type_id] = container;
                    m_Dic.Add(type_name, container);
                }

                container.register(function);
                return this;
            }

            public Container this[int type_id]
            {
                get { return m_List[type_id]; }
            }

            public Container this[string type_name]
            {
                get { return m_Dic[type_name]; }
            }
        }

        public class Container
        {
            TezEventBus.Function<TezItem> m_Function = null;

            public void register(TezEventBus.Function<TezItem> function)
            {
                m_Function = function;
            }

            public TezItem create()
            {
                return m_Function();
            }
        }

        static List<Group> m_List = new List<Group>();
        static Dictionary<string, Group> m_Dic = new Dictionary<string, Group>();

        public static void register(string group_name, int group_id, string type_name, int type_id, TezEventBus.Function<TezItem> function)
        {
            Group group = null;
            if(!m_Dic.TryGetValue(group_name, out group))
            {
                while (m_List.Count <= group_id)
                {
                    m_List.Add(null);
                }

                group = new Group();
                m_Dic.Add(group_name, group);
                m_List[group_id] = group;
            }

            group.register(type_name, type_id, function);
        }

        public static Group createGroup(string group_name, int group_id)
        {
            Group group = null;
            if (!m_Dic.TryGetValue(group_name, out group))
            {
                while (m_List.Count <= group_id)
                {
                    m_List.Add(null);
                }

                group = new Group();
                m_Dic.Add(group_name, group);
                m_List[group_id] = group;
            }

            return group;
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
                throw new System.Exception();
            }

            return (T)item;
        }

        public static TezItem create(string group_name, string type_name)
        {
            return m_Dic[group_name][type_name].create();
        }

        public static T create<T>(string group_name, string type_name) where T : TezItem
        {
            var item = m_Dic[group_name][type_name].create();

            if(!(item is T))
            {
                throw new System.Exception();
            }

            return (T)item;
        }
    }
}