using System;
using System.Collections.Generic;

namespace tezcat.Framework.Core
{
    /// <summary>
    /// 分类系统
    /// <para>通过一个分类序列将对象进行分类</para>
    /// <para>序列中最后一个为实际类型</para>
    /// <para>其他则为分类路径</para>
    /// 
    /// <para>此类属于特化型唯一变量</para>
    /// <para>仅在配置单生成</para>
    /// 
    /// <para>他由Item赋予真实生成的Object</para>
    /// <para>或者使用CategorySystem获得</para>
    /// 
    /// <para>请勿在运行时单独生成</para>
    /// </summary>
    public class TezCategory
        : ITezCloseable
        , IEquatable<TezCategory>
    {
        /// <summary>
        /// 主Token
        /// 此类型的最上级分类
        /// </summary>
        public ITezCategoryRootToken rootToken => (ITezCategoryRootToken)m_Tokens[0];

        /// <summary>
        /// 最终Token
        /// 此类型的最下级分类
        /// 用于比较
        /// </summary>
        public ITezCategoryFinalToken finalToken => (ITezCategoryFinalToken)m_Tokens[m_Last];

        /// <summary>
        /// 总分类等级
        /// </summary>
        public int count
        {
            get { return m_Tokens.Length; }
        }

        int m_Last = -1;
        ITezCategoryBaseToken[] m_Tokens = null;

        public ITezCategoryBaseToken this[int index]
        {
            get { return m_Tokens[index];}
        }

        public void setToken(List<ITezCategoryBaseToken> list)
        {
            m_Tokens = list.ToArray();
            m_Last = m_Tokens.Length - 1;
        }

        public void setToken(params ITezCategoryBaseToken[] tokens)
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
        public bool contains(ITezCategoryBaseToken token)
        {
            if (token.layer < m_Tokens.Length)
            {
                return m_Tokens[token.layer].Equals(token);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return this.Equals((TezCategory)other);
        }

        public bool Equals(TezCategory other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return this.rootToken.toID == other.rootToken.toID && this.finalToken.toID == other.finalToken.toID;
        }

        public static bool operator ==(TezCategory a, TezCategory b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        public static bool operator !=(TezCategory a, TezCategory b)
        {
            return !(a == b);
        }
    }
}
