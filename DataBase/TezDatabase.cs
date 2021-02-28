using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Database
{
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
        public virtual void register(TezDatabaseItem item)
        {
            item.onRegister(this.ID, m_ItemID++);
        }

        public abstract void close();
    }

    public class TezItemDatabase
    {
        public class Container
        {
            TezItemDatabase m_Database = null;
            List<int> m_IndexList = new List<int>();
            Dictionary<string, int> m_NameSearchDic = new Dictionary<string, int>();

            public IReadOnlyList<int> itemIndexList => m_IndexList;
            public int count => m_IndexList.Count;

            public Container(TezItemDatabase database)
            {
                m_Database = database;
            }

            public void add(string name, int posInDataBase)
            {
                m_IndexList.Add(posInDataBase);
                m_NameSearchDic.Add(name, posInDataBase);
                this.onAdd(posInDataBase);
            }

            protected virtual void onAdd(int posInDataBase)
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
            public TezDatabaseGameItem getFromContainer(int index)
            {
                return m_Database.get(m_IndexList[index]);
            }

            /// <summary>
            /// 使用Database中的Index直接获得Item
            /// </summary>
            public TezDatabaseGameItem getFromDatabase(int index)
            {
                return m_Database.get(index);
            }
        }

        List<TezDatabaseGameItem> m_ContainerWithID = new List<TezDatabaseGameItem>();
        Dictionary<string, int> m_ContainerWithName = new Dictionary<string, int>();

        List<Container> m_TagContainerList = new List<Container>();

        public void register(TezDatabaseGameItem item)
        {
            var id = m_ContainerWithID.Count;
            item.onRegister(0, id);
            m_ContainerWithID.Add(item);
            m_ContainerWithName.Add(item.NID, id);
            this.registerWithCategory(item, id);
            this.onRegister(item, id);
        }

        private void onRegister(TezDatabaseGameItem item, int id)
        {

        }

        /// <summary>
        /// 利用Category的UID来建立数据库分类系统
        /// </summary>
        private void registerWithCategory(TezDatabaseGameItem item, int id)
        {
            if (item.category != null)
            {
                var category = item.category;
                for (int i = 0; i < category.count; i++)
                {
                    var uid = category[i].UID;
                    while (uid >= m_TagContainerList.Count)
                    {
                        m_TagContainerList.Add(null);
                    }

                    Container container = m_TagContainerList[uid];
                    if (container == null)
                    {
                        container = this.createContainer(uid);
                        m_TagContainerList[uid] = container;
                    }

                    container.add(item.NID, id);
                }
            }
        }

        protected virtual Container createContainer(int tokenUID)
        {
            return new Container(this);
        }

        public TezDatabaseGameItem get(string name)
        {
            if (m_ContainerWithName.TryGetValue(name, out int pos))
            {
                return m_ContainerWithID[pos];
            }

            throw new Exception();
        }

        public TezDatabaseGameItem get(int index)
        {
            return m_ContainerWithID[index];
        }

        public Container getContainerFromTokenName(string tokenName)
        {
            return m_TagContainerList[TezCategorySystem.getToken(tokenName).UID];
        }

        public Container getContainerFromToken(ITezCategoryBaseToken baseToken)
        {
            return m_TagContainerList[baseToken.UID];
        }

        public virtual void close()
        {

        }
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

        public override void register(TezDatabaseItem item)
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