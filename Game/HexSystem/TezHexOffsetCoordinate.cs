using UnityEngine;
using System.Collections;
using tezcat.Framework.Math;
using System;

namespace tezcat.Framework.Game
{
    public struct TezHexOffsetCoordinate
    {
        public int q;
        public int r;

        public TezHexOffsetCoordinate(int q, int r)
        {
            this.q = q;
            this.r = r;
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }

        public override int GetHashCode()
        {
#if false
            var hash = TezHash.intHash(q);
            hash = TezHash.intHash(hash + r);
            return hash;
#else

            var hq = TezHash.intHash(q);
            var hr = TezHash.intHash(r);
            return hq ^ (hr + 0x61C88647 + (hq << 6) + (hq >> 2));
#endif
        }

        public TezHexCubeCoordinate toCube(TezHexGrid.Layout layout)
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

            throw new Exception("TezHexOffsetCoordinate toCoordinate");
        }

        public override string ToString()
        {
            return string.Format("{0},{1}", q, r);
        }
    }
}