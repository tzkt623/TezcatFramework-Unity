using System;
using System.Collections.Generic;
using tezcat.Framework.Core;

namespace tezcat.Framework.Database
{
    /// <summary>
    /// 分类系统
    /// 
    /// <para>
    /// 此类属于特化型唯一变量
    /// 仅在配置单生成
    /// 请勿在运行时单独生成
    /// 由Item赋予真实生成的Object
    /// 或者使用CategorySystem获得
    /// </para>
    /// 
    /// <para>
    /// 通过一个分类序列将对象进行分类
    /// 序列中最后一个为实际类型
    /// 其他则为分类路径
    /// 
    /// 如果数据库的分类和游戏内分类无法对齐
    /// 那分类就没有意义
    /// 理论上所有在游戏中的物品,都应该有一个原型数据存在
    /// </para>
    /// 
    /// <para>
    /// 例如
    /// 分类信息
    /// 装备->武器->炮台->激光炮台
    /// 
    /// finalToken里有全局唯一ID
    /// 所以database里面可以使用这个唯一ID生成分类存储器
    /// 可以把存储结构扁平化
    /// 就算在两个不同的分类下有同名类型,他们的唯一ID也不一样,只是名称一样
    /// </para>
    /// 
    /// <para>
    /// 数据库的检索
    /// 所有物品都在同总数据库列表中
    /// 子数据库里面只包含对应的物品在总数据里面的索引位置
    /// </para>
    /// 
    /// 
    /// </summary>
    public class TezCategory
        : ITezNonCloseable
        , IEquatable<TezCategory>
    {
        public string name => m_Tokens[m_Last].name;

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


        private int lastTokenUID => m_Tokens[m_Last].intValue;

        /// <summary>
        /// 总分类等级
        /// </summary>
        public int count
        {
            get { return m_Tokens.Length; }
        }

        private int m_Last = -1;
        private ITezCategoryBaseToken[] m_Tokens = null;


        public ITezCategoryBaseToken this[int index]
        {
            get { return m_Tokens[index]; }
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
            return m_Tokens[index].intValue;
        }

        /// <summary>
        /// 检测Category是否包含此类型
        /// </summary>
        public bool contains(ITezCategoryBaseToken token)
        {
            if (token.layer < m_Tokens.Length)
            {
                return m_Tokens[token.layer].UID == token.UID;
            }

            return false;
        }

        /// <summary>
        /// 快速比较,不判断参数是否为空
        /// </summary>
        public bool fastCompare(TezCategory other)
        {
            return lastTokenUID == other.lastTokenUID;
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

            return lastTokenUID == other.lastTokenUID;
        }

        public static bool operator ==(TezCategory a, TezCategory b)
        {
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
            {
                return false;
            }

            return a.lastTokenUID == b.lastTokenUID;
        }

        public static bool operator !=(TezCategory a, TezCategory b)
        {
            return !(a == b);
        }
    }
}