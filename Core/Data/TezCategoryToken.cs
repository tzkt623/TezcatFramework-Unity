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
    }

    public interface ITezCategoryMainToken : ITezCategoryToken
    {
        ITezCategoryFinalToken get(string name);
        ITezCategoryFinalToken get(int index);

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
        void registerFinalToken(ITezCategoryFinalToken token);
    }

    public interface ITezCategoryFinalToken : ITezCategoryToken
    {
        /// <summary>
        /// 用于在MainToken中进行排序
        /// </summary>
        int index { get; }
    }

    public abstract class TezCategoryBaseToken<TEnum, TValue>
        : TezEnumeration<TEnum, TValue>
        , ITezCategoryToken
        where TEnum : TezCategoryBaseToken<TEnum, TValue>
        where TValue : struct, IComparable
    {
        public int layer { get; }

        protected TezCategoryBaseToken(TValue value, int layer) : base(value)
        {
            this.layer = layer;
        }
    }

    public abstract class TezCategoryMainToken<TEnum, TValue>
        : TezCategoryBaseToken<TEnum, TValue>
        , ITezCategoryMainToken
        where TEnum : TezCategoryBaseToken<TEnum, TValue>
        where TValue : struct, IComparable
    {
        private int m_ID = 0;
        List<ITezCategoryFinalToken> m_FinalTokenList = new List<ITezCategoryFinalToken>();
        Dictionary<string, ITezCategoryFinalToken> m_FinalTokenDic = new Dictionary<string, ITezCategoryFinalToken>();

        protected TezCategoryMainToken(TValue value) : base(value, 0)
        {
            TezCategorySystem.registerMainToken(this);
        }

        int ITezCategoryMainToken.generateID()
        {
            return m_FinalTokenList.Count;
        }

        void ITezCategoryMainToken.registerFinalToken(ITezCategoryFinalToken token)
        {
            m_FinalTokenList.Add(token);
            m_FinalTokenDic.Add(token.toName, token);
        }

        /// <summary>
        /// 通过Name获得FinalToken
        /// </summary>
        public ITezCategoryFinalToken get(string name)
        {
            ITezCategoryFinalToken token = null;
            if(m_FinalTokenDic.TryGetValue(name, out token))
            {
                return token;
            }

            throw new Exception(string.Format("{0} : no finaltoken named {1}", this.toName, name));
        }

        /// <summary>
        /// 通过Index获得FinalToken
        /// </summary>
        public ITezCategoryFinalToken get(int index)
        {
            return m_FinalTokenList[index];
        }
    }

    public abstract class TezCategoryPathToken<TEnum, TValue>
        : TezCategoryBaseToken<TEnum, TValue>
        where TEnum : TezCategoryBaseToken<TEnum, TValue>
        where TValue : struct, IComparable
    {
        /// <summary>
        /// 传入类型路径中的父级来构建路径
        /// </summary>
        /// <param name="parent">父级路径Token</param>
        protected TezCategoryPathToken(TValue value, ITezCategoryToken parent) : base(value, parent.layer + 1)
        {

        }
    }

    public abstract class TezCategoryFinalToken<TEnum, TValue>
        : TezCategoryBaseToken<TEnum, TValue>
        , ITezCategoryFinalToken
        where TEnum : TezCategoryBaseToken<TEnum, TValue>
        where TValue : struct, IComparable
    {
        public int index { get; }

        /// <summary>
        /// 传入类型路径中的父级来构建路径
        /// </summary>
        /// <param name="parent">父级路径Token</param>
        protected TezCategoryFinalToken(TValue value, ITezCategoryToken parent, ITezCategoryMainToken main_token) : base(value, parent.layer + 1)
        {
            this.index = main_token.generateID();
            main_token.registerFinalToken(this);
        }
    }
}
