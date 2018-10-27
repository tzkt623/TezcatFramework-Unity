namespace tezcat.Framework.Utility
{
    public struct TezPosition2I
    {
        public readonly static TezPosition2I zero = new TezPosition2I(0, 0);

        public int x;
        public int y;

        public TezPosition2I(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static TezPosition2I operator +(TezPosition2I v1, TezPosition2I v2)
        {
            return new TezPosition2I(v1.x + v2.x, v1.y + v2.y);
        }

        public static TezPosition2I operator -(TezPosition2I v1, TezPosition2I v2)
        {
            return new TezPosition2I(v1.x - v2.x, v1.y - v2.y);
        }

        public static bool operator !=(TezPosition2I v1, TezPosition2I v2)
        {
            return v1.x != v2.x || v1.y != v2.y;
        }

        public static bool operator ==(TezPosition2I v1, TezPosition2I v2)
        {
            return v1.x == v2.x && v1.y == v2.y;
        }

        public void set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 47;
                hash = hash * 227 + x.GetHashCode();
                hash = hash * 227 + y.GetHashCode();

                return hash;
            }
        }
    }

    public struct TezPosition3I
    {
        public readonly static TezPosition3I zero = new TezPosition3I(0, 0, 0);

        public int x;
        public int y;
        public int z;

        public TezPosition3I(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static TezPosition3I operator +(TezPosition3I v1, TezPosition3I v2)
        {
            return new TezPosition3I(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static TezPosition3I operator -(TezPosition3I v1, TezPosition3I v2)
        {
            return new TezPosition3I(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static bool operator !=(TezPosition3I v1, TezPosition3I v2)
        {
            return v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
        }

        public static bool operator ==(TezPosition3I v1, TezPosition3I v2)
        {
            return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
        }

        public void set(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public override bool Equals(object obj)
        {
            return obj.GetHashCode() == this.GetHashCode();
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 47;
                hash = hash * 227 + x.GetHashCode();
                hash = hash * 227 + y.GetHashCode();
                hash = hash * 227 + z.GetHashCode();

                return hash;
            }
        }
    }
}