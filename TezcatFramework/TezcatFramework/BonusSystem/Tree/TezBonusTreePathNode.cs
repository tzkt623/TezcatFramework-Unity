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
            get { return m_Children.count; }
        }

        /// <summary>
        /// 存储的是总表中的index
        /// 用于查询
        /// 这个表只有在addBountyObjectToChildren时会用到
        /// </summary>
        TezStepArray<int> m_Children = new TezStepArray<int>(1);

        protected TezBonusTreePathNode(int id, ITezBonusTree tree) : base(id, tree)
        {

        }

        public void addChild(int id)
        {
            m_Children.add(id);
        }

        public override void addBonusObjectToChildren(ITezBonusObject obj)
        {
            for (int i = 0; i < this.childCount; i++)
            {
                var node = this.tree.getNode(m_Children[i]);
                node.addBonusObjectToChildren(obj);
            }
        }

        public override void removeBonusObjectFromChildren(ITezBonusObject obj)
        {
            for (int i = 0; i < this.childCount; i++)
            {
                var node = this.tree.getNode(m_Children[i]);
                node.removeBonusObjectFromChildren(obj);
            }
        }

        public override void close()
        {
            base.close();
            m_Children.close();
            m_Children = null;
        }
    }
}