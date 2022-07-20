using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    /// <summary>
    /// 
    /// 建议使用
    /// TezMultDatabase<Item>
    /// 
    /// <para>
    /// 1.物品数据库不会有重名的物品存在
    /// </para>
    /// 
    /// <para>
    /// 2.如果有重名的单位,用后缀作为区别
    ///     例如xxx_001,xxx_aab
    /// </para>
    /// 
    /// <para>
    /// 3.数据库的每一件物品只有唯一的ID
    /// </para>
    /// 
    /// <para>
    /// 物品定死类,既不支持自定义物品属性的游戏(例如万智牌等)
    /// 每一个物品对应一个唯一的ID
    /// 保存时只需要保存物品对应ID和名称,以及对应拥有多少个就可以了
    /// </para>
    /// 
    /// <para>
    /// 自定义物品类,数据库保存的物品只能算做模板物品
    /// 玩家在保存时,需要保存每个物品的详细信息
    /// </para>
    /// 
    /// <para>
    /// 混合上面两种
    /// 是模板物品就保存物品名称和ID
    /// 不是模板物品就保存所有信息
    /// </para>
    /// 
    /// </summary>
    /// 

    public interface ITezDatabase
    {
        void registerItem(TezDatabaseGameItem item);
        TezDatabaseGameItem getItem(int uid);
        TezDatabaseGameItem getItem(string nid);
    }

    public class TezDatabase : ITezDatabase
    {
        protected List<TezDatabaseGameItem> m_ItemList = new List<TezDatabaseGameItem>();
        protected Dictionary<string, TezDatabaseGameItem> m_ItemDict = new Dictionary<string, TezDatabaseGameItem>();

        public void registerItem(TezDatabaseGameItem item)
        {
            if (m_ItemDict.ContainsKey(item.NID))
            {
                throw new Exception(string.Format("{0} : Item {1} had registered!!", this.GetType().Name, item.NID));
            }

            item.onRegister(m_ItemList.Count);
            m_ItemList.Add(item);
            m_ItemDict.Add(item.NID, item);
        }

        public TezDatabaseGameItem getItem(int uid)
        {
            return m_ItemList[uid];
        }

        public TezDatabaseGameItem getItem(string nid)
        {
            return m_ItemDict[nid];
        }
    }

    public interface ITezMultDatabase : ITezDatabase
    {
        int UID { get; }
    }

    public static class TezMultDatabase
    {
        static List<ITezMultDatabase> m_DatabaseList = new List<ITezMultDatabase>();

        public static void register(ITezMultDatabase db)
        {
            while (m_DatabaseList.Count <= db.UID)
            {
                m_DatabaseList.Add(null);
            }

            if (m_DatabaseList[db.UID] != null)
            {
                throw new Exception("This Database ID already existed");
            }

            m_DatabaseList[db.UID] = db;
        }

        public static ITezMultDatabase get(int id)
        {
            return m_DatabaseList[id];
        }
    }


    /// <summary>
    /// 多数据库存储模式
    /// </summary>
    /// <typeparam name="Item"></typeparam>
    public class TezMultDatabase<Item>
        : ITezMultDatabase
        where Item : TezDatabaseGameItem
    {
        public int UID { get; }

        protected Dictionary<string, Item> m_ItemDict = new Dictionary<string, Item>();
        protected List<Item> m_ItemList = new List<Item>();

        public TezMultDatabase(int uid)
        {
            this.UID = uid;
            TezMultDatabase.register(this);
        }

        public void registerItem(Item item)
        {
            if (m_ItemDict.ContainsKey(item.NID))
            {
                throw new Exception(string.Format("{0} : Item {1} had registered!!", this.GetType().Name, item.NID));
            }

            item.onRegister(this.UID, m_ItemList.Count);
            m_ItemList.Add(item);
            m_ItemDict.Add(item.NID, item);
        }

        public Item getItem(int uid)
        {
            return m_ItemList[uid];
        }

        public Item getItem(string nid)
        {
            return m_ItemDict[nid];
        }

        TezDatabaseGameItem ITezDatabase.getItem(int uid)
        {
            return m_ItemList[uid];
        }

        TezDatabaseGameItem ITezDatabase.getItem(string nid)
        {
            return m_ItemDict[nid];
        }

        void ITezDatabase.registerItem(TezDatabaseGameItem item)
        {
            this.registerItem((Item)item);
        }
    }
}