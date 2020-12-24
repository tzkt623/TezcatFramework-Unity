using tezcat.Framework.Utility;

namespace tezcat.Framework.Definition
{
    public abstract class TezDefinitionPath : TezDefinitionNode
    {
        public sealed override TezDefinitionNodeType nodeType { get; } = TezDefinitionNodeType.Path;
        public int childCount
        {
            get { return m_Children.count; }
        }
        TezArray<int> m_Children = new TezArray<int>(1);

        protected TezDefinitionPath(int id, TezDefinitionSystem set) : base(id, set)
        {
        }

        public void addChild(int id)
        {
            m_Children.add(id);
        }

        protected TezDefinitionNode getPrimaryNode(int id)
        {
            return this.system.getPrimaryNode(id);
        }

        protected TezDefinitionLeaf getSecondaryNode(int id)
        {
            return this.system.getSecondaryNode(id);
        }

        public override void addDefinitionObjectToChildren(ITezDefinitionObject def_object)
        {
            for (int i = 0; i < this.childCount; i++)
            {
                var handler = this.getPrimaryNode(m_Children[i]);
                handler.addDefinitionObjectToChildren(def_object);
            }
        }

        public override void removeDefinitionObjectFromChildren(ITezDefinitionObject def_object)
        {
            for (int i = 0; i < this.childCount; i++)
            {
                var handler = this.getPrimaryNode(m_Children[i]);
                handler.removeDefinitionObjectFromChildren(def_object);
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