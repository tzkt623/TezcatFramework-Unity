using System.Collections.Generic;

namespace tezcat.Framework.BonusSystem
{
    public abstract class TezBonusTreeLeafNode : TezBonusTreeNode
    {
        public sealed override TezBonusTreeNodeType nodeType { get; } = TezBonusTreeNodeType.Leaf;

        List<ITezBonusObjectHandler> m_HandlerList = new List<ITezBonusObjectHandler>();

        protected TezBonusTreeLeafNode(int id, ITezBonusTree system) : base(id, system)
        {

        }

        public override void registerAgent(ITezBonusObjectHandler handler)
        {
            base.registerAgent(handler);
            m_HandlerList.Add(handler);
        }

        public override void unregisterAgent(ITezBonusObjectHandler handler)
        {
            base.unregisterAgent(handler);
            m_HandlerList.Remove(handler);
        }

        public override void addBonusObjectToChildren(ITezBonusObject obj)
        {
            for (int i = 0; i < m_HandlerList.Count; i++)
            {
                m_HandlerList[i].addBonusObject(obj);
            }
        }

        public override void removeBonusObjectFromChildren(ITezBonusObject obj)
        {
            for (int i = 0; i < m_HandlerList.Count; i++)
            {
                m_HandlerList[i].removeBonusObject(obj);
            }
        }

        public override void close()
        {
            base.close();
            m_HandlerList.Clear();
            m_HandlerList = null;
        }
    }
}