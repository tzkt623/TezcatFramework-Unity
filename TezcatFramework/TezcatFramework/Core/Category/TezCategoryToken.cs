using System;

namespace tezcat.Framework.Core
{
    public enum TezCategoryTokenType
    {
        Root = 0,
        Path = 1,
        Final = 2
    }

    public interface ITezCategoryBaseToken : ITezNonCloseable
    {
        /// <summary>
        /// 名称
        /// </summary>
        string name { get; }

        /// <summary>
        /// Token的UID
        /// 用于比较两个Token是否相同
        /// </summary>
        int UID { get; }

        /// <summary>
        /// Token存在的层
        /// 即在Category系统里TokenArray中的Index
        /// </summary>
        int layerID { get; }

        /// <summary>
        /// Token的int值
        /// </summary>
        int intValue { get; }

        /// <summary>
        /// Token分类
        /// </summary>
        TezCategoryTokenType tokenType { get; }

        /// <summary>
        /// 父级
        /// 用于回溯
        /// Root没有父级
        /// </summary>
        ITezCategoryBaseToken parent { get; }
    }

    public interface ITezCategoryRootToken : ITezCategoryBaseToken
    {

    }

    public interface ITezCategoryPathToken : ITezCategoryBaseToken
    {

    }

    public interface ITezCategoryFinalToken : ITezCategoryBaseToken
    {
        /// <summary>
        /// 全局ID
        /// 无差别赋予的ID
        /// 可用于索引
        /// </summary>
        int globalID { get; }

        int typeID { get; }

        void setTypeID(int id);
    }

    public abstract class TezCategoryBaseToken<Self, TEnumValue>
        : ITezCategoryBaseToken
        where Self : TezCategoryBaseToken<Self, TEnumValue>
        where TEnumValue : struct, IComparable
    {
        public abstract TezCategoryTokenType tokenType { get; }

        public TEnumValue enumValue { get; }
        public int intValue { get; }
        public string name { get; }

        /// <summary>
        /// 路径中的层级
        /// </summary>
        public int layerID { get; }
        public int UID { get; } = -1;

        public ITezCategoryBaseToken parent { get; }

        protected TezCategoryBaseToken(TEnumValue enumValue, int layer, ITezCategoryBaseToken parent)
        {
            this.enumValue = enumValue;
            this.intValue = Convert.ToInt32(this.enumValue);

            this.layerID = layer;
            this.parent = parent;

            this.name = Enum.GetName(typeof(TEnumValue), this.enumValue);
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
        public sealed override TezCategoryTokenType tokenType => TezCategoryTokenType.Root;

        /// <summary>
        /// 用于创建RootToken
        /// </summary>
        protected TezCategoryRootToken(TValue value) : base(value, 0, null)
        {
            TezCategorySystem.registerRootToken(this);
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
        public sealed override TezCategoryTokenType tokenType => TezCategoryTokenType.Path;

        /// <summary>
        /// 用于创建PathToken
        /// </summary>
        /// <param name="parentToken">Path中的上一级</param>
        protected TezCategoryPathToken(TValue value, ITezCategoryBaseToken parentToken) : base(value, parentToken.layerID + 1, parentToken)
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
        public sealed override TezCategoryTokenType tokenType => TezCategoryTokenType.Final;

        int mGlobalID = -1;
        /// <summary>
        /// 全局ID
        /// 无差别赋予的ID
        /// 可用于索引
        /// </summary>
        public int globalID => mGlobalID;

        int mTypeID = -1;
        public int typeID => mTypeID;

        protected TezCategoryFinalToken(TValue value, ITezCategoryBaseToken parentToken) : base(value, parentToken.layerID + 1, parentToken)
        {

        }

        protected void registerID()
        {
            mGlobalID = TezCategorySystem.registerFinalToken(this);
            TezCategorySystem.createCategory(this);
        }

        void ITezCategoryFinalToken.setTypeID(int id)
        {
            if(mTypeID == -1)
            {
                mTypeID = id;
            }
            else
            {
                if(mTypeID != id)
                {
                    throw new Exception();
                }
            }
        }
    }
}