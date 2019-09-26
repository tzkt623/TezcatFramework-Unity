using System;
using System.Collections.Generic;
using tezcat.Framework.Core;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Definition
{
    public abstract class TezDefinitionSet : ITezCloseable
    {
        #region Manager
        public class Element : ITezDefinitionToken
        {
            public int tokenID { get; }
            public string tokenName { get; }
            public TezDefinitionTokenType tokenType { get; }

            public Element(int id, string name, TezDefinitionTokenType type)
            {
                this.tokenID = id;
                this.tokenName = name;
                this.tokenType = type;
            }

            public void close()
            {

            }
        }

        static List<Element> m_PrimaryElements = new List<Element>();
        static Dictionary<string, Element> m_PrimaryElementsWithName = new Dictionary<string, Element>();
        public static Element createPrimaryElement(string name, TezDefinitionTokenType type)
        {
            if (m_PrimaryElementsWithName.ContainsKey(name))
            {
                throw new Exception(string.Format("TezDefinitionSet : this Primary name [{0}] is existed", name));
            }

            var id = m_PrimaryElements.Count;
            var element = new Element(id, name, type);
            m_PrimaryElements.Add(element);
            m_PrimaryElementsWithName.Add(name, element);
            return element;
        }

        public static Element getPrimary(string name)
        {
            return m_PrimaryElementsWithName[name];
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
            return m_SecondaryElementsWithName[name];
        }

        public static Element getSecondary(int index)
        {
            return m_SecondaryElements[index];
        }
        #endregion

        List<TezDefinitionSetObject> m_PrimaryNodes = new List<TezDefinitionSetObject>();
        List<TezDefinitionSetLeaf> m_SecondaryNodes = new List<TezDefinitionSetLeaf>();

        protected abstract TezDefinitionSetObject onCreatePrimaryChild(ITezDefinitionToken token);
        protected abstract TezDefinitionSetLeaf onCreateSecondaryChild(ITezDefinitionToken token);

        protected TezDefinitionSetObject getOrCreatePrimaryNode(ITezDefinitionToken token, ref TezDefinitionSetPath pre_path_node)
        {
            var id = token.tokenID;
            while (m_PrimaryNodes.Count <= id)
            {
                m_PrimaryNodes.Add(null);
            }

            var node = m_PrimaryNodes[id];
            if (node == null)
            {
                node = this.onCreatePrimaryChild(token);
                m_PrimaryNodes[id] = node;
                pre_path_node?.addChild(id);
            }

            if (node.nodeType == TezDefinitionNodeType.Path)
            {
                pre_path_node = (TezDefinitionSetPath)node;
            }

            return node;
        }

        public TezDefinitionSetObject getPrimaryNode(ITezDefinitionToken token)
        {
            return this.getPrimaryNode(token.tokenID);
        }

        public TezDefinitionSetObject getPrimaryNode(int id)
        {
            return m_PrimaryNodes[id];
        }

        protected TezDefinitionSetLeaf getOrCreateSecondaryNode(ITezDefinitionToken token)
        {
            var id = token.tokenID;
            while (m_SecondaryNodes.Count < id)
            {
                m_SecondaryNodes.Add(null);
            }

            var node = m_SecondaryNodes[id];
            if (node == null)
            {
                node = this.onCreateSecondaryChild(token);
                m_SecondaryNodes[id] = node;
            }

            return node;
        }

        public TezDefinitionSetLeaf getSecondaryNode(ITezDefinitionToken token)
        {
            return this.getSecondaryNode(token.tokenID);
        }

        public TezDefinitionSetLeaf getSecondaryNode(int id)
        {
            return m_SecondaryNodes[id];
        }

        public void registerObject(ITezDefinitionPathObject path_with_object)
        {
            var definition_path = path_with_object.definitionPath;
            var primary_length = definition_path.primaryLength;
            if (primary_length > 0)
            {
                TezDefinitionSetPath pre_path_node = null;
                for (int i = 0; i < primary_length; i++)
                {
                    this.getOrCreatePrimaryNode(definition_path.getPrimaryPathToken(i), ref pre_path_node)
                        .onRegisterObject(path_with_object);
                }
            }

            var secondary_length = definition_path.secondaryLength;
            if (secondary_length > 0)
            {
                for (int i = 0; i < secondary_length; i++)
                {
                    this.getOrCreateSecondaryNode(definition_path.getSecondaryPathToken(i)).onRegisterObject(path_with_object);
                }
            }
        }

        public void unregisterObject(ITezDefinitionPathObject path_with_object)
        {
            var definition_path = path_with_object.definitionPath;
            var primary_length = definition_path.primaryLength;
            if (primary_length > 0)
            {
                for (int i = 0; i < primary_length; i++)
                {
                    this.getPrimaryNode(definition_path.getPrimaryPathToken(i)).onUnregisterObject(path_with_object);
                }
            }

            var secondary_length = definition_path.secondaryLength;
            if (secondary_length > 0)
            {
                for (int i = 0; i < secondary_length; i++)
                {
                    this.getSecondaryNode(definition_path.getSecondaryPathToken(i)).onUnregisterObject(path_with_object);
                }
            }
        }

        public virtual void close()
        {
            for (int i = 0; i < m_PrimaryNodes.Count; i++)
            {
                m_PrimaryNodes[i]?.close();
            }

            for (int i = 0; i < m_SecondaryNodes.Count; i++)
            {
                m_SecondaryNodes[i]?.close();
            }

            m_PrimaryNodes.Clear();
            m_SecondaryNodes.Clear();

            m_PrimaryNodes = null;
            m_SecondaryNodes = null;
        }
    }

    public abstract class TezDefinitionSetObject : ITezDefinitionNode
    {
        public int ID { get; }
        public TezDefinitionSet definitionSet { get; private set; } = null;
        public abstract TezDefinitionNodeType nodeType { get; }

        protected TezDefinitionSetObject(int id, TezDefinitionSet set)
        {
            this.ID = id;
            this.definitionSet = set;
        }

        public abstract void onRegisterObject(ITezDefinitionPathObject path_with_object);
        public abstract void onUnregisterObject(ITezDefinitionPathObject path_with_object);

        public virtual void close()
        {
            this.definitionSet = null;
        }
    }

    public abstract class TezDefinitionSetPath : TezDefinitionSetObject
    {
        public sealed override TezDefinitionNodeType nodeType { get; } = TezDefinitionNodeType.Path;
        public int childCount
        {
            get { return m_Children.count; }
        }
        TezArray<int> m_Children = new TezArray<int>(1);

        protected TezDefinitionSetPath(int id, TezDefinitionSet set) : base(id, set)
        {
        }

        public void addChild(int id)
        {
            m_Children.add(id);
        }

        public TezDefinitionSetObject getPrimaryNode(int id)
        {
            return this.definitionSet.getPrimaryNode(id);
        }

        public TezDefinitionSetLeaf getSecondaryNode(int id)
        {
            return this.definitionSet.getSecondaryNode(id);
        }

        public override void close()
        {
            base.close();
            m_Children.close();
            m_Children = null;
        }
    }

    public abstract class TezDefinitionSetLeaf : TezDefinitionSetObject
    {
        public sealed override TezDefinitionNodeType nodeType { get; } = TezDefinitionNodeType.Leaf;

        protected TezDefinitionSetLeaf(int id, TezDefinitionSet set) : base(id, set)
        {

        }
    }
}