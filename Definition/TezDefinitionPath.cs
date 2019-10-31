using tezcat.Framework.Core;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Definition
{
    /// <summary>
    /// <para>类型分类信息</para>
    /// <para>包括对象 属性 Modifier等</para>
    /// <para>主分类为线性分类,有且只有一条主路径信息</para>
    /// <para>次分类为离散分类(依托在MainToken分类信息下),可以有多条次路径信息</para>
    /// </summary>
    public sealed class TezDefinitionPath : ITezCloseable
    {
        static readonly ITezDefinitionToken[] m_DefaultPrimaryPath = new ITezDefinitionToken[0];
        static readonly ITezDefinitionToken[] m_DefaultSecondaryPath = new ITezDefinitionToken[0];

        /// <summary>
        /// 先判断有没有
        /// 在做处理
        /// </summary>
        public int primaryLength
        {
            get { return m_PrimaryPath.Length; }
        }

        /// <summary>
        /// 先判断有没有
        /// 在做处理
        /// </summary>
        public int secondaryLength
        {
            get { return m_SecondaryPath.Length; }
        }

        ITezDefinitionToken m_MainToken = null;
        ITezDefinitionToken[] m_PrimaryPath = null;
        ITezDefinitionToken[] m_SecondaryPath = null;

        public TezDefinitionPath(ITezDefinitionToken main_token, ITezDefinitionToken[] primary_path = null, ITezDefinitionToken[] secondary_path = null)
        {
            m_MainToken = main_token;
            m_PrimaryPath = primary_path == null ? m_DefaultPrimaryPath : primary_path;
            m_SecondaryPath = secondary_path == null ? m_DefaultSecondaryPath : secondary_path;
        }

        public TezDefinitionPath(ITezDefinitionToken[] primary_path = null, ITezDefinitionToken[] secondary_path = null)
        {
            m_MainToken = null;
            m_PrimaryPath = primary_path == null ? m_DefaultPrimaryPath : primary_path;
            m_SecondaryPath = secondary_path == null ? m_DefaultSecondaryPath : secondary_path;
        }

        public TezDefinitionPath clone()
        {
            return new TezDefinitionPath(m_MainToken, m_PrimaryPath, m_SecondaryPath);
        }

        /// <summary>
        /// 单独设置主路径
        /// </summary>
        public void setPrimaryPath(ITezDefinitionToken[] path)
        {
            m_PrimaryPath = path;
        }

        /// <summary>
        /// 单独设置副路径
        /// </summary>
        public void setSecondaryPath(ITezDefinitionToken[] path)
        {
            m_SecondaryPath = path;
        }

        /// <summary>
        /// 替换SecondaryPath中的某个Token
        /// </summary>
        public bool replaceSecondaryPathToken(ITezDefinitionToken new_token, TezEventExtension.Function<bool, ITezDefinitionToken> finder)
        {
            for (int i = 0; i < m_SecondaryPath.Length; i++)
            {
                if (finder(m_SecondaryPath[i]))
                {
                    m_SecondaryPath[i] = new_token;
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// MainToken
        /// 可以用于多种分类
        /// </summary>
        /// <returns></returns>
        public ITezDefinitionToken getMainToken()
        {
            return m_MainToken;
        }

        /// <summary>
        /// 主分类路径信息
        /// </summary>
        public ITezDefinitionToken getPrimaryPathToken(int index)
        {
            return m_PrimaryPath[index];
        }

        /// <summary>
        /// 次分类的所有信息应该都不是主分类路径上的信息
        /// </summary>
        public ITezDefinitionToken getSecondaryPathToken(int index)
        {
            return m_SecondaryPath[index];
        }

        public override string ToString()
        {
            string primary = "Null", secondary = "Null";
            if (this.primaryLength > 0)
            {
                primary = null;
                for (int i = 0; i < m_PrimaryPath.Length; i++)
                {
                    primary += m_PrimaryPath[i].tokenName;
                    if (i != m_PrimaryPath.Length - 1)
                    {
                        primary += "-";
                    }
                }
            }

            if (this.secondaryLength > 0)
            {
                secondary = null;
                for (int i = 0; i < m_SecondaryPath.Length; i++)
                {
                    secondary += m_PrimaryPath[i].tokenName;
                    if (i != m_PrimaryPath.Length - 1)
                    {
                        secondary += "-";
                    }
                }
            }

            return string.Format("Main:{0}\nPrimary:{1}\nSecondary:{2}"
                , m_MainToken.tokenName
                , primary
                , secondary);
        }

        public void close()
        {
            m_MainToken = null;
            m_PrimaryPath = null;
            m_SecondaryPath = null;
        }
    }
}