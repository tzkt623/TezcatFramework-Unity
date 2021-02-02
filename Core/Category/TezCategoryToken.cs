using System;
using System.Collections.Generic;
using tezcat.Framework.TypeTraits;

namespace tezcat.Framework.Core
{
    public enum TezCategoryTokenClassify
    {
        Root,
        Path,
        Final
    }

    public interface ITezCategoryBaseToken : ITezEnumeration
    {
        /// <summary>
        /// Token存在的层
        /// 即在Category系统里TokenArray中的Index
        /// </summary>
        int layer { get; }

        /// <summary>
        /// 分类
        /// </summary>
        TezCategoryTokenClassify classify { get; }
    }

    public interface ITezCategoryToken : ITezCategoryBaseToken
    {
        /// <summary>
        /// FinalToken在Root中的位置
        /// 只有final才会有
        /// 其他为-1
        /// </summary>
        int finalIndexInRootToken { get; }
    }

    public interface ITezCategoryRootToken : ITezCategoryBaseToken
    {
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

    public abstract class TezCategoryBaseToken<Self, TValue>
        : TezEnumeration<Self, TValue>
        , ITezCategoryBaseToken
        where Self : TezCategoryBaseToken<Self, TValue>
        where TValue : struct, IComparable
    {
        public abstract TezCategoryTokenClassify classify { get; }

        /// <summary>
        /// Path中的层级
        /// </summary>
        public int layer { get; }

        protected TezCategoryBaseToken(TValue value, int layer) : base(value)
        {
            this.layer = layer;
        }
    }

    /// <summary>
    /// 主分类节点
    /// 用于对object进行分类
    /// </summary>
    public abstract class TezCategoryRootToken<TEnum, TValue>
        : TezCategoryBaseToken<TEnum, TValue>
        , ITezCategoryRootToken
        where TEnum : TezCategoryRootToken<TEnum, TValue>
        where TValue : struct, IComparable
    {
        List<ITezCategoryToken> m_FinalTokenList = new List<ITezCategoryToken>();
        Dictionary<string, ITezCategoryToken> m_FinalTokenDic = new Dictionary<string, ITezCategoryToken>();

        public override TezCategoryTokenClassify classify => TezCategoryTokenClassify.Root;

        /// <summary>
        /// 用于创建RootToken
        /// </summary>
        protected TezCategoryRootToken(TValue value) : base(value, 0)
        {
            TezCategorySystem.registerRootToken(this);
        }

        public ITezCategoryToken get(string name)
        {
            return m_FinalTokenDic[name];
        }

        public ITezCategoryToken get(int index)
        {
            return m_FinalTokenList[index];
        }

        int ITezCategoryRootToken.generateID()
        {
            return m_FinalTokenList.Count;
        }

        void ITezCategoryRootToken.registerFinalToken(ITezCategoryToken token)
        {
            m_FinalTokenList.Add(token);
            m_FinalTokenDic.Add(token.toName, token);
        }
    }

    /// <summary>
    /// 路径分类节点
    /// 通过传入父路径节点来建立联系
    /// </summary>
    public abstract class TezCategoryToken<Self, TValue>
        : TezCategoryBaseToken<Self, TValue>
        , ITezCategoryToken
        where Self : TezCategoryBaseToken<Self, TValue>
        where TValue : struct, IComparable
    {
        public override TezCategoryTokenClassify classify
        {
            get
            {
                switch (this.finalIndexInRootToken)
                {
                    case -1:
                        return TezCategoryTokenClassify.Path;
                    default:
                        return TezCategoryTokenClassify.Final;
                }
            }
        }

        /// <summary>
        /// FinalToken在Root中的位置
        /// 只有final才会有
        /// 其他为-1
        /// </summary>
        public int finalIndexInRootToken { get; } = -1;

        /// <summary>
        /// 用于创建PathToken
        /// </summary>
        /// <param name="parentToken">Path中的上一级</param>
        protected TezCategoryToken(TValue value, ITezCategoryBaseToken parentToken) : base(value, parentToken.layer + 1)
        {

        }

        /// <summary>
        /// 用于创建FinalToken
        /// </summary>
        /// <param name="value"></param>
        /// <param name="parentToken">Path中的上一级</param>
        /// <param name="rootToken">Root级Token</param>
        protected TezCategoryToken(TValue value, ITezCategoryBaseToken parentToken, ITezCategoryRootToken rootToken) : base(value, parentToken.layer + 1)
        {
            this.finalIndexInRootToken = rootToken.generateID();
            rootToken.registerFinalToken(this);
        }
    }
}
