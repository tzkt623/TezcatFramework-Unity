using System.Collections.Generic;

namespace tezcat.Framework.BonusSystem
{
    public abstract class TezBonusTreeLeafNode : TezBonusTreeNode
    {
        public sealed override TezBonusTreeNodeType nodeType { get; } = TezBonusTreeNodeType.Leaf;

        List<ITezBonusObjectHandler> mHandlerList = new List<ITezBonusObjectHandler>();

        protected TezBonusTreeLeafNode(int id, ITezBonusTree system) : base(id, system)
        {

        }

        public override void registerAgent(ITezBonusObjectHandler handler)
        {
            base.registerAgent(handler);
            mHandlerList.Add(handler);
        }

        public override void unregisterAgent(ITezBonusObjectHandler handler)
        {
            base.unregisterAgent(handler);
            mHandlerList.Remove(handler);
        }

        public override void addBonusObjectToChildren(ITezBonusCarrier obj)
        {
            for (int i = 0; i < mHandlerList.Count; i++)
            {
                mHandlerList[i].addBonusObject(obj);
            }
        }

        public override void removeBonusObjectFromChildren(ITezBonusCarrier obj)
        {
            for (int i = 0; i < mHandlerList.Count; i++)
            {
                mHandlerList[i].removeBonusObject(obj);
            }
        }

        protected override void onClose()
        {
            base.onClose();
            mHandlerList.Clear();
            mHandlerList = null;
        }
    }
}