using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tezcat.Framework.Definition
{
    public static class TezDefinitionElement
    {
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
                if (parent != null)
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

            public void close()
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
    }
}
