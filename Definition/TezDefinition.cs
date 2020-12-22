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
    public class TezDefinition : ITezCloseable
    {
        public enum Path
        {
            Primary,
            Secondary
        }

        static ITezDefinitionToken[] DefaultPrimaryPath = new ITezDefinitionToken[0];
        static ITezDefinitionToken[] DefaultSecondaryPath = new ITezDefinitionToken[0];

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

        public ITezDefinitionToken[] primaryPath
        {
            set
            {
                m_PrimaryPath = value ?? DefaultPrimaryPath;
            }
        }

        public ITezDefinitionToken[] secondaryPath
        {
            set
            {
                m_SecondaryPath = value ?? DefaultSecondaryPath;
            }
        }

        ITezDefinitionToken[] m_PrimaryPath = DefaultPrimaryPath;
        ITezDefinitionToken[] m_SecondaryPath = DefaultSecondaryPath;

        public TezDefinition() { }

        /// <summary>
        /// 建立一个Path路径
        /// </summary>
        /// <param name="mainToken">主分类</param>
        /// <param name="path">路径类型</param>
        /// <param name="otherPath">路径</param>
        public TezDefinition(Path path, ITezDefinitionToken[] otherPath = null)
        {
            switch (path)
            {
                case Path.Primary:
                    m_PrimaryPath = otherPath;
                    m_SecondaryPath = DefaultSecondaryPath;
                    break;
                case Path.Secondary:
                    m_PrimaryPath = DefaultPrimaryPath;
                    m_SecondaryPath = otherPath;
                    break;
                default:
                    break;
            }
        }

        public TezDefinition clone()
        {
            return new TezDefinition()
            {
                primaryPath = m_PrimaryPath,
                secondaryPath = m_SecondaryPath
            };
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
        /// 获得主分类Token
        /// 为PrimaryToken的第一个
        /// </summary>
        /// <returns></returns>
        public ITezDefinitionToken getMainToken()
        {
            return m_PrimaryPath[0];
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

        /// <summary>
        /// 检验主路径包含
        /// </summary>
        public bool containsPrimary(ITezDefinitionToken token)
        {
            if (token.layer < m_PrimaryPath.Length)
            {
                return m_PrimaryPath[token.layer].tokenID == token.tokenID;
            }

            return false;
        }

        /// <summary>
        /// 检验副路径包含
        /// </summary>
        public bool containsSecondary(ITezDefinitionToken token)
        {
            if (m_SecondaryPath.Length > 0)
            {
                foreach (var item in m_SecondaryPath)
                {
                    if (item.tokenID == token.tokenID)
                    {
                        return true;
                    }
                }
            }

            return false;
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

            return string.Format("Primary:{0}\nSecondary:{1}", primary, secondary);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void close()
        {
            m_PrimaryPath = null;
            m_SecondaryPath = null;
        }
    }
}