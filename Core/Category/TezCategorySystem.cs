using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public static class TezCategorySystem
    {
        static List<ITezCategoryToken> m_MainTokenList = new List<ITezCategoryToken>();
        static Dictionary<string, ITezCategoryToken> m_MainTokenDic = new Dictionary<string, ITezCategoryToken>();

        public static void registerMainToken(ITezCategoryToken token)
        {
            while (token.toID >= m_MainTokenList.Count)
            {
                m_MainTokenList.Add(null);
            }

            m_MainTokenList[token.toID] = token;
            m_MainTokenDic.Add(token.toName, token);
        }

        public static ITezCategoryToken getMainToken(string name)
        {
            return m_MainTokenDic[name];
        }

        public static ITezCategoryToken getMainToken(int index)
        {
            return m_MainTokenList[index];
        }
    }
}
