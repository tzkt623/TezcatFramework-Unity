using tezcat.Framework.Core;
using tezcat.Framework.Utility;

namespace tezcat.Framework.BonusSystem
{
    /// <summary>
    /// 属性路径信息
    /// </summary>
    public abstract class TezBonusTreePathNode : TezBonusTreeNode
    {
        public sealed override TezBonusTreeNodeType nodeType { get; } = TezBonusTreeNodeType.Path;

        public int childCount
        {
            get { return mChildren.count; }
        }

        /// <summary>
        /// 存储的是总表中的index
        /// 用于查询
        /// 这个表只有在addBountyObjectToChildren时会用到
        /// </summary>
        TezStepArray<int> mChildren = new TezStepArray<int>(1);

        protected TezBonusTreePathNode(int id, ITezBonusTree tree) : base(id, tree)
        {

        }

        public void addChild(int id)
        {
            mChildren.add(id);
        }

        public override void addBonusObjectToChildren(ITezBonusCarrier obj)
        {
            for (int i = 0; i < this.childCount; i++)
            {
                var node = this.tree.getNode(mChildren[i]);
                node.addBonusObjectToChildren(obj);
            }
        }

        public override void removeBonusObjectFromChildren(ITezBonusCarrier obj)
        {
            for (int i = 0; i < this.childCount; i++)
            {
                var node = this.tree.getNode(mChildren[i]);
                node.removeBonusObjectFromChildren(obj);
            }
        }

        protected override void onClose()
        {
            base.onClose();
            mChildren.close();
            mChildren = null;
        }
    }
}