using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.BonusSystem
{
    /// <summary>
    /// 加成树节点
    /// </summary>
    public abstract class TezBonusTreeNode : ITezBonusTreeNode
    {
        #region Pool
        static Queue<List<ITezBonusCarrier>> ListPool = new Queue<List<ITezBonusCarrier>>();
        static List<ITezBonusCarrier> create()
        {
            if (ListPool.Count > 0)
            {
                return ListPool.Dequeue();
            }

            return new List<ITezBonusCarrier>();
        }

        static void recycle(List<ITezBonusCarrier> list)
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
        List<ITezBonusCarrier> mObjects = null;

        protected TezBonusTreeNode(int id, ITezBonusTree tree)
        {
            this.ID = id;
            this.tree = tree;
        }

        public virtual void registerAgent(ITezBonusObjectHandler handler)
        {
            if (mObjects != null)
            {
                for (int i = 0; i < mObjects.Count; i++)
                {
                    handler.addBonusObject(mObjects[i]);
                }
            }
        }

        public virtual void unregisterAgent(ITezBonusObjectHandler handler)
        {
            if (mObjects != null)
            {
                for (int i = 0; i < mObjects.Count; i++)
                {
                    handler.removeBonusObject(mObjects[i]);
                }
            }
        }

        /// <summary>
        /// 添加一个BountyObject
        /// </summary>
        public void addBountyObject(ITezBonusCarrier obj)
        {
            if (mObjects == null)
            {
                mObjects = create();
            }
            mObjects.Add(obj);
            this.addBonusObjectToChildren(obj);
        }

        /// <summary>
        /// 向子节点添加
        /// </summary>
        public abstract void addBonusObjectToChildren(ITezBonusCarrier obj);

        public void removeBountyObject(ITezBonusCarrier obj)
        {
            if (mObjects.Remove(obj))
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
        public abstract void removeBonusObjectFromChildren(ITezBonusCarrier obj);

        void ITezCloseable.deleteThis()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {
            recycle(mObjects);
            mObjects = null;
            this.tree = null;
        }
    }
}