using System;
using System.Collections.Generic;

namespace tezcat.Framework.Definition
{
    public abstract class TezDefinitionRootNode : TezDefinitionChildrenNode
    {
        public sealed override TezDefinitionNodeType nodeType => TezDefinitionNodeType.Root;

        List<TezDefinitionLeafNode> m_SecondaryChildren = new List<TezDefinitionLeafNode>();

        /// <summary>
        /// 查找路径终点上的节点
        /// </summary>
        protected ITezDefinitionNode findPrimaryNode(TezDefinitionPath path)
        {
            ITezDefinitionNode node = this;

            var primary_length = path.primaryLength;
            for (int i = 0; i < primary_length; i++)
            {
                switch (node.nodeType)
                {
                    case TezDefinitionNodeType.Root:
                        var root_node = (TezDefinitionRootNode)node;
                        node = root_node.getPrimaryChild(path.getPrimaryPathToken(i));
                        break;
                    case TezDefinitionNodeType.Path:
                        var path_node = (TezDefinitionPathNode)node;
                        node = path_node.getPrimaryChild(path.getPrimaryPathToken(i));
                        break;
                    default:
                        throw new Exception(string.Format("{0} : [{1}] can not be Found in search!!", this.GetType().Name, node.nodeType));
                }
            }

            return node;
        }

        public void registerObject(ITezDefinitionPathObject path_object)
        {
            this.onRegisterObject(path_object);

            ///在主路径上注册Object
            var path = path_object.definitionPath;
            var primary_length = path.primaryLength;
            if (primary_length > 0)
            {
                ITezDefinitionNode node = this;
                for (int i = 0; i < primary_length; i++)
                {
                    switch (node.nodeType)
                    {
                        case TezDefinitionNodeType.Root:
                            var root_node = (TezDefinitionRootNode)node;
                            node = root_node.getPrimaryChild(path.getPrimaryPathToken(i));
                            break;
                        case TezDefinitionNodeType.Path:
                            var path_node = (TezDefinitionPathNode)node;
                            path_node.onRegisterObject(path_object);
                            node = path_node.getPrimaryChild(path.getPrimaryPathToken(i));
                            break;
                        default:
                            throw new Exception(string.Format("{0} : [{1}] can not be Found in search!!", this.GetType().Name, node.nodeType));
                    }
                }
                ///最后的Node必定是Leaf
                ((TezDefinitionLeafNode)node).onRegisterObject(path_object);
            }

            ///在次路径上注册Object
            var secondary_length = path.secondaryLength;
            if (secondary_length > 0)
            {
                for (int i = 0; i < secondary_length; i++)
                {
                    this.getSecondaryChild(path.getSecondaryPathToken(i)).onRegisterObject(path_object);
                }
            }
        }

        public void unregisterObject(ITezDefinitionPathObject path_object)
        {
            this.onUnregisterObject(path_object);

            var path = path_object.definitionPath;

            var primary_length = path.primaryLength;
            if (primary_length > 0)
            {
                ITezDefinitionNode node = this;
                for (int i = 0; i < primary_length; i++)
                {
                    switch (node.nodeType)
                    {
                        case TezDefinitionNodeType.Root:
                            var root_node = (TezDefinitionRootNode)node;
                            node = root_node.getPrimaryChild(path.getPrimaryPathToken(i));
                            break;
                        case TezDefinitionNodeType.Path:
                            var path_node = (TezDefinitionPathNode)node;
                            path_node.onUnregisterObject(path_object);
                            node = path_node.getPrimaryChild(path.getPrimaryPathToken(i));
                            break;
                        default:
                            throw new Exception(string.Format("{0} : [{1}] can not exist in foreach", this.GetType().Name, node.nodeType));
                    }
                }
                ((TezDefinitionLeafNode)node).onUnregisterObject(path_object);
            }

            var secondary_length = path.secondaryLength;
            if (secondary_length > 0)
            {
                for (int i = 0; i < secondary_length; i++)
                {
                    this.getSecondaryChild(path.getSecondaryPathToken(i)).onUnregisterObject(path_object);
                }
            }
        }

        protected TezDefinitionLeafNode getSecondaryChild(ITezDefinitionToken token)
        {
            var id = token.tokenID;
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