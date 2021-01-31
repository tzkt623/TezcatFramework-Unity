using System.Collections.Generic;
using tezcat.Framework.Utility;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// 记得重载GetHashCode()以获得相等比较的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TezAStarNode<T>
        : ITezAStarNode<T>
        where T : TezAStarNode<T>, new()
    {
        #region Pool
        static Queue<ITezAStarNode> s_Pool = new Queue<ITezAStarNode>();
        public static T create()
        {
            if (s_Pool.Count > 0)
            {
                return (T)s_Pool.Dequeue();
            }

            return new T();
        }

        static void recycle(ITezAStarNode obj)
        {
            s_Pool.Enqueue(obj);
        }
        #endregion

        public ITezAStarNode parent { get; set; }

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

        public virtual int CompareTo(T other)
        {
            int compare = this.fCost.CompareTo(other.fCost);
            if (compare == 0)
            {
                compare = this.hCost.CompareTo(other.hCost);
            }
            return -compare;
        }

        public sealed override int GetHashCode()
        {
            return this.onGetHashCode();
        }

        protected abstract int onGetHashCode();

        public override bool Equals(object obj)
        {
            return this.Equals((T)obj);
        }

        public virtual bool Equals(T other)
        {
            if (object.ReferenceEquals(other, null))
            {
                return false;
            }

            return this.GetHashCode() == other.GetHashCode();
        }

        public virtual void close()
        {
            this.parent = null;
            recycle(this);
        }

        public static bool operator ==(TezAStarNode<T> a, TezAStarNode<T> b)
        {
            if (object.ReferenceEquals(a, null))
            {
                return object.ReferenceEquals(b, null);
            }

            return a.Equals(b);
        }

        public static bool operator !=(TezAStarNode<T> a, TezAStarNode<T> b)
        {
            return !(a == b);
        }
    }
}

