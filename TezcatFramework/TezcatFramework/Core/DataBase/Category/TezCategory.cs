using System;
using System.Collections.Generic;

namespace tezcat.Framework.Core
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
    /// </summary>
    public class TezCategory
        : ITezNonCloseable
        , IEquatable<TezCategory>
    {
        /// <summary>
        /// 最后一个Token的Name
        /// </summary>
        public string name => mFinalToken.name;

        /// <summary>
        /// 主Token
        /// 此类型的最上级分类
        /// </summary>
        public ITezCategoryRootToken rootToken => mRootToken;

        /// <summary>
        /// 最终Token
        /// 此类型的最下级分类
        /// 用于比较
        /// </summary>
        public ITezCategoryFinalToken finalToken => mFinalToken;

        /// <summary>
        /// 总分类等级
        /// </summary>
        public int count
        {
            get { return mTokens.Length; }
        }

        //        private init mLast = -1;
        private ITezCategoryBaseToken[] mTokens = null;
        private ITezCategoryRootToken mRootToken = null;
        private ITezCategoryFinalToken mFinalToken = null;


        public ITezCategoryBaseToken this[int index]
        {
            get { return mTokens[index]; }
        }

        public void setToken(List<ITezCategoryBaseToken> list)
        {
            mTokens = list.ToArray();
            var last = mTokens.Length - 1;

            mRootToken = (ITezCategoryRootToken)mTokens[0];
            mFinalToken = (ITezCategoryFinalToken)mTokens[last];
        }

        public void setToken(params ITezCategoryBaseToken[] tokens)
        {
            mTokens = tokens;
            var last = mTokens.Length - 1;

            mRootToken = (ITezCategoryRootToken)mTokens[0];
            mFinalToken = (ITezCategoryFinalToken)mTokens[last];
        }

        public int get(int index)
        {
            return mTokens[index].intValue;
        }

        /// <summary>
        /// 检测Category是否包含此类型
        /// </summary>
        public bool contains(ITezCategoryBaseToken token)
        {
            if (token.layerID < mTokens.Length)
            {
                return mTokens[token.layerID].UID == token.UID;
            }

            return false;
        }

        /// <summary>
        /// 快速比较,不判断参数是否为空
        /// </summary>
        public bool sameAs(TezCategory other)
        {
            return mFinalToken.UID == other.mFinalToken.UID;
        }

        public override int GetHashCode()
        {
            return mFinalToken.globalID;
        }

        /// <summary>
        /// 此类不允许有空类存在
        /// 所以只要有其中一个对象是空
        /// 那就一定返回false
        /// 也就是null和null的运算符比较,不能判定为相等,因为他们不是存在的类型
        /// 如果要进行空判断 请使用object.ReferenceEquals
        /// </summary>
        public override bool Equals(object other)
        {
            return this.Equals((TezCategory)other);
        }

        /// <summary>
        /// 此类不允许有空类存在
        /// 所以只要有其中一个对象是空
        /// 那就一定返回false
        /// 也就是null和null的运算符比较,不能判定为相等,因为他们不是存在的类型
        /// 如果要进行空判断 请使用object.ReferenceEquals
        /// </summary>
        public bool Equals(TezCategory other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return this.sameAs(other);
        }

        /// <summary>
        /// 此类不允许有空类存在
        /// 所以只要有其中一个对象是空
        /// 那就一定返回false
        /// 也就是null和null的运算符比较,不能判定为相等,因为他们不是存在的类型
        /// 如果要进行空判断 请使用object.ReferenceEquals
        /// </summary>
        public static bool operator ==(TezCategory a, TezCategory b)
        {
            if (object.ReferenceEquals(a, null) || object.ReferenceEquals(b, null))
            {
                return false;
            }

            return a.sameAs(b);
        }

        /// <summary>
        /// 此类不允许有空类存在
        /// 所以只要有其中一个对象是空
        /// 那就一定返回false
        /// 也就是null和null的运算符比较,不能判定为相等,因为他们不是存在的类型
        /// 如果要进行空判断 请使用object.ReferenceEquals
        /// </summary>
        public static bool operator !=(TezCategory a, TezCategory b)
        {
            return !(a == b);
        }
    }
}