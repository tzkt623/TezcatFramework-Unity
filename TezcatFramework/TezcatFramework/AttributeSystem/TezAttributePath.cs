using tezcat.Framework.Utility;

namespace tezcat.Framework.Attribute
{
    /// <summary>
    /// 属性路径信息
    /// </summary>
    public abstract class TezAttributePath : TezAttributeNode
    {
        public sealed override TezAttributeNodeType nodeType { get; } = TezAttributeNodeType.Path;
        public int childCount
        {
            get { return m_Children.count; }
        }
        TezArray<int> m_Children = new TezArray<int>(1);

        protected TezAttributePath(int id, ITezAttributeTree system) : base(id, system)
        {
        }

        public void addChild(int id)
        {
            m_Children.add(id);
        }

        protected TezAttributeNode getPrimaryNode(int id)
        {
            return this.system.getPrimaryNode(id);
        }

        protected TezAttributeLeaf getSecondaryNode(int id)
        {
            return this.system.getSecondaryNode(id);
        }

        public override void addAttributeDefObjectToChildren(ITezAttributeDefObject def_object)
        {
            for (int i = 0; i < this.childCount; i++)
            {
                var handler = this.getPrimaryNode(m_Children[i]);
                handler.addAttributeDefObjectToChildren(def_object);
            }
        }

        public override void removeAttributeDefObjectFromChildren(ITezAttributeDefObject def_object)
        {
            for (int i = 0; i < this.childCount; i++)
            {
                var handler = this.getPrimaryNode(m_Children[i]);
                handler.removeAttributeDefObjectFromChildren(def_object);
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