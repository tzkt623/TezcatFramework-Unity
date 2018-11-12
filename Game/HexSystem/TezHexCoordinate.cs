using System;
using tezcat.Framework.Math;
using UnityEngine;

namespace tezcat.Framework.Game
{
    public struct TezHexCubeCoordinate : IEquatable<TezHexCubeCoordinate>
    {
        public static readonly TezHexCubeCoordinate zero = new TezHexCubeCoordinate(0, 0, 0);
        public static readonly TezHexCubeCoordinate one = new TezHexCubeCoordinate(1, 1, 1);
        public static readonly TezHexCubeCoordinate max = new TezHexCubeCoordinate(int.MaxValue, int.MaxValue, int.MaxValue);
        public static readonly TezHexCubeCoordinate min = new TezHexCubeCoordinate(int.MinValue, int.MinValue, int.MinValue);

        public int x;
        public int y
        {
            get { return -x - z; }
        }
        public int z;

        public int q
        {
            get { return x; }
            set { x = value; }
        }
        public int r
        {
            get { return z; }
            set { z = value; }
        }

        public TezHexCubeCoordinate(int x, int y, int z)
        {
            this.x = x;
            this.z = z;
        }

        public TezHexCubeCoordinate(int q, int r)
        {
            this.x = q;
            this.z = r;
        }

        public TezHexOffsetCoordinate toOffset(TezHexGrid.Layout layout)
        {
            switch (layout)
            {
                case TezHexGrid.Layout.Pointy:
                    {
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

        public bool Equals(TezHexCubeCoordinate other)
        {
            return this.x == other.x && this.z == other.z;
        }

        public override bool Equals(object other)
        {
            return this.Equals((TezHexCubeCoordinate)other);
        }

        public override int GetHashCode()
        {
            var hash = TezHash.intHash(x);
            hash = TezHash.intHash(hash + z);
            return hash;
        }

        public int getDistanceFrom(TezHexCubeCoordinate other)
        {
            return (Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y) + Mathf.Abs(z - other.z)) / 2;
        }

        public override string ToString()
        {
            return string.Format("{0},{1},{2}", x, y, z);
        }

        public void scale(int radius)
        {
            this.x *= radius;
            this.z *= radius;
        }

        public void add(int x, int z)
        {
            this.x += x;
            this.z += z;
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
    }
}