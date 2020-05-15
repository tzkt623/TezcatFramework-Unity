using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    public static class TezCategorySystem
    {
        static List<ITezCategoryMainToken> m_MainTokenList = new List<ITezCategoryMainToken>();
        static Dictionary<string, ITezCategoryMainToken> m_MainTokenDic = new Dictionary<string, ITezCategoryMainToken>();

        public static void registerMainToken(ITezCategoryMainToken token)
        {
            while (token.toID >= m_MainTokenList.Count)
            {
                m_MainTokenList.Add(null);
            }

            m_MainTokenList[token.toID] = token;
            m_MainTokenDic.Add(token.toName, token);
        }

        public static ITezCategoryMainToken getMainToken(string name)
        {
            return m_MainTokenDic[name];
        }

        public static ITezCategoryMainToken getMainToken(int index)
        {
            return m_MainTokenList[index];
        }
    }
}
