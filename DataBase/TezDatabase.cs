using System;
using System.Collections.Generic;
using tezcat.Core;
using tezcat.Extension;

namespace tezcat.DataBase
{
    public sealed class TezDatabase : ITezService
    {
        class SubGroup
        {
            public string NID { get; set; }
            public int SGID { get; set; }

            Dictionary<string, TezDataBaseGameItem> m_Dic = new Dictionary<string, TezDataBaseGameItem>();
            List<TezDataBaseGameItem> m_List = new List<TezDataBaseGameItem>();

            Stack<int> m_FreeID = new Stack<int>();
            int m_IDGiver = 0;

            public void add(TezDataBaseGameItem item)
            {
                if (m_Dic.ContainsKey(item.NID))
                {
                    throw new Exception(string.Format("Item : {0} is added", item.NID));
                }

                item.itemID = m_List.Count;
                item.retain();

                m_List.Add(item);
                m_Dic.Add(item.NID, item);
                m_IDGiver = m_List.Count;
            }

            public TezDataBaseGameItem get(int item_id)
            {
                return m_List[item_id];
            }

            public TezDataBaseGameItem get(string item_name)
            {
                return m_Dic[item_name];
            }

            public void foreachItem(TezEventExtension.Action<TezDataBaseGameItem> for_item)
            {
                for (int i = 0; i < m_List.Count; i++)
                {
                    for_item(m_List[i]);
                }
            }

            public TezDataBaseGameItem updataItem(TezDataBaseGameItem item)
            {
                TezDataBaseGameItem new_temp = item.birth();

                var id = item.itemID;
                if (item.release())
                {
                    m_FreeID.Push(id);
                }

                new_temp.itemID = m_FreeID.Count > 0 ? m_FreeID.Pop() : m_IDGiver++;
                return new_temp;
            }

            public void recycleItem(TezDataBaseGameItem item)
            {
                if (item.itemID >= m_List.Count)
                {
                    m_FreeID.Push(item.itemID);
                }
            }

            public void close()
            {

            }
        }

        class Group
        {
            public string NID { get; set; }
            public int GID { get; set; }

            Dictionary<string, SubGroup> m_Dic = new Dictionary<string, SubGroup>();
            List<SubGroup> m_List = new List<SubGroup>();

            public void add(TezDataBaseGameItem item)
            {
                var sgid = item.subgroup.toID;
                SubGroup sub;
                if (!m_Dic.TryGetValue(item.subgroup.NID, out sub))
                {
                    sub = new SubGroup() { NID = item.subgroup.NID, SGID = sgid };
                    m_Dic.Add(item.subgroup.NID, sub);

                    while (m_List.Count <= sgid)
                    {
                        m_List.Add(null);
                    }

                    m_List[sgid] = sub;
                }

                sub.add(item);
            }

            public void remove(string name)
            {

            }

            public TezDataBaseGameItem get(string sub_name, string item_name)
            {
                return m_Dic[sub_name].get(item_name);
            }

            public TezDataBaseGameItem get(int sub_id, int item_id)
            {
                return m_List[sub_id].get(item_id);
            }

            public void close()
            {
                m_Dic.Clear();
                m_Dic = null;
            }

            public void foreachSubgroup(TezEventExtension.Action<string, int> for_subgroup, TezEventExtension.Action<TezDataBaseGameItem> for_item)
            {
                foreach (var subgroup in m_List)
                {
                    for_subgroup(subgroup.NID, subgroup.SGID);
                    subgroup.foreachItem(for_item);
                }
            }

            public TezDataBaseGameItem updateItem(TezDataBaseGameItem game_item)
            {
                return m_List[game_item.subgroup.toID].updataItem(game_item);
            }

            public void recycleItem(TezDataBaseGameItem game_item)
            {
                m_List[game_item.subgroup.toID].recycleItem(game_item);
            }
        }

        Dictionary<string, Group> m_GroupDic = new Dictionary<string, Group>();
        List<Group> m_GroupList = new List<Group>();

        public void add(TezDataBaseGameItem item)
        {
            var gid = item.group.toID;

            Group group = null;
            if (!m_GroupDic.TryGetValue(item.group.NID, out group))
            {
                group = new Group() { NID = item.group.NID, GID = gid };
                m_GroupDic.Add(item.group.NID, group);

                while (m_GroupList.Count <= gid)
                {
                    m_GroupList.Add(null);
                }

                m_GroupList[gid] = group;
            }

            group.add(item);
        }

        public T get<T>(string group_name, string sub_name, string item_name) where T : TezDataBaseGameItem
        {
            return (T)this.get(group_name, sub_name, item_name);
        }

        public T get<T>(int group_id, int sub_id, int item_id) where T : TezDataBaseGameItem
        {
            return (T)this.get(group_id, sub_id, item_id);
        }

        public TezDataBaseGameItem get(int group_id, int sub_id, int item_id)
        {
            return m_GroupList[group_id].get(sub_id, item_id);
        }

        public TezDataBaseGameItem get(string group_name, string sub_name, string item_name)
        {
            return m_GroupDic[group_name].get(sub_name, item_name);
        }

        public void foreachDataBase(
            TezEventExtension.Action<string, int> for_group,
            TezEventExtension.Action<string, int> for_subgroup,
            TezEventExtension.Action<TezDataBaseGameItem> for_item)
        {
            for (int i = 0; i < m_GroupList.Count; i++)
            {
                var group = m_GroupList[i];
                for_group(group.NID, group.GID);
                group.foreachSubgroup(for_subgroup, for_item);
            }
        }

        public TezDataBaseGameItem updateItem(TezDataBaseGameItem game_item)
        {
            return m_GroupList[game_item.group.toID].updateItem(game_item);
        }

        public void recycleItem(TezDataBaseGameItem game_item)
        {
            m_GroupList[game_item.group.toID].recycleItem(game_item);
        }

        public void close()
        {
            foreach (var container in m_GroupList)
            {
                container?.close();
            }

            m_GroupList.Clear();
            m_GroupDic.Clear();

            m_GroupList = null;
            m_GroupDic = null;
        }
    }
}