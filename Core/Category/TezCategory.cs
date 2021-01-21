using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 分类系统
    /// 通过一个分类序列将对象进行分类
    /// 序列中最后一个为实际类型
    /// 其他则为分类路径
    /// </summary>
    public class TezCategory : ITezCloseable
    {
        /// <summary>
        /// 主Token
        /// 此类型的最上级分类
        /// </summary>
        public ITezCategoryToken mainToken => m_Tokens[0];

        /// <summary>
        /// 最终Token
        /// 此类型的最下级分类
        /// 用于比较
        /// </summary>
        public ITezCategoryToken finalToken => m_Tokens[m_Last];

        /// <summary>
        /// 总分类等级
        /// </summary>
        public int count
        {
            get { return m_Tokens.Length; }
        }

        int m_Last = -1;
        ITezCategoryToken[] m_Tokens = null;

        public void setToken(List<ITezCategoryToken> list)
        {
            m_Tokens = list.ToArray();
            m_Last = m_Tokens.Length - 1;
        }

        public void setToken(params ITezCategoryToken[] tokens)
        {
            m_Tokens = tokens;
            m_Last = m_Tokens.Length - 1;
        }

        public int get(int index)
        {
            return m_Tokens[index].toID;
        }

        public void close()
        {
            m_Tokens = null;
        }

        /// <summary>
        /// 检测Category是否包含此类型
        /// </summary>
        public bool contains(ITezCategoryToken token)
        {
            if (token.layer < m_Tokens.Length)
            {
                return m_Tokens[token.layer].sameAs(token);
            }

            return false;
        }

        /// <summary>
        /// 与另一个比较是否相同
        /// </summary>
        public bool sameAs(TezCategory other)
        {
            if (this.count != other.count)
            {
                return false;
            }

            return this.finalToken.sameAs(other.finalToken);
        }
    }
}
