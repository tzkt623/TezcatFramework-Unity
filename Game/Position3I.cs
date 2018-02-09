using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public struct Position2I
    {
        public readonly static Position2I zero = new Position2I(0, 0);

        public int x;
        public int y;

        public Position2I(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public static Position2I operator +(Position2I v1, Position2I v2)
        {
            return new Position2I(v1.x + v2.x, v1.y + v2.y);
        }

        public static Position2I operator -(Position2I v1, Position2I v2)
        {
            return new Position2I(v1.x - v2.x, v1.y - v2.y);
        }

        public static bool operator !=(Position2I v1, Position2I v2)
        {
            return v1.x != v2.x || v1.y != v2.y;
        }

        public static bool operator ==(Position2I v1, Position2I v2)
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

    public struct Position3I
    {
        public readonly static Position3I zero = new Position3I(0, 0, 0);

        public int x;
        public int y;
        public int z;

        public Position3I(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public static Position3I operator +(Position3I v1, Position3I v2)
        {
            return new Position3I(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Position3I operator -(Position3I v1, Position3I v2)
        {
            return new Position3I(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static bool operator !=(Position3I v1, Position3I v2)
        {
            return v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
        }

        public static bool operator ==(Position3I v1, Position3I v2)
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