using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
//     public class TezFastDatabase : ITezService
//     {
//         public class Subgroup
//         {
//             List<TezDatabaseGameItem> m_List = new List<TezDatabaseGameItem>();
//             Dictionary<string, TezDatabaseGameItem> m_Dic = new Dictionary<string, TezDatabaseGameItem>();
// 
//             public void add(TezDatabaseGameItem item)
//             {
//                 if (m_Dic.ContainsKey(item.NID))
//                 {
//                     throw new Exception(string.Format("TezFastDatabase : {0}[{1}] already existed", item.NID, item.GetType().Name));
//                 }
// 
//                 item.onAddToDB(m_List.Count);
//                 m_List.Add(item);
//                 m_Dic.Add(item.NID, item);
//             }
// 
//             public TItem get<TItem>(string name) where TItem : TezDatabaseGameItem
//             {
//                 return (TItem)m_Dic[name];
//             }
//         }
// 
//         public class Group
//         {
//             List<Subgroup> m_SubgroupList = new List<Subgroup>();
// 
//             public void add(TezDatabaseGameItem item)
//             {
//                 var id = item.subgroup.toID;
//                 while (id >= m_SubgroupList.Count)
//                 {
//                     m_SubgroupList.Add(new Subgroup());
//                 }
// 
//                 m_SubgroupList[id].add(item);
//             }
// 
//             public TItem get<TItem>(int subclass_id, string name) where TItem : TezDatabaseGameItem
//             {
//                 return m_SubgroupList[subclass_id].get<TItem>(name);
//             }
//         }
// 
//         List<Group> m_GroupList = new List<Group>();
// 
//         public void add(TezDatabaseGameItem item)
//         {
//             var id = item.group.toID;
//             while (id >= m_GroupList.Count)
//             {
//                 m_GroupList.Add(new Group());
//             }
// 
//             m_GroupList[id].add(item);
//         }
// 
//         public TItem get<TItem>(int class_id, int subclass_id, string name) where TItem : TezDatabaseGameItem
//         {
//             return m_GroupList[class_id].get<TItem>(subclass_id, name);
//         }
// 
//         public void close(bool self_close = true)
//         {
// 
//         }
//     }
}