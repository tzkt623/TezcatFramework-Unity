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

        public TezDefinitionNode getPrimaryNode(int id)
        {
            return this.system.getPrimaryNode(id);
        }

        public TezDefinitionLeaf getSecondaryNode(int id)
        {
            return this.system.getSecondaryNode(id);
        }

        protected override void onAddCustomData(ITezDefinitionObject def_object)
        {
            for (int i = 0; i < this.childCount; i++)
            {
                var handler = this.getPrimaryNode(i);
                handler.addDefinitionObject(def_object);
            }
        }

        protected override void onRemoveCustomData(ITezDefinitionObject def_object)
        {
            for (int i = 0; i < this.childCount; i++)
            {
                var handler = this.getPrimaryNode(i);
                handler.removeDefinitionObject(def_object);
            }
        }

        public override void close(bool self_close = true)
        {
            base.close(self_close);
            m_Children.close(false);
            m_Children = null;
        }
    }
}