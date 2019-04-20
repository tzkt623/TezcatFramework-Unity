using tezcat.Framework.Core;
using tezcat.Framework.DataBase;

namespace tezcat.Framework.Definition
{
    /// <summary>
    /// <para>类型分类信息</para>
    /// <para>包括对象 属性 Modifier等</para>
    /// <para>主分类为线性分类(0号分类信息为MainToken),有且只有一条主路径信息</para>
    /// <para>次分类为离散分类(依托在MainToken分类信息下),可以有多条次路径信息</para>
    /// </summary>
    public class TezDefinitionPath : ITezCloseable
    {
        /// <summary>
        /// 先判断有没有
        /// 在做处理
        /// </summary>
        public int primaryLength
        {
            get { return m_PrimaryPathes != null ? m_PrimaryPathes.Length : 0; }
        }

        /// <summary>
        /// 先判断有没有
        /// 在做处理
        /// </summary>
        public int secondaryLength
        {
            get { return m_SecondaryPathes != null ? m_SecondaryPathes.Length : 0; }
        }

        ITezGroup m_MainToken = null;
        ITezDefinitionToken[] m_PrimaryPathes = null;
        ITezDefinitionToken[] m_SecondaryPathes = null;

        public TezDefinitionPath(ITezGroup main_token, ITezDefinitionToken[] primary_path = null, ITezDefinitionToken[] secondary_path = null)
        {
            m_MainToken = main_token;
            m_PrimaryPathes = primary_path;
            m_SecondaryPathes = secondary_path;
        }

        public TezDefinitionPath clone()
        {
            return new TezDefinitionPath(m_MainToken, m_PrimaryPathes, m_SecondaryPathes);
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
            return m_PrimaryPathes[index];
        }

        /// <summary>
        /// 次分类的所有信息应该都不是主分类路径上的信息
        /// </summary>
        public ITezDefinitionToken getSecondaryPathToken(int index)
        {
            return m_SecondaryPathes[index];
        }

        public override string ToString()
        {
            string primary = "Null", secondary = "Null";
            if(this.primaryLength > 0)
            {
                primary = null;
                for (int i = 0; i < m_PrimaryPathes.Length; i++)
                {
                    primary += m_PrimaryPathes[i].toName;
                    if (i != m_PrimaryPathes.Length - 1)
                    {
                        primary += "-";
                    }
                }
            }

            if(this.secondaryLength > 0)
            {
                secondary = null;
                for (int i = 0; i < m_SecondaryPathes.Length; i++)
                {
                    secondary += m_PrimaryPathes[i].toName;
                    if (i != m_PrimaryPathes.Length - 1)
                    {
                        secondary += "-";
                    }
                }
            }

            return string.Format("Main:{0}\nPrimary:{1}\nSecondary:{2}"
                , m_MainToken.toName
                , primary
                , secondary);
        }

        public void close()
        {
            m_MainToken = null;
            m_PrimaryPathes = null;
            m_SecondaryPathes = null;
        }
    }
}