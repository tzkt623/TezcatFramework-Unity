using System;
using tezcat.Framework.Math;
using UnityEngine;

namespace tezcat.Framework.Game
{
    public struct TezHexCubeCoordinate : IEquatable<TezHexCubeCoordinate>
    {
        public static readonly TezHexCubeCoordinate zero = new TezHexCubeCoordinate(0, 0, 0, true);
        public static readonly TezHexCubeCoordinate one = new TezHexCubeCoordinate(1, 1, 1, true);
        public static readonly TezHexCubeCoordinate max = new TezHexCubeCoordinate(int.MaxValue, int.MaxValue, int.MaxValue, true);
        public static readonly TezHexCubeCoordinate min = new TezHexCubeCoordinate(int.MinValue, int.MinValue, int.MinValue, true);

        #region 朝向计算
        /// <summary>
        /// 1左移(V+1)位
        /// 
        /// </summary>
        const int MaskX001 = 1 << 0;
        const int MaskX010 = 1 << 1;
        const int MaskX100 = 1 << 2;

        const int MaskY001 = 1 << 3;
        const int MaskY010 = 1 << 4;
        const int MaskY100 = 1 << 5;

        const int MaskZ001 = 1 << 6;
        const int MaskZ010 = 1 << 7;
        const int MaskZ100 = 1 << 8;

        const int Z_XY = MaskZ010 | MaskX100 | MaskY001;
        const int Y_XZ = MaskY010 | MaskX100 | MaskZ001;
        const int X_YZ = MaskX010 | MaskY100 | MaskZ001;
        #endregion

        public int x;
        public int y;
        public int z;

        public int q
        {
            set { x = value; }
            get { return x; }
        }
        public int r
        {
            set { z = value; }
            get { return z; }
        }

        private TezHexCubeCoordinate(int x, int y, int z, bool holder)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public TezHexCubeCoordinate(int x, int y, int z)
        {
#if UNITY_EDITOR
            if (x + y + z != 0)
            {
                throw new Exception(string.Format("TezHexCubeCoordinate : {0},{1},{2}", x, y, z));
            }
#endif

            this.x = x;
            this.y = y;
            this.z = z;
        }

        public TezHexCubeCoordinate(int q, int r)
        {
            this.x = q;
            this.y = -q - r;
            this.z = r;
        }

        public bool Equals(TezHexCubeCoordinate other)
        {
            return this.x == other.x && this.z == other.z;
        }

        public override bool Equals(object other)
        {
            return this.Equals((TezHexCubeCoordinate)other);
        }

        /// <summary>
        /// 请包装类记录一下Hash值
        /// </summary>
        public override int GetHashCode()
        {
#if false
            ///经过测试
            ///此算法约80W数据时冲突362个
            var hash = TezHash.intHash(q);
            hash = TezHash.intHash(hash + r);
            return hash;
#else
            ///经过测试
            ///此算法约80W数据时冲突76个值
            var hq = TezHash.intHash(q);
            var hr = TezHash.intHash(r);
            return hq ^ (hr + 0x61C88647 + (hq << 6) + (hq >> 2));
#endif
        }

        public override string ToString()
        {
            return string.Format("[{0},{1},{2}]", x, y, z);
        }

        public static TezHexCubeCoordinate operator +(TezHexCubeCoordinate v1, TezHexCubeCoordinate v2)
        {
            return new TezHexCubeCoordinate(v1.x + v2.x, v1.z + v2.z);
        }

        public static TezHexCubeCoordinate operator -(TezHexCubeCoordinate v1, TezHexCubeCoordinate v2)
        {
            return new TezHexCubeCoordinate(v1.x - v2.x, v1.z - v2.z);
        }

        public static bool operator !=(TezHexCubeCoordinate v1, TezHexCubeCoordinate v2)
        {
            return v1.x != v2.x || v1.z != v2.z;
        }

        public static bool operator ==(TezHexCubeCoordinate v1, TezHexCubeCoordinate v2)
        {
            return v1.x == v2.x && v1.z == v2.z;
        }
        public TezHexOffsetCoordinate toOffset(TezHexGrid.Layout layout)
        {
            switch (layout)
            {
                case TezHexGrid.Layout.Pointy:
                    {
                        ///-5, 0, 5
                        ///-5 + (5 + (5 & 1) >> 1)
                        ///-5 + (6 >> 1)
                        ///-5 + 3 = -2
                        /// = 5
                        var q = x + (z + (z & 1) >> 1);
                        var r = z;
                        return new TezHexOffsetCoordinate(q, r);
                    }
                case TezHexGrid.Layout.Flat:
                    {
                        var q = x;
                        var r = z + (x + (x & 1) >> 1);
                        return new TezHexOffsetCoordinate(q, r);
                    }
            }

            throw new Exception("TezHexCoordinate toOffsetCoordinate");
        }

        public void set(int x, int y, int z)
        {
            this.x = x;
            this.z = z;
        }

        public int getDistanceFrom(TezHexCubeCoordinate other)
        {
            return (Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y) + Mathf.Abs(z - other.z)) / 2;
        }

        /// <summary>
        /// 从目标坐标获得标准化的朝向方向
        /// </summary>
        public TezHexCubeCoordinate getDirectionFrom(TezHexCubeCoordinate target)
        {
            var x = target.x - this.x;
            var y = target.y - this.y;
            var z = target.z - this.z;

            var distance = (Mathf.Abs(x) + Mathf.Abs(y) + Mathf.Abs(z)) / 2;
            return new TezHexCubeCoordinate(x / distance, y / distance, z / distance);
        }

        public void scale(int value)
        {
            this.x *= value;
            this.y *= value;
            this.z *= value;
        }

        public void add(int x, int y, int z)
        {
            this.x += x;
            this.y += y;
            this.z += z;
        }

        public void rotateLeft()
        {
            var x = -this.y;
            var y = -this.z;
            var z = -this.x;

            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void rotateRight()
        {
            var x = -this.z;
            var y = -this.x;
            var z = -this.y;

            this.x = x;
            this.y = y;
            this.z = z;
        }

        public void rotate180()
        {
            var x = -this.x;
            var y = -this.y;
            var z = -this.z;

            this.x = x;
            this.y = y;
            this.z = z;
        }

        public TezHexCubeCoordinate rotateToLeft()
        {
            return new TezHexCubeCoordinate(-this.y, -this.z, -this.x);
        }

        public TezHexCubeCoordinate rotateToRight()
        {
            return new TezHexCubeCoordinate(-this.z, -this.x, -this.y);
        }

        public TezHexCubeCoordinate rotateTo180()
        {
            return new TezHexCubeCoordinate(-this.x, -this.y, -this.z);
        }

        public static TezHexCubeCoordinate fromAxial(int q, int r)
        {
            return new TezHexCubeCoordinate(q, r);
        }

        public static TezHexCubeCoordinate fromOffset(int q, int r, TezHexGrid.Layout layout)
        {
            switch (layout)
            {
                case TezHexGrid.Layout.Pointy:
                    {
                        var x = q - (r + (r & 1) >> 1);
                        var z = r;
                        return new TezHexCubeCoordinate(x, z);
                    }
                case TezHexGrid.Layout.Flat:
                    {
                        var x = q;
                        var z = r - (q + (q & 1) >> 1);
                        return new TezHexCubeCoordinate(x, z);
                    }
            }

            throw new Exception("TezHexCoordinate fromOffset");
        }

    }
}