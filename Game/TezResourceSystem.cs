using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace tezcat
{
    public class TezItemSystem
    {
        //ID分配器
        int m_UniqueIDGenerator = 0;
        public int itemCount
        {
            get { return m_UniqueIDGenerator; }
        }

        class Group
        {
            public int id { get; set; }

            List<Container> m_List = new List<Container>();
            public List<Container> containers
            {
                get { return m_List; }
            }

            public void foreachItem(TezEventBus.Action<TezItem> action)
            {
                foreach (var container in m_List)
                {
                    container.foreachItem(action);
                }
            }

            public void registerItem(TezItem new_item)
            {
                int type_id = new_item.typeID;
                while (m_List.Count <= type_id)
                {
                    m_List.Add(new Container());
                }

                m_List[type_id].registerItem(new_item);
            }

            public void unregisterItem(int type_id, int object_id)
            {
                var container = m_List[type_id];
                container.unregisterItem(object_id);
            }

            public Container this[int type_id]
            {
                get { return m_List[type_id]; }
            }

            public void clear()
            {
                foreach (var container in m_List)
                {
                    container.clear();
                }
            }
        }

        class Container
        {
            List<TezItem> m_List = new List<TezItem>();
            public List<TezItem> items
            {
                get { return m_List; }
            }

            public void registerItem(TezItem new_item)
            {
                if (new_item.objectID == -1)
                {
                    new_item.setObjectID(m_List.Count);
                    m_List.Add(new_item);
                }
                else
                {
                    while (m_List.Count <= new_item.objectID)
                    {
                        m_List.Add(null);
                    }

                    m_List[new_item.objectID] = new_item;
                }
            }

            public void unregisterItem(int object_id)
            {
                var remove_item = m_List[object_id];
                var last_item = m_List[m_List.Count - 1];

                m_List[object_id] = last_item;
                m_List.RemoveAt(m_List.Count - 1);
            }

            public void foreachItem(TezEventBus.Action<TezItem> action)
            {
                foreach (var item in m_List)
                {
                    action(item);
                }
            }

            public void clear()
            {
                m_List.Clear();
            }

            public TezItem this[int object_id]
            {
                get { return m_List[object_id]; }
            }

        }

        List<Group> m_GroupList = new List<Group>();

        public void initialization(int container_count)
        {
            for (int i = 0; i < container_count; i++)
            {
                m_GroupList.Add(new Group());
            }
        }

        public TezItem getItem(int group_id, int type_id, int object_id)
        {
            return m_GroupList[group_id][type_id][object_id];
        }

        public T getItem<T>(int group_id, int type_id, int object_id) where T : TezItem
        {
            return m_GroupList[group_id][type_id][object_id] as T;
        }

        public void registerItem<T>(TezJsonReader reader) where T : TezItem, new()
        {
            var item = new T();
            item.deserialization(reader);
            this.registerItem(item);
        }

        public void registerItem(TezItem new_item)
        {
            var group_id = new_item.groupID;
            Assert.IsTrue(m_GroupList.Count > group_id && m_GroupList[group_id] != null);
            m_GroupList[group_id].registerItem(new_item);
        }

        public void unregisterItem(int group_id, int type_id, int object_id)
        {
            var item = this.getItem(group_id, type_id, object_id);
            m_GroupList[group_id].unregisterItem(type_id, object_id);
        }

        public void foreachItem(TezEventBus.Action<TezItem> action)
        {
            foreach (var group in m_GroupList)
            {
                group.foreachItem(action);
            }
        }

        public void clear()
        {
            foreach (var group in m_GroupList)
            {
                group.clear();
            }
        }

        public void save()
        {
            TezJsonWriter writer = new TezJsonWriter(true);

            foreach (var group in m_GroupList)
            {
                group.foreachItem((TezItem item) =>
                {
                    writer.beginObject();
                    item.serialization(writer);
                    writer.endObject();
                });
            }

            writer.save("C:/Users/Administrator/Desktop/TBS/save1.json");
        }

        public void load()
        {
            TezJsonReader reader = new TezJsonReader();
            reader.load("C:/Users/Administrator/Desktop/TBS/save1.json");

            var count = reader.count();

            for (int i = 0; i < count; i++)
            {
                reader.enter(i);

                reader.enter("id");
                var gid = reader.getInt("group_id");
                var tid = reader.getInt("type_id");
                reader.exit();

                var item = TezItemFactory.create(gid, tid);
                item.deserialization(reader);
                this.registerItem(item);

                reader.exit();
            }
        }
    }
}