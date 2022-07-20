using tezcat.Framework.Core;

namespace tezcat.Framework.BonusSystem
{
    /// <summary>
    /// 加成路径
    /// 所有的加成路径在配置文件中就会生成完毕
    /// 只需要按需取用就行了
    /// 不用再重新生成
    /// 
    /// agent使用此Path对自身进行定义
    /// 各种modifier可以使用这个定义寻找路径进行加成计算
    /// </summary>
    public class TezBonusPath : ITezNonCloseable
    {
        static ITezBonusToken[] DefaultPrimaryPath = new ITezBonusToken[0];

        public int length
        {
            get { return m_Path.Length; }
        }

        public ITezBonusToken[] path
        {
            set
            {
                m_Path = value;
                m_LastIndex = m_Path.Length - 1;
            }
        }

        public ITezBonusToken lastToken => m_Path[m_LastIndex];

        ITezBonusToken[] m_Path = DefaultPrimaryPath;
        int m_LastIndex = -1;

        public TezBonusPath() { }

        public TezBonusPath(ITezBonusToken[] path)
        {
            m_Path = path;
            m_LastIndex = m_Path.Length - 1;
        }

        /// <summary>
        /// 主分类路径信息
        /// </summary>
        public ITezBonusToken getToken(int index)
        {
            return m_Path[index];
        }

        /// <summary>
        /// 检验主路径包含
        /// </summary>
        public bool contains(ITezBonusToken token)
        {
            if (token.layer < m_Path.Length)
            {
                return m_Path[token.layer].tokenID == token.tokenID;
            }

            return false;
        }
    }
}