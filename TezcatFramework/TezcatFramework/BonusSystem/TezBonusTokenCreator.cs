using System;
using System.Collections.Generic;

namespace tezcat.Framework.BonusSystem
{
    public class TezBonusToken : ITezBonusToken
    {
        public int tokenID { get; }
        public string tokenName { get; }
        public TezBonusTokenType tokenType { get; }
        public ITezBonusToken parent { get; }

        /// <summary>
        /// 层级
        /// 只在Primary路径上生效
        /// </summary>
        public int layer { get; } = -1;

        public TezBonusToken(int id, string name, TezBonusTokenType type)
        {
            this.tokenID = id;
            this.tokenName = name;
            this.tokenType = type;
            this.parent = null;
        }

        public TezBonusToken(int id, string name, TezBonusTokenType type, ITezBonusToken parent)
        {
            this.tokenID = id;
            this.tokenName = name;
            this.tokenType = type;
            this.parent = parent;
            if (this.parent != null)
            {
                this.layer = parent.layer + 1;
            }
            else
            {
                this.layer = 0;
            }
        }

        public TezBonusPath createPath()
        {
            Stack<ITezBonusToken> stack = new Stack<ITezBonusToken>();
            ITezBonusToken temp = this;

            while (temp != null)
            {
                stack.Push(temp);
                temp = temp.parent;
            }

            return new TezBonusPath()
            {
                path = stack.ToArray()
            };
        }

        public override bool Equals(object obj)
        {
            return this.tokenID == ((TezBonusToken)obj).tokenID;
        }

        public override int GetHashCode()
        {
            return tokenID.GetHashCode();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="BountyMask">用于区别不同的加成系统</typeparam>
    public static class TezBonusTokenCreator<BountyMask>
    {
        public static IReadOnlyList<TezBonusToken> tokenList => m_TokenList;
        public static IReadOnlyDictionary<string, TezBonusToken> tokenDict => m_TokenDict;
        public static IReadOnlyList<TezBonusPath> pathList => m_PathList;


        static List<TezBonusToken> m_TokenList = new List<TezBonusToken>();
        static Dictionary<string, TezBonusToken> m_TokenDict = new Dictionary<string, TezBonusToken>();
        static List<TezBonusPath> m_PathList = new List<TezBonusPath>();


        public static TezBonusToken createToken(string name, TezBonusTokenType type, ITezBonusToken parent)
        {
            if (m_TokenDict.ContainsKey(name))
            {
                throw new Exception(string.Format("TezBonusTokenCreator<{0}> : This name [{1}] is existed", typeof(BountyMask).Name, name));
            }

            var id = m_TokenList.Count;
            var token = new TezBonusToken(id, name, type, parent);
            m_TokenList.Add(token);
            m_TokenDict.Add(name, token);
            m_PathList.Add(token.createPath());

            return token;
        }

        public static TezBonusPath getPath(ITezBonusToken token)
        {
            return m_PathList[token.tokenID];
        }

        public static TezBonusPath getPath(string lastTokenName)
        {
            return m_PathList[m_TokenDict[lastTokenName].tokenID];
        }

        public static TezBonusToken getToken(string name)
        {
            TezBonusToken token = null;
            if (m_TokenDict.TryGetValue(name, out token))
            {
                return token;
            }

            throw new Exception(string.Format("TezBonusTokenCreator<{0}> : This name [{1}] not exist", typeof(BountyMask).Name, name));
        }

        public static TezBonusToken getToken(int index)
        {
            return m_TokenList[index];
        }
    }
}
