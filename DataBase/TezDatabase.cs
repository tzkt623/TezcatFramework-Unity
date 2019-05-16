using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Extension;
using UnityEngine.Assertions;

namespace tezcat.Framework.DataBase
{
    public sealed class TezDatabase : ITezService
    {
        class Subgroup
        {
            public string NID { get; set; }
            public int SGID { get; set; }

            Dictionary<string, TezDataBaseGameItem> m_Dic = new Dictionary<string, TezDataBaseGameItem>();
            List<TezDataBaseGameItem> m_List = new List<TezDataBaseGameItem>();

            public void add(TezDataBaseGameItem item)
            {
                Assert.IsFalse(m_Dic.ContainsKey(item.NID), string.Format("DataBase Item : {0} is added", item.NID));
                item.onAddToDB(m_List.Count);

                m_List.Add(item);
                m_Dic.Add(item.NID, item);
            }

            public TezDataBaseGameItem get(int item_id)
            {
                Assert.IsFalse(item_id > m_List.Count, string.Format("Database : [M : get(int item_id)] {0} out of index", item_id));
                return m_List[item_id];
            }

            public TezDataBaseGameItem get(string item_name)
            {
                TezDataBaseGameItem item = null;
                if(m_Dic.TryGetValue(item_name, out item))
                {
                    return item;
                }
                else
                {
                    throw new Exception(string.Format("This item[{0}/{1}] is not in DB!", this.NID, item_name));
                }
            }

            public void foreachItem(TezEventExtension.Action<TezDataBaseGameItem> for_item)
            {
                for (int i = 0; i < m_List.Count; i++)
                {
                    for_item(m_List[i]);
                }
            }

            public int getItemCount()
            {
                return m_List.Count;
            }

            public List<TezDataBaseGameItem> getAllItem()
            {
                return m_List;
            }

            public void close()
            {
                m_Dic.Clear();
                m_List.Clear();

                m_Dic = null;
                m_List = null;
            }
        }

        class Group
        {
            public string NID { get; set; }
            public int GID { get; set; }

            Dictionary<string, Subgroup> m_Dic = new Dictionary<string, Subgroup>();
            List<Subgroup> m_List = new List<Subgroup>();

            public void add(TezDataBaseGameItem item)
            {
                var sgid = item.detailedGroup.toID;
                Subgroup sub;
                if (!m_Dic.TryGetValue(item.detailedGroup.toName, out sub))
                {
                    sub = new Subgroup() { NID = item.detailedGroup.toName, SGID = sgid };
                    m_Dic.Add(item.detailedGroup.toName, sub);

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
                Subgroup subgroup = null;
                if(m_Dic.TryGetValue(sub_name, out subgroup))
                {
                    return subgroup.get(item_name);
                }
                else
                {
                    throw new Exception(string.Format("This subgroup[{0}/{1}] is not in DB!", this.NID, sub_name));
                }
            }

            public TezDataBaseGameItem get(int sub_id, int item_id)
            {
                return m_List[sub_id].get(item_id);
            }

            public TezDataBaseGameItem get(int sub_id, string item_name)
            {
                return m_List[sub_id].get(item_name);
            }

            public void close()
            {
                foreach (var item in m_List)
                {
                    item.close();
                }
                m_List.Clear();
                m_Dic.Clear();

                m_Dic = null;
                m_List = null;
            }

            public void foreachSubgroup(TezEventExtension.Action<string, int> for_subgroup, TezEventExtension.Action<TezDataBaseGameItem> for_item)
            {
                foreach (var subgroup in m_List)
                {
                    for_subgroup(subgroup.NID, subgroup.SGID);
                    subgroup.foreachItem(for_item);
                }
            }

            public int getItemCount(int sub_id)
            {
                return m_List[sub_id].getItemCount();
            }

            public int getSubGroupCount()
            {
                return m_List.Count;
            }

            public List<TezDataBaseGameItem> getAllItem()
            {
                List<TezDataBaseGameItem> list = new List<TezDataBaseGameItem>();

                foreach (var sub in m_List)
                {
                    if(sub != null)
                    {
                        list.AddRange(sub.getAllItem());
                    }
                }

                return list;
            }
        }

        Dictionary<string, Group> m_GroupDic = new Dictionary<string, Group>();
        List<Group> m_GroupList = new List<Group>();

        public void add(TezDataBaseGameItem item)
        {
            var gid = item.group.toID;

            Group group = null;
            if (!m_GroupDic.TryGetValue(item.group.toName, out group))
            {
                group = new Group() { NID = item.group.toName, GID = gid };
                m_GroupDic.Add(item.group.toName, group);

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

        public T get<T>(int group_id, int sub_id, int item_id = 0) where T : TezDataBaseGameItem
        {
            return (T)this.get(group_id, sub_id, item_id);
        }

        public T get<T>(int group_id, int sub_id, string item_name) where T : TezDataBaseGameItem
        {
            return (T)m_GroupList[group_id].get(sub_id, item_name);
        }

        public T get<T>(int group_id, string sub_name, string item_name) where T : TezDataBaseGameItem
        {
            return (T)m_GroupList[group_id].get(sub_name, item_name);
        }

        public TezDataBaseGameItem get(int group_id, int sub_id, int item_id)
        {
            return m_GroupList[group_id].get(sub_id, item_id);
        }

        public TezDataBaseGameItem get(string group_name, string sub_name, string item_name)
        {
            return m_GroupDic[group_name].get(sub_name, item_name);
        }

        public int getGroupCount()
        {
            return m_GroupList.Count;
        }

        public int getSubGroupCount(int group_id)
        {
            return m_GroupList[group_id].getSubGroupCount();
        }

        public int getItemCount(int group_id, int sub_id)
        {
            return m_GroupList[group_id].getItemCount(sub_id);
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

        public List<TezDataBaseGameItem> get(int group_id)
        {
            return m_GroupList[group_id].getAllItem();
        }

        public void close()
        {
            foreach (var group in m_GroupList)
            {
                group?.close();
            }

            m_GroupList.Clear();
            m_GroupDic.Clear();

            m_GroupList = null;
            m_GroupDic = null;
        }
    }
}