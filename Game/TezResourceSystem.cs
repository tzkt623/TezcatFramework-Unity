using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace tezcat
{
    public class TezResourceSystem : TezSingleton<TezResourceSystem>
    {
        //ID分配器
        int m_UniqueIDGenerator = 0;
        public int itemCount
        {
            get { return m_UniqueIDGenerator; }
        }

        public class TezItemList : IEnumerable<TezItem>
        {
            public readonly int itemSize = 100;

            List<TezItem[]> m_ItemWithUniqueID = new List<TezItem[]>();
            List<TezItem[]> itemList
            {
                get { return m_ItemWithUniqueID; }
            }

            public TezItem this[int unique_id]
            {
                get
                {
                    var area = unique_id / itemSize;
                    if (area > m_ItemWithUniqueID.Count - 1)
                    {
                        return null;
                    }

                    var index = unique_id - area * itemSize;
                    return m_ItemWithUniqueID[area][index];
                }
            }

            public void registerItem(TezItem item)
            {
//                 var area = item.uniqueID / itemSize;
// 
//                 while (area > m_ItemWithUniqueID.Count - 1)
//                 {
//                     m_ItemWithUniqueID.Add(new TezItem[itemSize]);
//                 }
// 
//                 m_ItemWithUniqueID[area][item.uniqueID - itemSize * area] = item;
            }

            public void unregisterItem(int unique_id)
            {
                var area = unique_id / itemSize;
                var index = unique_id - area * itemSize;

                m_ItemWithUniqueID[area][index] = null;
            }

            public void resetItem(TezItem item)
            {
//                 var area = item.uniqueID / itemSize;
//                 var index = item.uniqueID - area * itemSize;
// 
//                 m_ItemWithUniqueID[area][index] = item;
            }

            IEnumerator<TezItem> IEnumerable<TezItem>.GetEnumerator()
            {
                return new ItemListEnum(this);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new ItemListEnum(this);
            }

            class ItemListEnum : IEnumerator<TezItem>
            {
                int m_Area = 0;
                int m_Index = -1;

                TezItemList m_List = null;

                public ItemListEnum(TezItemList list)
                {
                    m_List = list;
                }

                TezItem IEnumerator<TezItem>.Current
                {
                    get
                    {
                        return m_List.itemList[m_Area][m_Index];
                    }
                }

                object IEnumerator.Current
                {
                    get { return m_List.itemList[m_Area][m_Index]; }
                }

                void IDisposable.Dispose()
                {
                    m_List = null;
                }

                bool IEnumerator.MoveNext()
                {
                    m_Index += 1;
                    if (m_Index >= m_List.itemSize)
                    {
                        m_Index = 0;
                        m_Area += 1;
                        if (m_Area >= m_List.itemList.Count)
                        {
                            return false;
                        }
                    }

                    return true;
                }

                void IEnumerator.Reset()
                {
                    m_Index = -1;
                    m_Area = 0;
                }
            }
        }

        protected class TezItemGroup
        {
            private int m_GroupID;
            public int groupID
            {
                get { return m_GroupID; }
                set { m_GroupID = value; }
            }

            List<List<TezItem>> m_ItemsWithType = new List<List<TezItem>>();

            public List<TezItem> getAllItems()
            {
                List<TezItem> items = new List<TezItem>();
                foreach (var container in m_ItemsWithType)
                {
                    foreach (var item in container)
                    {
                        items.Add(item);
                    }
                }

                return items;
            }

            public void registerItem(TezItem new_item)
            {
                int type_id = new_item.resUID.type_id;
                while (m_ItemsWithType.Count <= type_id)
                {
                    m_ItemsWithType.Add(new List<TezItem>());
                }

                var container = m_ItemsWithType[type_id];
                new_item.resUID.setObjectID(container.Count);
                container.Add(new_item);
            }

            public void unregisterItem(int type_id, int self_id)
            {
                var type_container = m_ItemsWithType[type_id];

                var remove_item = type_container[self_id];
                var last_item = type_container[type_container.Count - 1];

                type_container[self_id] = last_item;
                type_container.RemoveAt(type_container.Count - 1);
            }

            public void resetItem(TezItem item)
            {
                m_ItemsWithType[item.resUID.type_id][item.resUID.object_id] = item;
            }

            public List<TezItem> this[int type_id]
            {
                get
                {
                    return m_ItemsWithType[type_id];
                }
            }
        }

        List<TezItemGroup> m_TezItemGroupList = new List<TezItemGroup>();

        public void initialization(int container_count)
        {
            for (int i = 0; i < container_count; i++)
            {
                m_TezItemGroupList.Add(new TezItemGroup() { groupID = m_TezItemGroupList.Count });
            }
        }

        protected TezItemGroup getGroup(int group_id)
        {
            return m_TezItemGroupList[group_id];
        }

        public TezItem getItem(int group_id, int type_id, int self_id)
        {
            return m_TezItemGroupList[group_id][type_id][self_id];
        }

        public T getItem<T>(int group_id, int type_id, int self_id) where T : TezItem, new()
        {
            return m_TezItemGroupList[group_id][type_id][self_id] as T;
        }

        public void registerItem<T>(TezJsonReader reader) where T : TezItem, new()
        {
            var item = new T();
            item.deserialization(reader);
            this.registerItem(item);
        }

        private void registerItem(TezItem new_item)
        {
            var group_id = new_item.resUID.group_id;

            Assert.IsTrue(m_TezItemGroupList.Count > group_id && m_TezItemGroupList[group_id] != null);

            m_TezItemGroupList[group_id].registerItem(new_item);
        }

        public void unregisterItem(int group_id, int type_id, int self_id)
        {
            var item = this.getItem(group_id, type_id, self_id);
            m_TezItemGroupList[group_id].unregisterItem(type_id, self_id);
        }

        private void resetItem(TezItem item)
        {
            m_TezItemGroupList[item.resUID.group_id].resetItem(item);
        }
    }
}