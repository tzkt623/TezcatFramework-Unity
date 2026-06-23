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
    /// <typeparam name="BonusMask">用于区别不同的加成系统</typeparam>
    public static class TezBonusTokenCreator<BonusMask>
    {
        public static IReadOnlyList<TezBonusToken> tokenList => sTokenList;
        public static IReadOnlyDictionary<string, TezBonusToken> tokenDict => sTokenDict;
        public static IReadOnlyList<TezBonusPath> pathList => sPathList;


        static List<TezBonusToken> sTokenList = new List<TezBonusToken>();
        static Dictionary<string, TezBonusToken> sTokenDict = new Dictionary<string, TezBonusToken>();
        static List<TezBonusPath> sPathList = new List<TezBonusPath>();


        public static TezBonusToken createToken(string name, TezBonusTokenType type, ITezBonusToken parent)
        {
            if (sTokenDict.ContainsKey(name))
            {
                throw new Exception($"TezBonusTokenCreator<{typeof(BonusMask).Name}> : This name [{name}] is existed");
            }

            var id = sTokenList.Count;
            var token = new TezBonusToken(id, name, type, parent);
            sTokenList.Add(token);
            sTokenDict.Add(name, token);
            sPathList.Add(token.createPath());

            return token;
        }

        public static TezBonusPath getPath(ITezBonusToken token)
        {
            return sPathList[token.tokenID];
        }

        public static TezBonusPath getPath(string lastTokenName)
        {
            return sPathList[sTokenDict[lastTokenName].tokenID];
        }

        public static TezBonusToken getToken(string name)
        {
            TezBonusToken token = null;
            if (sTokenDict.TryGetValue(name, out token))
            {
                return token;
            }

            throw new Exception($"TezBonusTokenCreator<{typeof(BonusMask).Name}> : This name [{name}] not exist");
        }

        public static TezBonusToken getToken(int index)
        {
            return sTokenList[index];
        }
    }
}
