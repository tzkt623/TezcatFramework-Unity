using System.Collections.Generic;
using tezcat.Utility;

namespace tezcat.DataBase
{
    public sealed class TezDatabase
    {
        public static readonly TezDatabase instance = new TezDatabase();

        public abstract class GroupRTTI : TezRTTI
        {

        }

        public abstract class TypeRTTI : TezRTTI
        {

        }

        public int itemCount { get; private set; } = 0;

        class Group
        {
            public GroupRTTI groupRTTI { get; private set; }

            public int id { get; set; }
            public List<Container> containers { get; } = new List<Container>();

            public Group() { }
            public Group(GroupRTTI group_rtti) { this.groupRTTI = group_rtti; }

            public void registerContainer(TypeRTTI container_type)
            {
                while (this.containers.Count <= container_type.ID)
                {
                    this.containers.Add(null);
                }

                var container = new Container(container_type);
                this.containers[container_type.ID] = container;
            }

            public void foreachItem<T>(TezEventBus.Action<TypeRTTI> get_type, TezEventBus.Action<T> action) where T : ITezItem
            {
                foreach (var container in this.containers)
                {
                    get_type(container.typeRTTI);
                    container.foreachItem(action);
                }
            }

            public void foreachItem(TezEventBus.Action<ITezItem> action)
            {
                foreach (var container in this.containers)
                {
                    container.foreachItem(action);
                }
            }

            public void registerItem(ITezItem new_item)
            {
                int type_id = new_item.typeRTTI.ID;
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

            public void unregisterItem(ITezItem item)
            {
                this.containers[item.typeRTTI.ID].unregisterItem(item);
            }

            public void registerContainers<T>(List<T> types) where T : TypeRTTI
            {
                for (int i = 0; i < types.Count; i++)
                {
                    this.containers.Add(new Container(types[i]));
                }
            }
        }

        public class DatabaseSlot : TezSlot
        {
            public void registerItem(ITezItem item)
            {
                item.objectID = this.id;
                this.item = item;
            }
        }

        class Container : TezSlotSet<DatabaseSlot>
        {
            public TypeRTTI typeRTTI { get; private set; }

            public Container() { }
            public Container(TypeRTTI type_rtti) { this.typeRTTI = type_rtti; }

            public void registerItem(ITezItem new_item)
            {
                if (new_item.objectID == -1)
                {
                    this.add((DatabaseSlot slot) =>
                    {
                        slot.registerItem(new_item);
                    });
                }
                else
                {
                    this.grow(new_item.objectID);
                    this.set(new_item.objectID, (DatabaseSlot slot) =>
                    {
                        slot.item = new_item;
                    });
                }
            }

            public void unregisterItem(ITezItem item)
            {
                this.remove(item.objectID);
            }

            public void unregisterItem(int object_id)
            {
                this.remove(object_id);
            }

            public void foreachItem<T>(TezEventBus.Action<T> function) where T : ITezItem
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

            List<ITezItem> m_Items = new List<ITezItem>();

            Queue<int> m_FreeInnate = new Queue<int>();
            Queue<int> m_FreeRuntime = new Queue<int>();

            HashSet<int> m_Collide = new HashSet<int>();

            public int innateCount
            {
                get; private set;
            }

            int m_GUID = 0;

            public ITezItem this[int index]
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
            public void registerInnateItem(ITezItem item)
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
                            throw new System.Exception("ITezItem Collide!!!");
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
            public void unregisterInnateItem(ITezItem item)
            {
#if UNITY_EDITOR
                TezDebug.isTrue(item.GUID < innateCount, "TezDataBase", "This is not a Innate ITezItem");
#endif
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

            public void foreachItem(TezEventBus.Action<ITezItem> action)
            {
                foreach (var item in m_Items)
                {
                    if (item != null)
                    {
                        action(item);
                    }
                }
            }

            public void foreachRuntimeItem(TezEventBus.Action<ITezItem> action)
            {
                for (int i = this.innateCount; i < m_Items.Count; i++)
                {
                    action(m_Items[i]);
                }
            }

            public void foreachRuntimeItem<T>(TezEventBus.Action<T> action) where T : ITezItem
            {
                for (int i = this.innateCount; i < m_Items.Count; i++)
                {
                    action((T)m_Items[i]);
                }
            }

            /// <summary>
            /// 增加运行时物品
            /// </summary>
            /// <param name="item"></param>
            public void add(ITezItem item)
            {
                var guid = this.giveRuntimeGUID();
                item.GUID = guid;
                m_Items[guid] = item;
            }

            /// <summary>
            /// 移除运行时物品
            /// </summary>
            /// <param name="item"></param>
            public void remove(ITezItem item)
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

            public void registerRuntimeItem(ITezItem item)
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

        public void registerGroups<T>(List<T> groups) where T : GroupRTTI
        {
            for (int i = 0; i < groups.Count; i++)
            {
                m_Group.Add(new Group(groups[i]));
            }
        }

        public void registerTypes<T>(GroupRTTI group_type, List<T> types) where T : TypeRTTI
        {
            var group = m_Group[group_type.ID];
            group.registerContainers(types);
        }

        /// <summary>
        /// 取得一个物品
        /// </summary>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public ITezItem getItem(int GUID)
        {
            return m_Global[GUID];
        }

        /// <summary>
        /// 取得一个物品
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="GUID"></param>
        /// <returns></returns>
        public T getItem<T>(int GUID) where T : ITezItem
        {
            return (T)m_Global[GUID];
        }

        public ITezItem getItem(int group_id, int type_id, int object_id)
        {
            return m_Group[group_id][type_id][object_id];
        }

        public T getItem<T>(int group_id, int type_id, int object_id) where T : ITezItem
        {
            return (T)m_Group[group_id][type_id][object_id];
        }

        public bool tryGetItems(int group_id, int type_id, out List<DatabaseSlot> result)
        {
            result = m_Group[group_id][type_id].slots;
            return true;
        }

        /// <summary>
        /// 增加Runtime数据
        /// </summary>
        /// <param name="item"></param>
        public void addItem(ITezItem item)
        {
            if (item.GUID < 0)
            {
                m_Global.add(item);
                m_Group[item.groupRTTI.ID].registerItem(item);
            }
        }

        /// <summary>
        /// 删除Runtime数据
        /// </summary>
        /// <param name="item"></param>
        public void removeItem(ITezItem item)
        {
            if (item.GUID >= m_Global.innateCount)
            {
                m_Global.remove(item);
                m_Group[item.groupRTTI.ID].unregisterItem(item);
            }
        }

        /// <summary>
        /// 注册Runtime数据
        /// </summary>
        /// <param name="item"></param>
        public void registerRuntimeItem(ITezItem item)
        {
            if (item.GUID >= m_Global.innateCount)
            {
                m_Global.registerRuntimeItem(item);
                m_Group[item.groupRTTI.ID].registerItem(item);
            }
        }

        /// <summary>
        /// 注册Innate数据
        /// </summary>
        /// <param name="new_item"></param>
        public void registerInnateItem(ITezItem new_item)
        {
#if UNITY_EDITOR
            TezDebug.isTrue(
                m_Group.Count > new_item.groupRTTI.ID && m_Group[new_item.groupRTTI.ID] != null,
                "TezDataBase",
                "This ITezItem is out of range");
#endif
            m_Global.registerInnateItem(new_item);
            m_Group[new_item.groupRTTI.ID].registerItem(new_item);
        }

        /// <summary>
        /// 移除Innate数据
        /// </summary>
        /// <param name="item"></param>
        public void unregisterInnateItem(ITezItem item)
        {
            m_Group[item.groupRTTI.ID].unregisterItem(item);
            m_Global.unregisterInnateItem(item);
        }

        public void foreachItemByGroup<T>(
            TezEventBus.Action<GroupRTTI> get_group,
            TezEventBus.Action<TypeRTTI> get_type,
            TezEventBus.Action<T> get_item) where T : ITezItem
        {
            foreach (var group in m_Group)
            {
                get_group(group.groupRTTI);
                group.foreachItem(get_type, get_item);
            }
        }

        public void foreachItemByGroup(TezEventBus.Action<ITezItem> action)
        {
            foreach (var group in m_Group)
            {
                group.foreachItem(action);
            }
        }

        public void foreachItemByGUID(TezEventBus.Action<ITezItem> action)
        {
            m_Global.foreachItem(action);
        }

        public void foreachRuntimeItem<T>(TezEventBus.Action<T> action) where T : ITezItem
        {
            m_Global.foreachRuntimeItem(action);
        }

        public void foreachRuntimeItem(TezEventBus.Action<ITezItem> action)
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