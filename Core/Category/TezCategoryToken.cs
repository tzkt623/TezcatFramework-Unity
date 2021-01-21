using System;
using System.Collections.Generic;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.Core
{
    public interface ITezCategoryToken : ITezEnumeration
    {
        /// <summary>
        /// Token存在的层
        /// 即在Category系统里TokenArray中的Index
        /// </summary>
        int layer { get; }

        /// <summary>
        /// FinalToken在Root中的位置
        /// 只有final才会有
        /// 其他为-1
        /// </summary>
        int finalIndexInRootToken { get; }

        /// <summary>
        /// 是否为FinalToken
        /// </summary>
        bool isFinalToken { get; }

        /// <summary>
        /// 通过名称获得Token
        /// </summary>
        ITezCategoryToken get(string name);

        /// <summary>
        /// 通过索引获得Token
        /// </summary>
        ITezCategoryToken get(int index);

        /// <summary>
        /// 分配FinalToken的ID
        /// 请勿手动调用
        /// </summary>
        /// <returns></returns>
        int generateID();

        /// <summary>
        /// 注册FinalToken
        /// 请勿手动调用
        /// </summary>
        void registerFinalToken(ITezCategoryToken token);
    }

    /// <summary>
    /// 主分类节点
    /// 用于对object进行分类
    /// </summary>
    public abstract class TezCategoryToken<TEnum, TValue>
        : TezEnumeration<TEnum, TValue>
        , ITezCategoryToken
        where TEnum : TezCategoryToken<TEnum, TValue>
        where TValue : struct, IComparable
    {
        List<ITezCategoryToken> m_FinalTokenList = new List<ITezCategoryToken>();
        Dictionary<string, ITezCategoryToken> m_FinalTokenDic = new Dictionary<string, ITezCategoryToken>();

        /// <summary>
        /// Path中的层级
        /// </summary>
        public int layer { get; }

        /// <summary>
        /// FinalToken在Root中的位置
        /// 只有final才会有
        /// 其他为-1
        /// </summary>
        public int finalIndexInRootToken { get; } = -1;

        /// <summary>
        /// 是否为FinalToken
        /// </summary>
        public bool isFinalToken { get; } = false;

        private TezCategoryToken(TValue value, int layer) : base(value)
        {
            this.layer = layer;
        }

        /// <summary>
        /// 用于创建RootToken
        /// </summary>
        protected TezCategoryToken(TValue value) : this(value, 0)
        {
            TezCategorySystem.registerMainToken(this);
        }

        /// <summary>
        /// 用于创建PathToken
        /// </summary>
        /// <param name="parentToken">Path中的上一级</param>
        protected TezCategoryToken(TValue value, ITezCategoryToken parentToken) : this(value, parentToken.layer + 1)
        {

        }

        /// <summary>
        /// 用于创建FinalToken
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parentToken">Path中的上一级</param>
        /// <param name="rootToken">Root级Token</param>
        protected TezCategoryToken(TValue value, ITezCategoryToken parentToken, ITezCategoryToken rootToken) : this(value, parentToken.layer + 1)
        {
            this.isFinalToken = true;
            this.finalIndexInRootToken = rootToken.generateID();
            rootToken.registerFinalToken(this);
        }

        int ITezCategoryToken.generateID()
        {
            return m_FinalTokenList.Count;
        }

        void ITezCategoryToken.registerFinalToken(ITezCategoryToken token)
        {
            m_FinalTokenList.Add(token);
            m_FinalTokenDic.Add(token.toName, token);
        }

        /// <summary>
        /// 通过Name获得FinalToken
        /// </summary>
        public ITezCategoryToken get(string name)
        {
            ITezCategoryToken token = null;
            if (m_FinalTokenDic.TryGetValue(name, out token))
            {
                return token;
            }

            throw new Exception(string.Format("{0} : no finaltoken named {1}", this.toName, name));
        }

        /// <summary>
        /// 通过Index获得FinalToken
        /// </summary>
        public ITezCategoryToken get(int index)
        {
            return m_FinalTokenList[index];
        }
    }

    /// <summary>
    /// 路径分类节点
    /// 通过传入父路径节点来建立联系
    /// </summary>
    //     public abstract class TezCategoryPathToken<TEnum, TValue>
    //         : TezCategoryBaseToken<TEnum, TValue>
    //         where TEnum : TezCategoryBaseToken<TEnum, TValue>
    //         where TValue : struct, IComparable
    //     {
    //         /// <summary>
    //         /// 传入类型路径中的父级来构建路径
    //         /// </summary>
    //         /// <param name="parent">父级路径Token</param>
    //         protected TezCategoryPathToken(TValue value, ITezCategoryToken parent) : base(value, parent.layer + 1)
    //         {
    // 
    //         }
    //     }

    /// <summary>
    /// 最终分类节点
    /// 在此节点所标记的object就是实际被实现的object
    /// </summary>
    //     public abstract class TezCategoryFinalToken<TEnum, TValue>
    //         : TezCategoryBaseToken<TEnum, TValue>
    //         , ITezCategoryFinalToken
    //         where TEnum : TezCategoryBaseToken<TEnum, TValue>
    //         where TValue : struct, IComparable
    //     {
    //         /// <summary>
    //         /// 此节点在主分类节点中的index
    //         /// 多用于database
    //         /// </summary>
    //         public int mainTokenIndex { get; }
    // 
    //         /// <summary>
    //         /// 传入类型路径中的父级来构建路径
    //         /// </summary>
    //         /// <param name="parent">父级路径Token</param>
    //         protected TezCategoryFinalToken(TValue value, ITezCategoryToken parent, ITezCategoryMainToken main_token) : base(value, parent.layer + 1)
    //         {
    //             this.mainTokenIndex = main_token.generateID();
    //             main_token.registerFinalToken(this);
    //         }
    //     }
}
