using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Game
{
    public class TezHexBlock
        : ITezCloseable
        , IEquatable<TezHexBlock>
    {
        public TezHexCubeCoordinate coordinate { get; set; }

        bool m_NeedHash = true;
        int m_HashCode = 0;

        public virtual void close()
        {

        }

        /// <summary>
        /// HashCode由坐标计算得出
        /// 因为地块的坐标值唯一
        /// </summary>
        public override int GetHashCode()
        {
            if (m_NeedHash)
            {
                m_NeedHash = false;
                m_HashCode = this.coordinate.GetHashCode();
            }

            return m_HashCode;
        }

        /// <summary>
        /// 判断内存是否相等
        /// 因为地块始终只有一个
        /// </summary>
        public override bool Equals(object obj)
        {
            return this.Equals((TezHexBlock)obj);
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
