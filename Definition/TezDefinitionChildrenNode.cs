using System.Collections.Generic;

namespace tezcat.Framework.Definition
{
    public abstract class TezDefinitionChildrenNode : ITezDefinitionNode
    {
        public abstract TezDefinitionNodeType nodeType { get; }

        protected List<ITezDefinitionNode> m_Children = new List<ITezDefinitionNode>();
        protected List<ITezDefinitionNode> m_ActivedChildren = new List<ITezDefinitionNode>();

        public ITezDefinitionNode getPrimaryChild(ITezDefinitionToken token)
        {
            var id = token.toID;
            ///检测分路径是否存在
            while (m_Children.Count <= id)
            {
                m_Children.Add(null);
            }

            var node = m_Children[id];
            if (node == null)
            {
                node = this.onCreatePrimaryChild(token);
                m_Children[id] = node;
                m_ActivedChildren.Add(node);
            }

            return node;
        }

        public T getPrimaryChild<T>(ITezDefinitionToken token) where T : ITezDefinitionNode
        {
            return (T)this.getPrimaryChild(token);
        }

        protected abstract ITezDefinitionNode onCreatePrimaryChild(ITezDefinitionToken token);

        public abstract void onRegisterObject(ITezDefinitionPathWithObject path_with_object);
        public abstract void onUnregisterObject(ITezDefinitionPathWithObject path_with_object);

        public virtual void close()
        {
            m_Children.Clear();
            m_ActivedChildren.Clear();

            m_Children = null;
            m_ActivedChildren = null;
        }
    }
}

