using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Database
{
    /// <summary>
    /// 查找物品使用Table
    /// </summary>
    public class TezItemTable
    {
        List<TezDatabaseGameItem> m_List = new List<TezDatabaseGameItem>();
        Dictionary<string, TezDatabaseGameItem> m_NameSearchDic = new Dictionary<string, TezDatabaseGameItem>();

        public IReadOnlyList<TezDatabaseGameItem> itemList => m_List;
        public int count => m_List.Count;

        public int UID { get; }

        public TezItemTable(int uid)
        {
            this.UID = uid;
        }

        public void register(TezDatabaseGameItem item)
        {
            var item_id = TezIDCreator<TezItemDatabase>.next();
            //            item.onRegister(this.UID, item_id, m_List.Count);
            m_List.Add(item);
            m_NameSearchDic.Add(item.NID, item);
            //            this.onAdd(item.innerID);
        }

        protected virtual void onAdd(int innerID)
        {

        }

        public TezDatabaseGameItem get(string name)
        {
            if (m_NameSearchDic.TryGetValue(name, out TezDatabaseGameItem item))
            {
                return item;
            }

            throw new Exception();
        }

        /// <summary>
        /// 使用当前Container的Index间接获得Database中的Item
        /// </summary>
        public TezDatabaseGameItem get(int innerID)
        {
            return m_List[innerID];
        }
    }

    /// <summary>
    /// Database用于管理Table和注册物品
    /// </summary>
    public class TezItemDatabase
    {
        Dictionary<string, Tuple<TezItemTable, int>> m_ContainerWithName = new Dictionary<string, Tuple<TezItemTable, int>>();

        List<TezItemTable> m_TableList = new List<TezItemTable>();

        public void register(int tableID, TezDatabaseGameItem item)
        {
            var table = m_TableList[tableID];
            table.register(item);
            //            m_ContainerWithName.Add(item.NID, new Tuple<TezItemTable, int>(table, item.innerID));
            this.onRegister(item, table);
        }

        private void onRegister(TezDatabaseGameItem item, TezItemTable table)
        {

        }

        public TezItemTable getOrCreateTable(int tableID)
        {
            while (m_TableList.Count <= tableID)
            {
                m_TableList.Add(new TezItemTable(m_TableList.Count));
            }
            return m_TableList[tableID];
        }

        public TezItemTable getTable(int tableID)
        {
            return m_TableList[tableID];
        }

        public TezDatabaseGameItem get(string name)
        {
            if (m_ContainerWithName.TryGetValue(name, out var tuple))
            {
                return tuple.Item1.get(tuple.Item2);
            }

            throw new Exception(string.Format("This Item [{0}] Not Exist ", name));
        }

        public TezItemTable getTable(string tokenName)
        {
            return m_TableList[TezCategorySystem.getToken(tokenName).UID];
        }

        public TezItemTable getTable(ITezCategoryBaseToken baseToken)
        {
            return m_TableList[baseToken.UID];
        }

        public virtual void close()
        {

        }
    }

    public class TestTezDatabase
    {
        public void dosomething()
        {
            TezItemDatabase DB = new TezItemDatabase();
            var table1 = DB.getOrCreateTable(0);
        }
    }

    /// <summary>
    /// 数据库基础
    /// </summary>
    public abstract class TezDatabase : ITezService
    {
        public int ID { get; }

        int m_ItemID = 0;

        protected TezDatabase(int ID)
        {
            this.ID = ID;
        }

        /// <summary>
        /// 注册Item
        /// </summary>
        public virtual void register(TezDatabaseGameItem item)
        {
            item.onRegister(this.ID, m_ItemID++);
        }

        public abstract void close();
    }

    /// <summary>
    /// GameItem数据库基础类
    /// </summary>
    public abstract class TezItemDatabaseOOO : TezDatabase
    {
        class Slot
        {
            Dictionary<string, TezDatabaseGameItem> m_Dic = new Dictionary<string, TezDatabaseGameItem>();
            List<TezDatabaseGameItem> m_List = new List<TezDatabaseGameItem>();

            public void add(TezDatabaseGameItem item)
            {
                m_Dic.Add(item.NID, item);
                m_List.Add(item);
            }

            public TezDatabaseGameItem get(string name)
            {
                TezDatabaseGameItem item = null;
                if (m_Dic.TryGetValue(name, out item))
                {
                    return item;
                }

                throw new Exception(string.Format("{0} : Item[{1}] Not Registered", this.GetType().Name, name));
            }

            public TezDatabaseGameItem get(int index)
            {
                return m_List[index];
            }

            internal void close()
            {
                for (int i = 0; i < m_List.Count; i++)
                {
                    m_List[i].close();
                }

                m_List.Clear();
                m_Dic.Clear();

                m_Dic = null;
                m_List = null;
            }
        }

        List<Slot> m_Slots = new List<Slot>();

        Slot m_CurrentSlot = null;

        protected TezItemDatabaseOOO(int ID) : base(ID)
        {

        }

        public override void register(TezDatabaseGameItem item)
        {
            base.register(item);
            this.createSlot((TezDatabaseGameItem)item);
        }

        private void createSlot(TezDatabaseGameItem item)
        {
            //           var index = item.category.finalToken.finalIndexInRootToken;
            int index = 0;
            while (m_Slots.Count <= index)
            {
                m_Slots.Add(new Slot());
            }

            m_Slots[index].add(item);
        }

        public TezDatabaseGameItem get(ITezCategoryToken finalToken, string name)
        {
            var slot = m_Slots[finalToken.finalIndexInRootToken];
            return slot.get(name);
        }

        public TezDatabaseGameItem get(ITezCategoryToken finalToken, int index)
        {
            var slot = m_Slots[finalToken.finalIndexInRootToken];
            return slot.get(index);
        }

        /// <summary>
        /// 进入一个类型的数据库
        /// </summary>
        /// <param name="finalToken"></param>
        public void beginToken(ITezCategoryToken finalToken)
        {
            m_CurrentSlot = m_Slots[finalToken.finalIndexInRootToken];
        }

        /// <summary>
        /// 从一个类型的数据库中取得数据
        /// 请配合beginToken使用
        /// 不得单独使用
        /// </summary>
        public TezDatabaseGameItem get(int index)
        {
            return m_CurrentSlot.get(index);
        }

        /// <summary>
        /// 从一个数据类型库中退出
        /// 请配合beginToken成对使用
        /// </summary>
        public void endToken()
        {
            m_CurrentSlot = null;
        }

        public override void close()
        {
            for (int i = 0; i < m_Slots.Count; i++)
            {
                m_Slots[i].close();
            }

            m_Slots.Clear();
            m_Slots = null;
            m_CurrentSlot = null;
        }
    }
}