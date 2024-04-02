using tezcat.Framework.Core;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// Wrapper层
    /// 如果有Wrapper层存在
    /// 由于比较的是HashCode来判断相等
    /// 则Heap查询的方式必然会出现相同的内部Data却查不到的问题
    /// 
    /// 所以必须有个管理器记录下所有已经产生过的Wrapper
    /// 以便当查询到同一个Wrapper时 不用再次产生
    /// </summary>
    public abstract class TezAStarDataWrapper<Self, BlockData>
        : ITezAStarDataWrapper<Self, BlockData>
        where Self : TezAStarDataWrapper<Self, BlockData>, new()
    {
        public ITezAStarDataWrapper parent { get; set; }

        public BlockData blockData { get; set; }

        public object data => this.blockData;

        int ITezBinaryHeapItem.index { get; set; } = -1;

        /// <summary>
        /// 移动的代价
        /// 上一个点的代价+上一个点到此点的代价
        /// 等于此点的gCost
        /// </summary>
        public int gCost { get; set; }

        /// <summary>
        /// 距离终点无视任何阻挡的最短路径
        /// </summary>
        public int hCost { get; set; }

        /// <summary>
        /// 所有Cost
        /// </summary>
        public virtual int fCost
        {
            get { return this.gCost + this.hCost; }
        }

        public virtual bool isBlocked()
        {
            return false;
        }

        public virtual int CompareTo(Self other)
        {
            int compare = this.fCost.CompareTo(other.fCost);
            if (compare == 0)
            {
                compare = this.hCost.CompareTo(other.hCost);
            }
            return -compare;
        }

        /// <summary>
        /// HashCode是由内部数据impData计算得出
        /// </summary>
        public sealed override int GetHashCode()
        {
            return this.blockData.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return this.Equals((Self)obj);
        }

        public virtual bool Equals(Self other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return this.blockData.Equals(other.blockData);
        }

        void ITezCloseable.deleteThis()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {
            this.parent = null;
            this.blockData = default;
        }

        public static bool operator ==(TezAStarDataWrapper<Self, BlockData> a, TezAStarDataWrapper<Self, BlockData> b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.blockData.Equals(b.blockData);
        }

        public static bool operator !=(TezAStarDataWrapper<Self, BlockData> a, TezAStarDataWrapper<Self, BlockData> b)
        {
            return !(a == b);
        }
    }
}

