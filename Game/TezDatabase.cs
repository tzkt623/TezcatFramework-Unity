using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace tezcat
{
    public abstract class TezDatabase<Item> where Item : ITezItem
    {
        public int itemCount { get; private set; } = 0;

        class Group
        {
            public int id { get; set; }
            public List<Container> containers { get; } = new List<Container>();

            public void foreachItem(TezEventBus.Action<Item> action)
            {
                foreach (var container in this.containers)
                {
                    container.foreachItem(action);
                }
            }

            public void registerItem(Item new_item)
            {
                int type_id = new_item.typeID;
                while (this.containers.Count <= type_id)
                {
                    this.containers.Add(new Container());
                }

                this.containers[type_id].registerItem(new_item);
            }

            public void unregisterItem(int type_id, int object_id)
            {
                this.containers[type_id].unregisterItem(object_id);
            }

            public Container this[int type_id]
            {
                get { return this.containers[type_id]; }
            }

            public void clear()
            {
                foreach (var container in this.containers)
                {
                    container.clear();
                }
            }

            public void unregisterItem(Item item)
            {
                this.containers[item.typeID].unregisterItem(item);
            }
        }

        class Container
        {
            public List<Item> items { get; } = new List<Item>();

            public void registerItem(Item new_item)
            {
                if (new_item.objectID == -1)
                {
                    new_item.objectID = this.items.Count;
                    this.items.Add(new_item);
                }
                else
                {
                    while (this.items.Count <= new_item.objectID)
                    {
                        items.Add(default(Item));
                    }

                    this.items[new_item.objectID] = new_item;
                }
            }

            public void unregisterItem(Item item)
            {
                this.unregisterItem(item.objectID);
            }

            public void unregisterItem(int object_id)
            {
                if (object_id == items.Count - 1)
                {
                    this.items.RemoveAt(object_id);
                }
                else
                {
                    var remove_item = this.items[object_id];
                    var last_item = this.items[items.Count - 1];

                    this.items[object_id] = last_item;
                    last_item.objectID = object_id;
                    this.items.RemoveAt(items.Count - 1);
                }
            }

            public void foreachItem(TezEventBus.Action<Item> action)
            {
                foreach (var item in this.items)
                {
                    action(item);
                }
            }

            public void clear()
            {
                this.items.Clear();
            }

            public Item this[int object_id]
            {
                get { return this.items[object_id]; }
            }

        }

        class Global
        {
            List<Item> m_Items = new List<Item>();

            Stack<int> m_FreeInnate = new Stack<int>();
            Stack<int> m_FreeRuntime = new Stack<int>();

            public int innateCount
            {
                get; private set;
            }

            int m_GUID = 0;

            public Item this[int index]
            {
                get { return m_Items[index]; }
            }

            /// <summary>
            /// 注册固有物品
            /// </summary>
            /// <param name="item"></param>
            public void registerInnateItem(Item item)
            {
                if (item.GUID < 0)
                {
                    var guid = this.giveInnateGUID();
                    item.GUID = guid;
                    m_Items[item.GUID] = item;
                }
                else
                {
                    while (m_Items.Count <= item.GUID)
                    {
                        m_Items.Add(default(Item));
                    }

                    m_Items[item.GUID] = item;
                }

                innateCount += 1;
            }

            /// <summary>
            /// 移除固有物品
            /// </summary>
            /// <param name="item"></param>
            public void unregisterInnateItem(Item item)
            {
                Assert.IsTrue(item.GUID < innateCount);
                if (item.GUID == innateCount - 1)
                {
                    m_Items[item.GUID] = default(Item);
                    m_FreeInnate.Push(item.GUID);
                }
                else
                {
                    var last_innate = m_Items[innateCount - 1];
                    m_FreeInnate.Push(last_innate.GUID);
                    m_Items[item.GUID] = last_innate;
                    last_innate.GUID = item.GUID;
                }

                innateCount -= 1;
            }

            int giveInnateGUID()
            {
                if (m_FreeInnate.Count > 0)
                {
                    return m_FreeInnate.Pop();
                }
                else
                {
                    m_Items.Add(default(Item));
                    return m_GUID++;
                }
            }

            public void foreachItem(TezEventBus.Action<Item> action)
            {
                foreach (var item in m_Items)
                {
                    if (item != null)
                    {
                        action(item);
                    }
                }
            }

            public void foreachRuntimeItem(TezEventBus.Action<Item> action)
            {
                for (int i = this.innateCount; i < m_Items.Count; i++)
                {
                    action(m_Items[i]);
                }
            }

            /// <summary>
            /// 增加运行时物品
            /// </summary>
            /// <param name="item"></param>
            public void add(Item item)
            {
                var guid = this.giveRuntimeGUID();
                item.GUID = guid;
                m_Items[guid] = item;
            }

            /// <summary>
            /// 移除运行时物品
            /// </summary>
            /// <param name="item"></param>
            public void remove(Item item)
            {
                var guid = item.GUID;
                if (guid == m_Items.Count - 1)
                {
                    m_Items[guid] = default(Item);
                    m_FreeRuntime.Push(guid);
                }
                else
                {
                    var last_runtime = m_Items[m_Items.Count - 1];
                    m_FreeRuntime.Push(last_runtime.GUID);
                    m_Items[guid] = last_runtime;
                    last_runtime.GUID = guid;
                }
            }

            int giveRuntimeGUID()
            {
                if (m_FreeRuntime.Count > 0)
                {
                    return m_FreeInnate.Pop();
                }
                else
                {
                    m_Items.Add(default(Item));
                    return m_GUID++;
                }
            }
        }

        List<Group> m_Group = new List<Group>();
        Global m_Global = new Global();

        public int innateCount
        {
            get { return m_Global.innateCount; }
        }

        public void initialization(int container_count)
        {
            for (int i = 0; i < container_count; i++)
            {
                m_Group.Add(new Group());
            }
        }

        public Item getItem(int GUID)
        {
            return m_Global[GUID];
        }

        public T getItem<T>(int GUID) where T : Item
        {
            return (T)m_Global[GUID];
        }

        public Item getItem(int group_id, int type_id, int object_id)
        {
            return m_Group[group_id][type_id][object_id];
        }

        public T getItem<T>(int group_id, int type_id, int object_id) where T : Item
        {
            return (T)m_Group[group_id][type_id][object_id];
        }

        /// <summary>
        /// 增加运行时物品
        /// </summary>
        /// <param name="item"></param>
        public void addItem(Item item)
        {
            if (item.GUID < 0)
            {
                m_Global.add(item);
                m_Group[item.groupID].registerItem(item);
            }
        }

        /// <summary>
        /// 移除运行时物品
        /// </summary>
        /// <param name="item"></param>
        public void removeItem(Item item)
        {
            if (item.GUID >= m_Global.innateCount)
            {
                m_Global.remove(item);
                m_Group[item.groupID].unregisterItem(item);
            }
        }

        public void registerRuntimeItem(Item item)
        {

        }

        /// <summary>
        /// 注册数据库物品
        /// </summary>
        /// <param name="new_item"></param>
        public void registerInnateItem(Item new_item)
        {
            Assert.IsTrue(m_Group.Count > new_item.groupID && m_Group[new_item.groupID] != null);
            m_Global.registerInnateItem(new_item);
            m_Group[new_item.groupID].registerItem(new_item);
        }

        /// <summary>
        /// 移除数据库物品
        /// </summary>
        /// <param name="item"></param>
        public void unregisterInnateItem(Item item)
        {
            m_Group[item.groupID].unregisterItem(item);
            m_Global.unregisterInnateItem(item);
        }

        public void foreachItemByGroup(TezEventBus.Action<Item> action)
        {
            foreach (var group in m_Group)
            {
                group.foreachItem(action);
            }
        }

        public void foreachItemByGUID(TezEventBus.Action<Item> action)
        {
            m_Global.foreachItem(action);
        }

        public void clear()
        {
            foreach (var group in m_Group)
            {
                group.clear();
            }
        }

        public void save()
        {
            //             TezJsonWriter writer = new TezJsonWriter(true);
            // 
            //             foreach (var group in m_Group)
            //             {
            //                 group.foreachItem((Item item) =>
            //                 {
            //                     writer.beginObject();
            //                     item.serialization(writer);
            //                     writer.endObject();
            //                 });
            //             }
            // 
            //             writer.save("C:/Users/Administrator/Desktop/TBS/save1.json");
        }

        public void load()
        {
            //             TezJsonReader reader = new TezJsonReader();
            //             reader.load("C:/Users/Administrator/Desktop/TBS/save1.json");
            // 
            //             var count = reader.count();
            // 
            //             for (int i = 0; i < count; i++)
            //             {
            //                 reader.enter(i);
            // 
            //                 reader.enter("id");
            //                 var gid = reader.getInt("group_id");
            //                 var tid = reader.getInt("type_id");
            //                 reader.exit();
            // 
            //                 var item = TezItemFactory.create(gid, tid);
            //                 item.deserialization(reader);
            //                 this.registerItem(item);
            // 
            //                 reader.exit();
            //             }
        }
    }
}