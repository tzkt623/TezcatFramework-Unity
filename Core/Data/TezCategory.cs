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
        ITezCategoryToken[] m_Tokens = null;

        /// <summary>
        /// 主Token
        /// </summary>
        public ITezCategoryMainToken mainToken { get; private set; } = null;

        /// <summary>
        /// 最终Token
        /// 用于比较
        /// </summary>
        public ITezCategoryFinalToken finalToken { get; private set; } = null;

        /// <summary>
        /// 总分类等级
        /// </summary>
        public int count
        {
            get { return m_Tokens.Length; }
        }

        public void setToken(List<ITezCategoryToken> list)
        {
            m_Tokens = list.ToArray();
            this.setToken();
        }

        public void setToken(params ITezCategoryToken[] tokens)
        {
            m_Tokens = tokens;
            this.setToken();
        }

        private void setToken()
        {
            this.mainToken = (ITezCategoryMainToken)m_Tokens[0];
            this.finalToken = (ITezCategoryFinalToken)m_Tokens[m_Tokens.Length - 1];
        }

        public int get(int index)
        {
            return m_Tokens[index].toID;
        }

        public void close(bool self_close = true)
        {
            m_Tokens = null;
            this.finalToken = null;
        }

        /// <summary>
        /// 检测Category是否包含此类型
        /// </summary>
        public bool isInclude(ITezCategoryToken token)
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
