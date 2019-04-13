using System.Collections.Generic;

namespace tezcat.Framework.Definition
{
    public abstract class TezDefinitionRootNode : TezDefinitionChildrenNode
    {
        public override TezDefinitionNodeType nodeType => TezDefinitionNodeType.Root;

        List<TezDefinitionLeafNode> m_SecondaryChildren = new List<TezDefinitionLeafNode>();

        public void registerObject(ITezDefinitionPathWithObject path_with_object)
        {
            this.onRegisterObject(path_with_object);

            var path = path_with_object.definitionPath;

            ITezDefinitionNode node = null;
            for (int i = 0; i < path.primaryLength; i++)
            {
                node = this.getPrimaryChild(path.getPrimaryPathToken(i));
                switch (node.nodeType)
                {
                    case TezDefinitionNodeType.Path:
                        ((TezDefinitionChildrenNode)node).onRegisterObject(path_with_object);
                        break;
                    case TezDefinitionNodeType.Leaf:
                        ((TezDefinitionLeafNode)node).onRegisterObject(path_with_object);
                        break;
                }
            }

            var length = path.secondaryLength;
            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    this.getSecondaryChild(path.getSecondaryPathToken(i)).onRegisterObject(path_with_object);
                }
            }
        }

        public void unregisterObject(ITezDefinitionPathWithObject path_with_object)
        {
            this.onUnregisterObject(path_with_object);

            var path = path_with_object.definitionPath;

            ITezDefinitionNode node = null;
            for (int i = 0; i < path.primaryLength; i++)
            {
                node = this.getPrimaryChild(path.getPrimaryPathToken(i));
                switch (node.nodeType)
                {
                    case TezDefinitionNodeType.Path:
                        ((TezDefinitionChildrenNode)node).onUnregisterObject(path_with_object);
                        break;
                    case TezDefinitionNodeType.Leaf:
                        ((TezDefinitionLeafNode)node).onUnregisterObject(path_with_object);
                        break;
                }
            }

            var length = path.secondaryLength;
            if (length > 0)
            {
                for (int i = 0; i < length; i++)
                {
                    this.getSecondaryChild(path.getSecondaryPathToken(i)).onUnregisterObject(path_with_object);
                }
            }
        }

        protected TezDefinitionLeafNode getSecondaryChild(ITezDefinitionToken token)
        {
            var id = token.toID;
            ///检测分路径是否存在
            while (m_SecondaryChildren.Count <= id)
            {
                m_SecondaryChildren.Add(null);
            }

            var node = m_SecondaryChildren[id];
            if (node == null)
            {
                node = this.onCreateSecondaryChild();
                m_SecondaryChildren[id] = node;
            }

            return node;
        }

        protected T getSecondaryChild<T>(ITezDefinitionToken token) where T : TezDefinitionLeafNode
        {
            return (T)this.getSecondaryChild(token);
        }

        protected abstract TezDefinitionLeafNode onCreateSecondaryChild();
    }
}