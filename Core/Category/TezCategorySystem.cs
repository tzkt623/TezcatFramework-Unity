using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Core
{
    public static class TezCategorySystem
    {
        #region RootToken
        static List<ITezCategoryRootToken> m_MainTokenList = new List<ITezCategoryRootToken>();
        static Dictionary<string, ITezCategoryRootToken> m_MainTokenDic = new Dictionary<string, ITezCategoryRootToken>();

        public static void registerRootToken(ITezCategoryRootToken token)
        {
            while (token.toID >= m_MainTokenList.Count)
            {
                m_MainTokenList.Add(null);
            }

            m_MainTokenList[token.toID] = token;
            m_MainTokenDic.Add(token.toName, token);
        }

        public static ITezCategoryRootToken getRootToken(string name)
        {
            return m_MainTokenDic[name];
        }

        public static ITezCategoryRootToken getRootToken(int index)
        {
            return m_MainTokenList[index];
        }
        #endregion

        #region Category
        class Slot
        {
            List<TezCategory> m_List = new List<TezCategory>();
            Dictionary<string, TezCategory> m_Dic = new Dictionary<string, TezCategory>();
            public void generate(ITezCategoryToken finalToken, out TezCategory category, TezEventExtension.Function<TezCategory> onGenerate)
            {
                if (!m_Dic.TryGetValue(finalToken.toName, out category))
                {
                    category = onGenerate();
                    m_Dic.Add(finalToken.toName, category);
                    while (finalToken.toID >= m_List.Count)
                    {
                        m_List.Add(null);
                    }
                    m_List[finalToken.toID] = category;
                }
            }

            public TezCategory getCategory(string finalName)
            {
                return m_Dic[finalName];
            }

            public TezCategory getCategory(int finalIndex)
            {
                return m_List[finalIndex];
            }
        }

        static List<Slot> m_SlotList = new List<Slot>();
        static Dictionary<string, Slot> m_SlotDic = new Dictionary<string, Slot>();

        public static TezCategory getCategory(string rootName, string finalName)
        {
            if (m_SlotDic.TryGetValue(rootName, out Slot slot))
            {
                return slot.getCategory(finalName);
            }

            throw new System.Exception();
        }

        public static TezCategory getCategory(int rootID, int finalID)
        {
            return m_SlotList[rootID].getCategory(finalID);                                      
        }

        public static void generate(ITezCategoryRootToken rootToken, ITezCategoryToken finalToken, out TezCategory category, TezEventExtension.Function<TezCategory> onGenerate)
        {
            if (!m_SlotDic.TryGetValue(rootToken.toName, out Slot slot))
            {
                slot = new Slot();
                while (rootToken.toID >= m_SlotList.Count)
                {
                    m_SlotList.Add(null);
                }
                m_SlotList[rootToken.toID] = slot;
                m_SlotDic.Add(rootToken.toName, slot);
            }

            slot.generate(finalToken, out category, onGenerate);
        }
        #endregion
    }
}
