using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using System.Linq;

namespace tezcat
{
    public abstract class TezDatabase<Item> where Item : class, ITezItem
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
            HashSet<int> m_Collide = new HashSet<int>();

            public void registerItem(Item new_item)
            {
                if (new_item.objectID == -1)
                {
                    new_item.objectID = this.items.Count;
                    this.items.Add(new_item);
                }
                else
                {
                    if(m_Collide.Contains(new_item.objectID))
                    {
                        Debug.LogAssertion("object id collide!!");
                    }
                    m_Collide.Add(new_item.objectID);

                    while (this.items.Count <= new_item.objectID)
                    {
                        items.Add(null);
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
                    var last_item = this.items[items.Count - 1];

                    this.items[object_id] = last_item;
                    last_item.objectID = object_id;
                    this.items.RemoveAt(items.Count - 1);
                }

                m_Collide.Remove(object_id);
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
            public TezDatabase<Item> database { get; set; }

            List<Item> m_Items = new List<Item>();

            Queue<int> m_FreeInnate = new Queue<int>();
            Queue<int> m_FreeRuntime = new Queue<int>();

            HashSet<int> m_Collide = new HashSet<int>();

            public int innateCount
            {
                get; private set;
            }

            int m_GUID = 0;

            public Item this[int index]
            {
                get { return m_Items[index]; }
            }

            int filterGUID(TezEventBus.Function<int> function)
            {
                int guid = -1;
                do
                {
                    guid = function();
                }
                while (m_Items[guid] != null);
                return guid;
            }

            /// <summary>
            /// 注册固有物品
            /// </summary>
            /// <param name="item"></param>
            public void registerInnateItem(Item item)
            {
                if (item.GUID < 0)
                {
                    ///过滤掉InnateID中的重复ID
                    var guid = this.filterGUID(this.giveInnateGUID);
                    item.GUID = guid;
                    m_Items[guid] = item;
                }
                else
                {
                    this.growInnate(item.GUID);
                    var temp = m_Items[item.GUID];
                    if (temp != null)
                    {
                        ///如果当前物品冲突了,优先让有ID的物品先设置
                        ///把无关的物品后移
                        if (m_Collide.Contains(item.GUID))
                        {
                            throw new System.Exception("Item Collide!!!");
                        }
                        m_Collide.Add(item.GUID);

                        temp.GUID = -1;
                        this.registerInnateItem(temp);
                    }

                    m_Items[item.GUID] = item;
                }

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
                    m_Items[item.GUID] = null;
                    m_FreeInnate.Enqueue(item.GUID);
                }
                else
                {
                    var last_id = innateCount - 1;
                    var last_innate = m_Items[last_id];
                    m_Items[last_id] = null;

                    m_FreeInnate.Enqueue(last_innate.GUID);
                    m_Items[item.GUID] = last_innate;
                    last_innate.GUID = item.GUID;
                }

                innateCount -= 1;
            }

            void growInnate(int guid)
            {
                while (m_Items.Count <= guid)
                {
                    m_FreeInnate.Enqueue(m_GUID++);
                    m_Items.Add(null);
                }

                this.innateCount = m_Items.Count;
            }

            int giveInnateGUID()
            {
                if (m_FreeInnate.Count > 0)
                {
                    return m_FreeInnate.Dequeue();
                }
                else
                {
                    m_Items.Add(null);
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
                    m_Items.RemoveAt(guid);
                    m_FreeRuntime.Enqueue(guid);
                }
                else
                {
                    var last_id = m_Items.Count - 1;
                    var last_runtime = m_Items[last_id];
                    m_Items.RemoveAt(last_id);

                    m_FreeRuntime.Enqueue(last_runtime.GUID);
                    m_Items[guid] = last_runtime;
                    last_runtime.GUID = guid;
                }
            }

            void growRuntime(int guid)
            {
                while (m_Items.Count <= guid)
                {
                    m_FreeRuntime.Enqueue(m_GUID++);
                    m_Items.Add(null);
                }
            }

            int giveRuntimeGUID()
            {
                if (m_FreeRuntime.Count > 0)
                {
                    return m_FreeRuntime.Dequeue();
                }
                else
                {
                    m_Items.Add(null);
                    return m_GUID++;
                }
            }

            public void registerRuntimeItem(Item item)
            {
                this.growRuntime(item.GUID);
                var guid = this.filterGUID(this.giveRuntimeGUID);
                m_Items[guid] = item;
            }

            public void clearZeroRefItem()
            {
                int i = this.innateCount;
                while (i < m_Items.Count)
                {
                    var item = m_Items[i];
                    if (item != null && item.refrence <= 0)
                    {
                        this.database.removeItem(item);
                    }
                    else
                    {
                        i++;
                    }
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
            m_Global.database = this;
            for (int i = 0; i < container_count; i++)
            {
                m_Group.Add(new Group());
            }
        }

        /// <summary>
        /// 取得一个物品
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public Item getItem(int GUID)
        {
            return m_Global[GUID];
        }

        /// <summary>
        /// 取得一个物品
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="GUID"></param>
        /// <returns></returns>
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
        /// 增加Runtime数据
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
        /// 删除Runtime数据
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

        /// <summary>
        /// 注册Runtime数据
        /// </summary>
        /// <param name="item"></param>
        public void registerRuntimeItem(Item item)
        {
            if (item.GUID >= m_Global.innateCount)
            {
                m_Global.registerRuntimeItem(item);
                m_Group[item.groupID].registerItem(item);
            }
        }

        /// <summary>
        /// 注册Innate数据
        /// </summary>
        /// <param name="new_item"></param>
        public void registerInnateItem(Item new_item)
        {
            Assert.IsTrue(m_Group.Count > new_item.groupID && m_Group[new_item.groupID] != null);
            m_Global.registerInnateItem(new_item);
            m_Group[new_item.groupID].registerItem(new_item);
        }

        /// <summary>
        /// 移除Innate数据
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

        public void foreachRuntimeItem(TezEventBus.Action<Item> action)
        {
            m_Global.foreachRuntimeItem(action);
        }

        public void clear()
        {
            foreach (var group in m_Group)
            {
                group.clear();
            }
        }

        /// <summary>
        /// 重新映射物品的GUID
        /// 如果物品的GUID小于old_innate_count值,则说明是Innate数据,不需要映射
        /// 如果大于等于old_innate_count值,说明是Runtime数据,重新映射
        /// </summary>
        /// <param name="old_innate_count">老版本的innate数据数量</param>
        /// <param name="guid">当前的GUID</param>
        /// <returns></returns>
        public int remapGUID(int old_innate_count, int guid)
        {
            if (guid < old_innate_count)
            {
                return guid;
            }
            else
            {
                return guid + (this.innateCount - old_innate_count);
            }
        }

        public void clearZeroRefItem()
        {
            m_Global.clearZeroRefItem();
        }
    }
}