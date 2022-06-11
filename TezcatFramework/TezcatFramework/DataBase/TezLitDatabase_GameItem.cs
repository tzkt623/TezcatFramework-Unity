using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    public static class TezLitDatabase
    {
        static Dictionary<string, TezLitDatabase_GameItem> m_ItemDatabaseDic = new Dictionary<string, TezLitDatabase_GameItem>();
        static List<TezLitDatabase_GameItem> m_ItemDatabaseList = new List<TezLitDatabase_GameItem>();

        public static int register(TezLitDatabase_GameItem db)
        {
            int uid = m_ItemDatabaseList.Count;
            m_ItemDatabaseDic.Add(db.name, db);
            m_ItemDatabaseList.Add(db);
            return uid;
        }

        public static TezLitDatabase_GameItem getItemDB(string name)
        {
            return m_ItemDatabaseDic[name];
        }

        public static TezLitDatabase_GameItem getItemDB(int uid)
        {
            return m_ItemDatabaseList[uid];
        }
    }

    public class TezLitDatabase_GameItem : ITezCloseable
    {
        public class Table
        {
            List<int> m_List = new List<int>();
            Dictionary<string, int> m_NameSearchDic = new Dictionary<string, int>();
            TezLitDatabase_GameItem m_Database = null;

            public IReadOnlyList<int> itemList => m_List;
            public int count => m_List.Count;

            public Table(TezLitDatabase_GameItem database)
            {
                m_Database = database;
            }

            public void register(string nid, int uid)
            {
                var index = m_List.Count;
                m_List.Add(uid);
                m_NameSearchDic.Add(nid, uid);
                this.onAdd(index);
            }

            protected virtual void onAdd(int index)
            {

            }

            public TezDatabaseGameItem get(string name)
            {
                if (m_NameSearchDic.TryGetValue(name, out int index))
                {
                    return m_Database.get(index);
                }

                throw new Exception();
            }

            /// <summary>
            /// 使用当前Container的Index间接获得Database中的Item
            /// </summary>
            public TezDatabaseGameItem get(int index)
            {
                return m_Database.get(m_List[index]);
            }
        }

        public string name { get; }
        public int UID { get; }

        List<Table> m_TableList = new List<Table>();
        List<TezDatabaseGameItem> m_List = new List<TezDatabaseGameItem>();
        Dictionary<string, int> m_ContainerWithName = new Dictionary<string, int>();

        protected TezLitDatabase_GameItem(string name)
        {
            this.name = name;
            this.UID = TezLitDatabase.register(this);
        }

        public void register(TezDatabaseGameItem item)
        {
            this.registerWithCategory(item);
            this.onRegister(item);
        }

        private void onRegister(TezDatabaseGameItem item)
        {

        }

        /// <summary>
        /// 利用Category的UID来建立数据库分类系统
        /// </summary>
        private void registerWithCategory(TezDatabaseGameItem item)
        {
            if (item.category == null)
            {
                throw new Exception(string.Format("This Item [{0}] Dont Has [Category]", item.NID));
            }

            var item_uid = m_List.Count;
            item.onRegister(this.UID, item_uid);

            var category = item.category;
            for (int i = 0; i < category.count; i++)
            {
                var table_id = category[i].UID;
                while (table_id >= m_TableList.Count)
                {
                    m_TableList.Add(null);
                }

                Table table = m_TableList[table_id];
                if (table == null)
                {
                    table = this.createTable(item);
                    m_TableList[table_id] = table;
                }

                table.register(item.NID, item_uid);
            }

            m_List.Add(item);
            m_ContainerWithName.Add(item.NID, item_uid);
        }

        protected virtual Table createTable(TezDatabaseGameItem item)
        {
            return new Table(this);
        }

        public TezDatabaseGameItem get(string name)
        {
            if (m_ContainerWithName.TryGetValue(name, out var index))
            {
                return m_List[index];
            }

            throw new Exception(string.Format("This Item [{0}] Not Exist ", name));
        }

        public TezDatabaseGameItem get(int index)
        {
            return m_List[index];
        }

        public Table getTable(string tokenName)
        {
            return m_TableList[TezCategorySystem.getToken(tokenName).UID];
        }

        public Table getTable(ITezCategoryBaseToken baseToken)
        {
            return m_TableList[baseToken.UID];
        }

        public virtual void close()
        {

        }
    }
}