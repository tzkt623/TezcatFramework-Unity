using System;
using System.Collections.Generic;
using tezcat.Framework.TypeTraits;
using UnityEngine;

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
        /// Token的UID
        /// </summary>
        int UID { get; }

        /// <summary>
        /// Token存在的层
        /// 即在Category系统里TokenArray中的Index
        /// </summary>
        int layer { get; }

        /// <summary>
        /// 分类
        /// </summary>
        TezCategoryTokenClassify classify { get; }

        /// <summary>
        /// 父级
        /// Root没有父级
        /// </summary>
        ITezCategoryBaseToken parent { get; }
    }

    public interface ITezCategoryPathToken : ITezCategoryBaseToken
    {

    }

    public interface ITezCategoryFinalToken : ITezCategoryBaseToken
    {
        /// <summary>
        /// 全局ID
        /// </summary>
        int finalUID { get; }

        /// <summary>
        /// 基于Root下的全局ID
        /// </summary>
        int finalRID { get; }
    }

    public interface ITezCategoryRootToken : ITezCategoryBaseToken
    {
        /// <summary>
        /// 通过名称获得Token
        /// </summary>
        ITezCategoryBaseToken get(string name);

        /// <summary>
        /// 通过索引获得Token
        /// </summary>
        ITezCategoryBaseToken get(int index);

        /// <summary>
        /// 注册FinalToken
        /// 请勿手动调用
        /// </summary>
        int registerFinalToken(ITezCategoryFinalToken token);
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

        public ITezCategoryBaseToken parent { get; }

        public int UID { get; } = -1;

        protected TezCategoryBaseToken(TValue value, int layer, ITezCategoryBaseToken parent) : base(value)
        {
            this.layer = layer;
            this.parent = parent;
            this.UID = TezCategorySystem.registerToken(this);
//            Debug.Log(string.Format("{0}:{1}", this.toName, this.UID));
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
        List<ITezCategoryBaseToken> m_FinalTokenList = new List<ITezCategoryBaseToken>();
        Dictionary<string, ITezCategoryBaseToken> m_FinalTokenDic = new Dictionary<string, ITezCategoryBaseToken>();

        public sealed override TezCategoryTokenClassify classify => TezCategoryTokenClassify.Root;

        /// <summary>
        /// 用于创建RootToken
        /// </summary>
        protected TezCategoryRootToken(TValue value) : base(value, 0, null)
        {
            TezCategorySystem.registerRootToken(this);
        }

        public ITezCategoryBaseToken get(string name)
        {
            return m_FinalTokenDic[name];
        }

        public ITezCategoryBaseToken get(int index)
        {
            return m_FinalTokenList[index];
        }

        int ITezCategoryRootToken.registerFinalToken(ITezCategoryFinalToken token)
        {
            var id = m_FinalTokenList.Count;
            m_FinalTokenList.Add(token);
            m_FinalTokenDic.Add(token.toName, token);
            return id;
        }
    }

    /// <summary>
    /// 路径分类节点
    /// 通过传入父路径节点来建立联系
    /// </summary>
    public abstract class TezCategoryPathToken<Self, TValue>
        : TezCategoryBaseToken<Self, TValue>
        , ITezCategoryPathToken
        where Self : TezCategoryBaseToken<Self, TValue>
        where TValue : struct, IComparable
    {
        public sealed override TezCategoryTokenClassify classify => TezCategoryTokenClassify.Path;


        /// <summary>
        /// 用于创建PathToken
        /// </summary>
        /// <param name="parentToken">Path中的上一级</param>
        protected TezCategoryPathToken(TValue value, ITezCategoryBaseToken parentToken) : base(value, parentToken.layer + 1, parentToken)
        {
        }
    }


    /// <summary>
    /// 终节点
    /// 用于最终的具体分类
    /// </summary>
    public abstract class TezCategoryFinalToken<Self, TValue>
        : TezCategoryBaseToken<Self, TValue>
        , ITezCategoryFinalToken
        where Self : TezCategoryBaseToken<Self, TValue>
        where TValue : struct, IComparable
    {
        public sealed override TezCategoryTokenClassify classify => TezCategoryTokenClassify.Final;

        int m_FinalUID = -1;
        int m_FinalRID = -1;

        public int finalUID => m_FinalUID;
        public int finalRID => m_FinalRID;


        protected TezCategoryFinalToken(TValue value, ITezCategoryBaseToken parentToken) : base(value, parentToken.layer + 1, parentToken)
        {
        }

        protected void registerID(ITezCategoryRootToken rootToken)
        {
            m_FinalRID = rootToken.registerFinalToken(this);
            m_FinalUID = TezCategorySystem.registerFinalToken(this);
        }
    }
}