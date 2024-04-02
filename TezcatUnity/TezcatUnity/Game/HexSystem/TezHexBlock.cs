using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// Block
    /// 真正应该关心的类
    /// 用于实现各种游戏功能
    /// </summary>
    public class TezHexBlock
        : ITezCloseable
        , IEquatable<TezHexBlock>
    {
        public TezHexCubeCoordinate coordinate;

        bool mNeedHash = true;
        int mHashCode = 0;

        void ITezCloseable.deleteThis()
        {
            this.onClose();
        }

        protected virtual void onClose()
        {

        }

        /// <summary>
        /// HashCode由坐标计算得出
        /// 因为地块的坐标值唯一
        /// </summary>
        public override int GetHashCode()
        {
            if (mNeedHash)
            {
                mNeedHash = false;
                mHashCode = this.coordinate.GetHashCode();
            }

            return mHashCode;
        }

        /// <summary>
        /// 判断内存是否相等
        /// 因为地块始终只有一个
        /// </summary>
        public override bool Equals(object other)
        {
            return this.Equals((TezHexBlock)other);
        }

        /// <summary>
        /// 判断内存是否相等
        /// 因为地块始终只有一个
        /// </summary>
        public bool Equals(TezHexBlock other)
        {
            return object.ReferenceEquals(this, other);
        }

        /// <summary>
        /// 比较的是内存
        /// 因为地块始终只有一个
        /// </summary>
        public static bool operator ==(TezHexBlock a, TezHexBlock b)
        {
            return object.ReferenceEquals(a, b);
        }

        /// <summary>
        /// 比较的是内存
        /// 因为地块始终只有一个
        /// </summary>
        public static bool operator !=(TezHexBlock a, TezHexBlock b)
        {
            return !(a == b);
        }
    }
}
