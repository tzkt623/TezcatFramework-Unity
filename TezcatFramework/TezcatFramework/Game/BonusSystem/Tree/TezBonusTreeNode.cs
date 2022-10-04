using System;
using System.Collections.Generic;

namespace tezcat.Framework.BonusSystem
{
    /// <summary>
    /// 加成树节点
    /// </summary>
    public abstract class TezBonusTreeNode : ITezBonusTreeNode
    {
        #region Pool
        static Queue<List<ITezBonusObject>> ListPool = new Queue<List<ITezBonusObject>>();
        static List<ITezBonusObject> create()
        {
            if (ListPool.Count > 0)
            {
                return ListPool.Dequeue();
            }

            return new List<ITezBonusObject>();
        }

        static void recycle(List<ITezBonusObject> list)
        {
            if (list == null)
            {
                return;
            }
            list.Clear();
            ListPool.Enqueue(list);
        }
        #endregion

        public int ID { get; }
        public abstract TezBonusTreeNodeType nodeType { get; }
        public ITezBonusTree tree { get; private set; } = null;

        /// <summary>
        /// 保存可能存在的加成者
        /// </summary>
        List<ITezBonusObject> m_Objects = null;

        protected TezBonusTreeNode(int id, ITezBonusTree tree)
        {
            this.ID = id;
            this.tree = tree;
        }

        public virtual void registerAgent(ITezBonusObjectHandler handler)
        {
            if (m_Objects != null)
            {
                for (int i = 0; i < m_Objects.Count; i++)
                {
                    handler.addBonusObject(m_Objects[i]);
                }
            }
        }

        public virtual void unregisterAgent(ITezBonusObjectHandler handler)
        {
            if (m_Objects != null)
            {
                for (int i = 0; i < m_Objects.Count; i++)
                {
                    handler.removeBonusObject(m_Objects[i]);
                }
            }
        }

        /// <summary>
        /// 添加一个BountyObject
        /// </summary>
        public void addBountyObject(ITezBonusObject obj)
        {
            if (m_Objects == null)
            {
                m_Objects = create();
            }
            m_Objects.Add(obj);
            this.addBonusObjectToChildren(obj);
        }

        /// <summary>
        /// 向子节点添加
        /// </summary>
        public abstract void addBonusObjectToChildren(ITezBonusObject obj);

        public void removeBountyObject(ITezBonusObject obj)
        {
            if (m_Objects.Remove(obj))
            {
                this.removeBonusObjectFromChildren(obj);
            }
            else
            {
                throw new Exception("removeBountyObject!!");
            }
        }

        /// <summary>
        /// 从子节点移除
        /// </summary>
        public abstract void removeBonusObjectFromChildren(ITezBonusObject obj);

        public virtual void close()
        {
            recycle(m_Objects);
            m_Objects = null;
            this.tree = null;
        }
    }
}