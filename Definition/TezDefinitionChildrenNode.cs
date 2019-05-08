using System.Collections.Generic;

namespace tezcat.Framework.Definition
{
    public abstract class TezDefinitionChildrenNode : ITezDefinitionNode
    {
        public abstract TezDefinitionNodeType nodeType { get; }

//        protected List<ITezDefinitionNode> m_Children = new List<ITezDefinitionNode>();
        Dictionary<int, ITezDefinitionNode> m_ChildrenWithID = new Dictionary<int, ITezDefinitionNode>();
        protected List<ITezDefinitionNode> m_ActivedChildren = new List<ITezDefinitionNode>();

        public ITezDefinitionNode getPrimaryChild(ITezDefinitionToken token)
        {
            var id = token.toID;

            ITezDefinitionNode node = null;
            if(!m_ChildrenWithID.TryGetValue(id, out node))
            {
                node = this.onCreatePrimaryChild(token);
                m_ChildrenWithID.Add(id, node);
                m_ActivedChildren.Add(node);
            }

            return node;
        }

        public T getPrimaryChild<T>(ITezDefinitionToken token) where T : ITezDefinitionNode
        {
            return (T)this.getPrimaryChild(token);
        }

        protected abstract ITezDefinitionNode onCreatePrimaryChild(ITezDefinitionToken token);

        public abstract void onRegisterObject(ITezDefinitionPathObject path_with_object);
        public abstract void onUnregisterObject(ITezDefinitionPathObject path_with_object);

        public virtual void close()
        {
            m_ChildrenWithID.Clear();
            m_ActivedChildren.Clear();

            m_ChildrenWithID = null;
            m_ActivedChildren = null;
        }
    }
}

