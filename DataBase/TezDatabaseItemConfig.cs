using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    public static class TezDatabaseItemConfig
    {
        public class Config
        {
            public string name;
            public int stackCount = 0;
        }

        static Config s_Default = new Config()
        {
            name = "#DefaultItemConfig",
            stackCount = 0
        };

        static List<List<Config>> m_ConfigList = new List<List<Config>>();

        public static Config createConfig(TezCategory category)
        {
            return createConfig(category.rootToken, category.finalToken);
        }

        public static Config createConfig(ITezCategoryRootToken rootToken, ITezCategoryToken finalToken)
        {
            var rid = rootToken.toID;
            var findex = finalToken.finalIndexInRootToken;

            while (m_ConfigList.Count <= rid)
            {
                m_ConfigList.Add(new List<Config>());
            }

            var configs = m_ConfigList[rid];
            while (configs.Count <= findex)
            {
                configs.Add(null);
            }

            var config = new Config();
            configs[findex] = config;
            return config;
        }

        public static Config getConfig(TezCategory category)
        {
            return getConfig(category.rootToken, category.finalToken);
        }

        public static Config getConfig(ITezCategoryRootToken rootToken, ITezCategoryToken finalToken)
        {
            var rid = rootToken.toID;
            var findex = finalToken.finalIndexInRootToken;
            if (rid < m_ConfigList.Count)
            {
                var list = m_ConfigList[rid];
                if (findex < list.Count)
                {
                    return list[findex];
                }
            }

            return s_Default;
        }
    }
}