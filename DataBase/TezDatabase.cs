using System;
using System.Collections.Generic;
using tezcat.Core;
using tezcat.Event;
using tezcat.TypeTraits;

namespace tezcat.DataBase
{
    public sealed class TezDatabase
    {
        public static TezEvent<ContainerSlot> onRegsiterItem { get; } = new TezEvent<ContainerSlot>();

        /// <summary>
        /// 组别类型
        /// </summary>
        public abstract class GroupType : TezType
        {

        }

        /// <summary>
        /// 分类类型
        /// </summary>
        public abstract class CategoryType : TezType
        {
            public TezEventBus.Function<TezItem> function { get; private set; } = null;

            void setCreator(TezEventBus.Function<TezItem> function)
            {
                this.function = function;
            }

            public TezItem create()
            {
                return function();
            }

            public T create<T>() where T : TezItem
            {
                return (T)function();
            }

            protected static T initType<T>(T e, string name, TezEventBus.Function<TezItem> function) where T : CategoryType, new()
            {
                if (e == null)
                {
                    var temp = TezTypeRegister<T>.register(name);
                    temp.setCreator(function);
                    return temp;
                }

                return e;
            }
        }

        class Group
        {
            public GroupType groupType { get; private set; }

            public int id { get; set; }
            public List<Container> containers { get; } = new List<Container>();

            public Group() { }
            public Group(GroupType group_type) { this.groupType = group_type; }

            public void registerContainer(CategoryType container_type)
            {
                while (this.containers.Count <= container_type.ID)
                {
                    this.containers.Add(null);
                }

                var container = new Container(container_type);
                this.containers[container_type.ID] = container;
            }

            public void foreachItem<T>(TezEventBus.Action<CategoryType> get_type, TezEventBus.Action<T> action) where T : TezItem
            {
                foreach (var container in this.containers)
                {
                    get_type(container.categoryType);
                    container.foreachItem(action);
                }
            }

            public void foreachCategoryType(TezEventBus.Action<CategoryType> get_type)
            {
                foreach (var container in this.containers)
                {
                    get_type(container.categoryType);
                }
            }

            public void foreachItem(TezEventBus.Action<TezItem> action)
            {
                foreach (var container in this.containers)
                {
                    container.foreachItem(action);
                }
            }

            public void registerItem(TezItem new_item)
            {
                int type_id = new_item.categoryType.ID;
                while (this.containers.Count <= type_id)
                {
                    this.containers.Add(new Container());
                }

                this.containers[type_id].registerItem(new_item);
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

            public void unregisterItem(TezItem item)
            {
                this.containers[item.categoryType.ID].unregisterItem(item);
            }

            public void registerContainers<T>(List<T> types) where T : CategoryType
            {
                for (int i = 0; i < types.Count; i++)
                {
                    this.containers.Add(new Container(types[i]));
                    TezDatabaseItemFactory.getGroup(groupType.ID)
                        .create(types[i].name, types[i].ID, types[i].function);
                }
            }

            public void addItem(TezItem item)
            {

            }
        }

        public class ContainerSlot : TezItemSlot
        {
            public void registerItem(TezItem item)
            {
                item.OID = this.ID;
                this.item = item;
            }
        }

        class Container : TezItemSlotManager<ContainerSlot>
        {
            public CategoryType categoryType { get; private set; }

            public Container() { }
            public Container(CategoryType category_type) { this.categoryType = category_type; }

            public void registerItem(TezItem item)
            {
                if (item.OID == -1)
                {
                    this.add((ContainerSlot slot) =>
                    {
                        slot.registerItem(item);
                        onRegsiterItem.invoke(slot);
                    });
                }
                else
                {
                    this.grow(item.OID);
                    this.set(item.OID, (ContainerSlot slot) =>
                    {
                        slot.item = item;
                        onRegsiterItem.invoke(slot);
                    });
                }
            }

            public void unregisterItem(TezItem item)
            {
                this.remove(item.OID);
            }

            public void foreachItem<T>(TezEventBus.Action<T> function) where T : TezItem
            {
                foreach (var slot in slots)
                {
                    function((T)slot.item);
                }
            }
        }

        class Global
        {
            public TezDatabase database { get; set; }

            List<TezItem> m_Items = new List<TezItem>();
            Queue<int> m_FreeInnate = new Queue<int>();

            public TezItem this[int index]
            {
                get { return m_Items[index]; }
            }

            /// <summary>
            /// 注册固有物品
            /// </summary>
            /// <param name="item"></param>
            public void registerItem(TezItem item)
            {
                if(item.GUID > 0)
                {
                    this.grow(item.GUID);
                }
                else
                {
                    item.GUID = this.giveGUID();
                }

                m_Items[item.GUID] = item;
            }

            /// <summary>
            /// 移除固有物品
            /// </summary>
            /// <param name="item"></param>
            public void unregisterItem(TezItem item)
            {
                if (item.GUID == m_Items.Count - 1)
                {
                    m_Items.RemoveAt(item.GUID);
                }
                else
                {
                    var last_id = m_Items.Count - 1;
                    var last_innate = m_Items[last_id];
                    m_Items.RemoveAt(last_id);

                    m_Items[item.GUID] = last_innate;
                    last_innate.GUID = item.GUID;
                }
            }

            void grow(int guid)
            {
                while (m_Items.Count <= guid)
                {
                    m_FreeInnate.Enqueue(m_Items.Count);
                    m_Items.Add(null);
                }
            }

            int giveGUID()
            {
                if (m_FreeInnate.Count > 0)
                {
                    return m_FreeInnate.Dequeue();
                }
                else
                {
                    int id = m_Items.Count;
                    m_Items.Add(null);
                    return id++;
                }
            }

            public void foreachItem(TezEventBus.Action<TezItem> action)
            {
                foreach (var item in m_Items)
                {
                    if (item != null)
                    {
                        action(item);
                    }
                }
            }

            public void sortItems()
            {
                List<TezItem> temp = new List<TezItem>(m_Items);
                m_Items.Clear();

                for (int i = 0; i < temp.Count; i++)
                {
                    var item = temp[i];
                    if (item)
                    {
                        item.GUID = m_Items.Count;
                        m_Items.Add(item);
                    }
                }

                temp.Clear();
                temp = null;
            }

            public void addItem(TezItem item)
            {
                this.grow(item.GUID);
                m_Items[item.GUID] = item;
            }
        }

        static List<Group> m_Group = new List<Group>();
        static Global m_Global = new Global();

        public static void initialization(int container_count)
        {
            for (int i = 0; i < container_count; i++)
            {
                m_Group.Add(new Group());
            }
        }

        public static void registerGroups<T>(List<T> groups) where T : GroupType
        {
            for (int i = 0; i < groups.Count; i++)
            {
                m_Group.Add(new Group(groups[i]));
                TezDatabaseItemFactory.createGroup(groups[i].name, groups[i].ID);
            }
        }

        public static void registerCategories<T>(GroupType group_type, List<T> types) where T : CategoryType
        {
            var group = m_Group[group_type.ID];
            group.registerContainers(types);
        }

        /// <summary>
        /// 取得一个物品
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public static TezItem getItem(int GUID)
        {
            return m_Global[GUID];
        }

        /// <summary>
        /// 取得一个物品
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public static T getItem<T>(int GUID) where T : TezItem
        {
            return (T)m_Global[GUID];
        }

        public static TezItem getItem(int group_id, int type_id, int object_id)
        {
            ContainerSlot slot = null;
            if (m_Group[group_id][type_id].tryGet(object_id, out slot))
            {
                return slot.item;
            }

            return null;
        }

        public static T getItem<T>(int group_id, int type_id, int object_id) where T : TezItem
        {
            ContainerSlot slot = null;
            if (m_Group[group_id][type_id].tryGet(object_id, out slot))
            {
                return (T)slot.item;
            }

            return null;
        }

        public static List<ContainerSlot> getItems(int group_id, int type_id)
        {
            return m_Group[group_id][type_id].slots;
        }

        /// <summary>
        /// 注册Innate数据
        /// </summary>
        /// <param name="item"></param>
        public static void registerItem(TezItem item)
        {
#if UNITY_EDITOR
            TezDebug.isTrue(
                m_Group.Count > item.groupType.ID && m_Group[item.groupType.ID] != null,
                "TezDataBase",
                "This TezItem is out of range");
#endif
            m_Global.registerItem(item);
            m_Group[item.groupType.ID].registerItem(item);
        }

        /// <summary>
        /// 移除Innate数据
        /// </summary>
        /// <param name="item"></param>
        public static void unregisterItem(TezItem item)
        {
            m_Group[item.groupType.ID].unregisterItem(item);
            m_Global.unregisterItem(item);
        }

        public static void foreachCategoryType(
            TezEventBus.Action<GroupType> get_group,
            TezEventBus.Action<CategoryType> get_type)
        {
            foreach (var group in m_Group)
            {
                get_group(group.groupType);
                group.foreachCategoryType(get_type);
            }
        }

        public static void foreachItemByGroup<T>(
            TezEventBus.Action<GroupType> get_group,
            TezEventBus.Action<CategoryType> get_type,
            TezEventBus.Action<T> get_item) where T : TezItem
        {
            foreach (var group in m_Group)
            {
                get_group(group.groupType);
                group.foreachItem(get_type, get_item);
            }
        }

        public static void foreachItemByGroup(TezEventBus.Action<TezItem> action)
        {
            foreach (var group in m_Group)
            {
                group.foreachItem(action);
            }
        }

        public static void foreachItemByGUID(TezEventBus.Action<TezItem> action)
        {
            m_Global.foreachItem(action);
        }

        public static void sortItems()
        {
            m_Global.sortItems();
        }

        public static void clear()
        {
            foreach (var group in m_Group)
            {
                group.clear();
            }
        }
    }
}