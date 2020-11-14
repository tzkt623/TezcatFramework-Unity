using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

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

    /// <summary>
    /// GameItem数据库基础类
    /// </summary>
    public abstract class TezItemDatabase : TezDatabase
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

        protected TezItemDatabase(int ID) : base(ID)
        {

        }

        public override void register(TezDatabaseItem item)
        {
            base.register(item);
            this.createSlot((TezDatabaseGameItem)item);
        }

        private void createSlot(TezDatabaseGameItem item)
        {
            var index = item.category.finalToken.index;
            while (m_Slots.Count <= index)
            {
                m_Slots.Add(new Slot());
            }

            m_Slots[index].add(item);
        }

        public TezDatabaseGameItem get(ITezCategoryFinalToken final_token, string name)
        {
            var slot = m_Slots[final_token.index];
            return slot.get(name);
        }

        public TezDatabaseGameItem get(ITezCategoryFinalToken final_token, int index)
        {
            var slot = m_Slots[final_token.index];
            return slot.get(index);
        }

        public override void close()
        {
            for (int i = 0; i < m_Slots.Count; i++)
            {
                m_Slots[i].close();
            }

            m_Slots.Clear();
            m_Slots = null;
        }
    }
}