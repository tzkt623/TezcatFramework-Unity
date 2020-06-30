using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Definition
{
    public abstract class TezDefinitionSystem
        : ITezCloseable
        , ITezDefinitionHandler
    {
        #region Manager
        public class Element : ITezDefinitionToken
        {
            public int tokenID { get; }
            public string tokenName { get; }
            public TezDefinitionTokenType tokenType { get; }

            /// <summary>
            /// 层级
            /// 只在Primary路径上生效
            /// </summary>
            public int layer { get; } = -1;

            public Element(int id, string name, TezDefinitionTokenType type)
            {
                this.tokenID = id;
                this.tokenName = name;
                this.tokenType = type;
            }

            public Element(int id, string name, TezDefinitionTokenType type, ITezDefinitionToken parent)
            {
                this.tokenID = id;
                this.tokenName = name;
                this.tokenType = type;
                if(parent != null)
                {
                    this.layer = parent.layer + 1;
                }
                else
                {
                    this.layer = 0;
                }
            }

            public override bool Equals(object obj)
            {
                return this.tokenID == ((Element)obj).tokenID;
            }

            public override int GetHashCode()
            {
                return tokenID.GetHashCode();
            }

            public void close(bool self_close = true)
            {

            }
        }

        static List<Element> m_PrimaryElements = new List<Element>();
        static Dictionary<string, Element> m_PrimaryElementsWithName = new Dictionary<string, Element>();
        public static Element createPrimaryElement(string name, TezDefinitionTokenType type, ITezDefinitionToken parent)
        {
            if (m_PrimaryElementsWithName.ContainsKey(name))
            {
                throw new Exception(string.Format("TezDefinitionSet : this Primary name [{0}] is existed", name));
            }

            var id = m_PrimaryElements.Count;
            var element = new Element(id, name, type, parent);
            m_PrimaryElements.Add(element);
            m_PrimaryElementsWithName.Add(name, element);
            return element;
        }

        public static Element getPrimary(string name)
        {
            Element element = null;
            if (m_PrimaryElementsWithName.TryGetValue(name, out element))
            {
                return element;
            }

            throw new Exception(string.Format("TezDefinitionSet : Primary name [{0}] not exist", name));
        }

        public static Element getPrimary(int index)
        {
            return m_PrimaryElements[index];
        }

        static List<Element> m_SecondaryElements = new List<Element>();
        static Dictionary<string, Element> m_SecondaryElementsWithName = new Dictionary<string, Element>();
        public static Element createSecondaryElement(string name)
        {
            if (m_SecondaryElementsWithName.ContainsKey(name))
            {
                throw new Exception(string.Format("TezDefinitionSet : this Secondary name [{0}] is existed", name));
            }

            var id = m_SecondaryElements.Count;
            var element = new Element(id, name, TezDefinitionTokenType.Leaf);
            m_SecondaryElements.Add(element);
            m_SecondaryElementsWithName.Add(name, element);
            return element;
        }

        public static Element getSecondary(string name)
        {
            Element element = null;
            if (m_SecondaryElementsWithName.TryGetValue(name, out element))
            {
                return element;
            }

            throw new Exception(string.Format("TezDefinitionSet : Secondary name [{0}] not exist", name));
        }

        public static Element getSecondary(int index)
        {
            return m_SecondaryElements[index];
        }
        #endregion

        //         List<TezDefinitionNode> m_PrimaryNodes = new List<TezDefinitionNode>();
        //         List<TezDefinitionLeaf> m_SecondaryNodes = new List<TezDefinitionLeaf>();

        Dictionary<int, TezDefinitionNode> m_PrimaryNodes = new Dictionary<int, TezDefinitionNode>();
        Dictionary<int, TezDefinitionLeaf> m_SecondaryNodes = new Dictionary<int, TezDefinitionLeaf>();

        protected abstract TezDefinitionNode onCreatePrimaryChild(ITezDefinitionToken token);
        protected abstract TezDefinitionLeaf onCreateSecondaryChild(ITezDefinitionToken token);

        protected TezDefinitionNode getOrCreatePrimaryNode(ITezDefinitionToken token, ref TezDefinitionPath pre_path_node)
        {
            var id = token.tokenID;

            TezDefinitionNode node = null;
            if (!m_PrimaryNodes.TryGetValue(id, out node))
            {
                node = this.onCreatePrimaryChild(token);
                m_PrimaryNodes.Add(id, node);
                pre_path_node?.addChild(id);
            }

            if (node.nodeType == TezDefinitionNodeType.Path)
            {
                pre_path_node = (TezDefinitionPath)node;
            }

            return node;
        }

        protected TezDefinitionNode getOrCreatePrimaryNode(ITezDefinitionToken token)
        {
            var id = token.tokenID;

            TezDefinitionNode node = null;
            if (!m_PrimaryNodes.TryGetValue(id, out node))
            {
                node = this.onCreatePrimaryChild(token);
                m_PrimaryNodes.Add(id, node);
            }

            return node;
        }

        public TezDefinitionNode getPrimaryNode(ITezDefinitionToken token)
        {
            return this.getPrimaryNode(token.tokenID);
        }

        public TezDefinitionNode getPrimaryNode(int id)
        {
            TezDefinitionNode node = null;
            m_PrimaryNodes.TryGetValue(id, out node);
            return node;
        }

        protected TezDefinitionLeaf getOrCreateSecondaryNode(ITezDefinitionToken token)
        {
            var id = token.tokenID;

            TezDefinitionLeaf node = null;
            if (!m_SecondaryNodes.TryGetValue(id, out node))
            {
                node = this.onCreateSecondaryChild(token);
                m_SecondaryNodes.Add(id, node);
            }

            return node;
        }

        public TezDefinitionLeaf getSecondaryNode(ITezDefinitionToken token)
        {
            return this.getSecondaryNode(token.tokenID);
        }

        public TezDefinitionLeaf getSecondaryNode(int id)
        {
            TezDefinitionLeaf node = null;
            m_SecondaryNodes.TryGetValue(id, out node);
            return node;
        }

        /// <summary>
        /// 注册Object
        /// </summary>
        public void registerObject(ITezDefinitionObjectAndHandler handler)
        {
            var definition_path = handler.definition;
            var primary_length = definition_path.primaryLength;
            TezDefinitionPath pre_path_node = null;
            for (int i = 1; i < primary_length; i++)
            {
                this.getOrCreatePrimaryNode(definition_path.getPrimaryPathToken(i), ref pre_path_node).onRegisterObject(handler);
            }

            var secondary_length = definition_path.secondaryLength;
            for (int i = 0; i < secondary_length; i++)
            {
                this.getOrCreateSecondaryNode(definition_path.getSecondaryPathToken(i)).onRegisterObject(handler);
            }
        }

        /// <summary>
        /// 单独注册PrimaryPath
        /// </summary>
        public void registerPrimaryPath(ITezDefinitionObjectAndHandler handler)
        {
            var definition_path = handler.definition;
            var primary_length = definition_path.primaryLength;
            TezDefinitionPath pre_path_node = null;
            for (int i = 1; i < primary_length; i++)
            {
                this.getOrCreatePrimaryNode(definition_path.getPrimaryPathToken(i), ref pre_path_node).onRegisterObject(handler);
            }
        }

        /// <summary>
        /// 单独注册SecondaryPath
        /// </summary>
        public void registerSecondaryPath(ITezDefinitionObjectAndHandler handler)
        {
            var definition_path = handler.definition;
            var secondary_length = definition_path.secondaryLength;
            for (int i = 0; i < secondary_length; i++)
            {
                this.getOrCreateSecondaryNode(definition_path.getSecondaryPathToken(i)).onRegisterObject(handler);
            }
        }

        /// <summary>
        /// 单独注册一个SecondaryToken
        /// </summary>
        /// <param name="handler"></param>
        public void registerSecondaryPath(ITezDefinitionObjectAndHandler handler, ITezDefinitionToken token)
        {
            this.getOrCreateSecondaryNode(token).onRegisterObject(handler);
        }

        /// <summary>
        /// 注销Object
        /// </summary>
        public void unregisterObject(ITezDefinitionObjectAndHandler handler)
        {
            var definition_path = handler.definition;
            var primary_length = definition_path.primaryLength;
            for (int i = 1; i < primary_length; i++)
            {
                this.getPrimaryNode(definition_path.getPrimaryPathToken(i)).onUnregisterObject(handler);
            }

            var secondary_length = definition_path.secondaryLength;
            for (int i = 0; i < secondary_length; i++)
            {
                this.getSecondaryNode(definition_path.getSecondaryPathToken(i)).onUnregisterObject(handler);
            }
        }

        /// <summary>
        /// 单独注销PrimaryPath
        /// </summary>
        public void unregisterPrimaryPath(ITezDefinitionObjectAndHandler handler)
        {
            var definition_path = handler.definition;
            var primary_length = definition_path.primaryLength;
            for (int i = 1; i < primary_length; i++)
            {
                this.getPrimaryNode(definition_path.getPrimaryPathToken(i)).onUnregisterObject(handler);
            }
        }

        /// <summary>
        /// 单独注销SecondaryPath
        /// </summary>
        public void unregisterSecondaryPath(ITezDefinitionObjectAndHandler handler)
        {
            var definition_path = handler.definition;
            var secondary_length = definition_path.secondaryLength;
            for (int i = 0; i < secondary_length; i++)
            {
                this.getSecondaryNode(definition_path.getSecondaryPathToken(i)).onUnregisterObject(handler);
            }
        }

        /// <summary>
        /// 单独注销一个SecondaryToken
        /// </summary>
        /// <param name="handler"></param>
        public void unregisterSecondaryPath(ITezDefinitionObjectAndHandler handler, ITezDefinitionToken token)
        {
            this.getSecondaryNode(token).onUnregisterObject(handler);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void close(bool self_close = true)
        {
            foreach (var pair in m_PrimaryNodes)
            {
                pair.Value.close(false);
            }

            foreach (var pair in m_SecondaryNodes)
            {
                pair.Value.close(false);
            }

            m_PrimaryNodes.Clear();
            m_SecondaryNodes.Clear();

            m_PrimaryNodes = null;
            m_SecondaryNodes = null;
        }

        /// <summary>
        /// 添加一个带定义的对象
        /// 一般为各种Modifier
        /// 用于属性加成等系统
        /// </summary>
        public void addDefinitionObject(ITezDefinitionObject def_object)
        {
            ///如果没有Object被注册到路径上
            ///那么就算此Modifier加入了也不会造成仍和影响
            ///
            ///如果路径已经建立好了
            ///Modifier加入时只需要直接加入他定义路径的最后的位置上即可
            var definition = def_object.definition;
            int length = definition.primaryLength;
            if (length > 0)
            {
                var node = this.getOrCreatePrimaryNode(definition.getPrimaryPathToken(length - 1));
                node.addDefinitionObject(def_object);
            }
            else
            {
                length = definition.secondaryLength;
                for (int i = 0; i < length; i++)
                {
                    var node = this.getOrCreateSecondaryNode(definition.getSecondaryPathToken(i));
                    node.addDefinitionObject(def_object);
                }
            }
        }

        public void removeDefinitionObject(ITezDefinitionObject def_object)
        {
            var defition = def_object.definition;
            int length = defition.primaryLength;
            if (length > 0)
            {
                var node = this.getPrimaryNode(defition.getPrimaryPathToken(length - 1));
                node.removeDefinitionObject(def_object);
            }
            else
            {
                length = defition.secondaryLength;
                for (int i = 0; i < length; i++)
                {
                    var node = this.getSecondaryNode(defition.getSecondaryPathToken(i));
                    node.removeDefinitionObject(def_object);
                }
            }
        }
    }
}