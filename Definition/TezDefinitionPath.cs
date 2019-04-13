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
        public int primaryLength
        {
            get { return m_PrimaryPathes.Length; }
        }

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

        public void close()
        {
            m_MainToken = null;
            m_PrimaryPathes = null;
            m_SecondaryPathes = null;
        }
    }
}