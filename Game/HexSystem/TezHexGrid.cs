using System.Collections.Generic;
using tezcat.Framework.Math;
using UnityEngine;

namespace tezcat.Framework.Game
{
    public class TezHexGrid
    {
        public static readonly float Sqrt3 = Mathf.Sqrt(3);
        public static readonly float Sqrt3D2 = Mathf.Sqrt(3) / 2;
        public static readonly float Sqrt3D3 = Mathf.Sqrt(3) / 3;

        public enum Direction : int
        {
            P_N_Z,
            P_Z_N,
            Z_P_N,
            N_P_Z,
            N_Z_P,
            Z_N_P
        }

        public static readonly Coordinate[] CubeDirections = new Coordinate[6]
        {
            new Coordinate(1, -1, 0),
            new Coordinate(1, 0, -1),
            new Coordinate(0, 1, -1),
            new Coordinate(-1, 1, 0),
            new Coordinate(-1, 0, 1),
            new Coordinate(0, -1, 1)
        };

        public static readonly int[] HexTriangleIndices = new int[]
        {
            0, 6, 5,
            0, 5, 4,
            0, 4, 3,
            0, 3, 2,
            0, 2, 1,
            0, 1, 6
        };

        public static readonly int[] BorderTriangleIndices = new int[]
        {
            0,  5,  11,
            5,  4,  10,
            4,  3,  9,
            3,  2,  8,
            2,  1,  7,
            1,  0,  6,

            6,  0,  11,
            11, 5,  10,
            10, 4,  9,
            9,  3,  8,
            8,  2,  7,
            7,  1,  6
        };

        public static Coordinate calculateCoordinate(Coordinate center, Coordinate dir)
        {
            return new Coordinate(center.x + dir.x, center.z + dir.z);
        }

        public static Coordinate neighbor(Coordinate center, Coordinate dir)
        {
            return new Coordinate(center.x + dir.x, center.z + dir.z);
        }

        public static Coordinate direction(Direction direction)
        {
            return CubeDirections[(int)direction];
        }

        public static List<Coordinate> range(Coordinate center, int range)
        {
            List<Coordinate> list = new List<Coordinate>();
            list.Capacity = (1 + range) * range / 2 * 6 + 1;

            for (int z = -range; z <= range; z++)
            {
                for (int y = -range; y <= range; y++)
                {
                    for (int x = -range; x <= range; x++)
                    {
                        if(x + y + z != 0)
                        {
                            continue;
                        }

                        list.Add(new Coordinate(center.x + x, center.z + z));
                    }
                }
            }

            return list;
        }

        public static List<Coordinate> ring(Coordinate center, int radius)
        {
            List<Coordinate> list = new List<Coordinate>();
            list.Capacity = 6 * radius;

            var begin = CubeDirections[4];
            begin.scale(radius);
            begin.add(center.x, center.z);

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    list.Add(begin);
                    begin = neighbor(begin, CubeDirections[i]);
                }
            }

            return list;
        }

        public interface Node
        {
            Coordinate coordinate { get; }
        }

        public class Container<T> where T : class, Node
        {
            class Wrapper
            {
                /// <summary>
                /// 1,1
                /// 1,-1
                /// -1,1
                /// -1,-1
                /// </summary>
                T[] m_Array = new T[4];

                public T get(int q, int r)
                {
                    return m_Array[(q < 0 ? 2 : 0) + (r < 0 ? 1 : 0)];
                }

                public void add(T node)
                {
                    var c = node.coordinate;
                    m_Array[(c.q < 0 ? 2 : 0) + (c.r < 0 ? 1 : 0)] = node;
                }

                public void close()
                {
                    m_Array = null;
                }
            }

            Wrapper[,] m_List = new Wrapper[1, 1];

            public void add(T node)
            {
                var coordinate = node.coordinate;
                var q = Mathf.Abs(coordinate.q);
                var r = Mathf.Abs(coordinate.r);

                var grow_q = m_List.GetLength(0) < q;
                var grow_r = m_List.GetLength(0) < r;

            }

            public T get(int q, int r)
            {
                return m_List[Mathf.Abs(q), Mathf.Abs(r)].get(q, r);
            }
        }

        public struct Coordinate
        {
            public static readonly Coordinate zero = new Coordinate(0, 0, 0);
            public static readonly Coordinate one = new Coordinate(1, 1, 1);
            public static readonly Coordinate max = new Coordinate(int.MaxValue, int.MaxValue, int.MaxValue);
            public static readonly Coordinate min = new Coordinate(int.MinValue, int.MinValue, int.MinValue);

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

            public Coordinate(int x, int y, int z)
            {
                this.x = x;
                this.z = z;
            }

            public Coordinate(int q, int r)
            {
                this.x = q;
                this.z = r;
            }

            public static Coordinate operator +(Coordinate v1, Coordinate v2)
            {
                return new Coordinate(v1.x + v2.x, v1.z + v2.z);
            }

            public static Coordinate operator -(Coordinate v1, Coordinate v2)
            {
                return new Coordinate(v1.x - v2.x, v1.z - v2.z);
            }

            public static bool operator !=(Coordinate v1, Coordinate v2)
            {
                return v1.x != v2.x || v1.z != v2.z;
            }

            public static bool operator ==(Coordinate v1, Coordinate v2)
            {
                return v1.x == v2.x && v1.z == v2.z;
            }

            public void set(int x, int y, int z)
            {
                this.x = x;
                this.z = z;
            }

            public override bool Equals(object obj)
            {
                return obj.GetHashCode() == this.GetHashCode();
            }

            public override int GetHashCode()
            {
                var hash = TezHash.intHash(x);
                hash = TezHash.intHash(hash + z);
                return hash;
            }

            public int getDistanceFrom(Coordinate other)
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
        }

#if false
        public struct AxialCoordinate : IEquatable<AxialCoordinate>
        {
            public static readonly AxialCoordinate zero = new AxialCoordinate(0, 0);
            public static readonly AxialCoordinate one = new AxialCoordinate(1, 1);
            public static readonly AxialCoordinate min = new AxialCoordinate(int.MinValue, int.MinValue);
            public static readonly AxialCoordinate max = new AxialCoordinate(int.MaxValue, int.MaxValue);

            public int q;
            public int r;

            public AxialCoordinate(int q, int r)
            {
                this.q = q;
                this.r = r;
            }

            public static AxialCoordinate operator +(AxialCoordinate v1, AxialCoordinate v2)
            {
                return new AxialCoordinate(v1.q + v2.q, v1.r + v2.r);
            }

            public static AxialCoordinate operator -(AxialCoordinate v1, AxialCoordinate v2)
            {
                return new AxialCoordinate(v1.q - v2.q, v1.r - v2.r);
            }

            public static bool operator !=(AxialCoordinate v1, AxialCoordinate v2)
            {
                return v1.q != v2.q || v1.r != v2.r;
            }

            public static bool operator ==(AxialCoordinate v1, AxialCoordinate v2)
            {
                return v1.q == v2.q && v1.r == v2.r;
            }

            public Coordinate toCube()
            {
                return new Coordinate(q, -q - r, r);
            }

            public int getDistanceFrom(AxialCoordinate other)
            {
                return (Mathf.Abs(q - other.q) + Mathf.Abs((-q - r) - (-other.q - other.r)) + Mathf.Abs(r - other.r)) / 2;
            }

            public override bool Equals(object obj)
            {
                return obj.GetHashCode() == this.GetHashCode();
            }

            public override int GetHashCode()
            {
                return AxialCoordinate.GetHashCode(q, r);
            }

            public static int GetHashCode(int q, int r)
            {
                var hash = TezHash.intHash(q);
                hash = TezHash.intHash(hash + r);
                return hash;
            }

            bool IEquatable<AxialCoordinate>.Equals(AxialCoordinate other)
            {
                return this.q == other.q && this.r == other.r;
            }

            public override string ToString()
            {
                return string.Format("{0},{1}", q, r);
            }
        }
#endif

        public class HexMesh
        {
            public List<Vector3> vertices = new List<Vector3>();
            public List<int> indices = new List<int>();
            public List<Vector2> uv = new List<Vector2>();

            public void combine(HexMesh data)
            {
                int rate = vertices.Count > 0 ? vertices.Count / 4 : 0;

                for (int i = 0; i < data.vertices.Count; i++)
                {
                    vertices.Add(data.vertices[i]);
                }

                int add = rate * data.vertices.Count;
                for (int i = 0; i < data.indices.Count; i++)
                {
                    indices.Add(data.indices[i] + add);
                }

                for (int i = 0; i < data.uv.Count; i++)
                {
                    uv.Add(data.uv[i]);
                }
            }
        }

        public enum Layout
        {
            Pointy,
            Flat
        }

        public Layout layout { get; private set; } = Layout.Pointy;
        public float size { get; private set; } = 1;
        public float cellHeight { get; private set; } = 0;
        public float cellWidth { get; private set; } = 0;
        public float vDistance { get; private set; } = 0;
        public float hDistance { get; private set; } = 0;

        bool m_BorderWith = false;

        public TezHexGrid(float size, Layout layout)
        {
            this.init(size, layout);
        }

        private void init(float size, Layout layout)
        {
            this.size = size;
            this.layout = layout;

            switch (layout)
            {
                case Layout.Pointy:
                    cellHeight = size * 2;
                    cellWidth = Sqrt3 / 2 * cellHeight;

                    vDistance = cellHeight * 3 / 4;
                    hDistance = cellWidth;
                    break;
                case Layout.Flat:
                    cellWidth = size * 2;
                    cellHeight = Sqrt3 / 2 * cellWidth;

                    vDistance = cellHeight;
                    hDistance = cellWidth * 3 / 4;
                    break;
            }
        }

        Coordinate round(Vector3 position)
        {
            var rx = Mathf.RoundToInt(position.x);
            var ry = Mathf.RoundToInt(position.y);
            var rz = Mathf.RoundToInt(position.z);

            var x_diff = Mathf.Abs(rx - position.x);
            var y_diff = Mathf.Abs(ry - position.y);
            var z_diff = Mathf.Abs(rz - position.z);

            if (x_diff > y_diff && x_diff > z_diff)
            {
                rx = -ry - rz;
            }
            else if (y_diff > z_diff)
            {
                ry = -rx - rz;
            }
            else
            {
                rz = -rx - ry;
            }

            return new Coordinate(rx, ry, rz);
        }

        Coordinate round(Vector2 position)
        {
            return this.round(new Vector3(position.x, -position.x - position.y, position.y));
        }

        Coordinate cubeToaxial(Coordinate cube)
        {
            return new Coordinate(cube.x, cube.z);
        }

        public Vector3 calculatePosition(Coordinate coordinate)
        {
            return this.calculatePosition(coordinate.x, coordinate.z);
        }

        public Vector3 calculatePosition(int q, int r)
        {
            float x = 0;
            float y = 0;

            switch (layout)
            {
                /*
                 function hex_to_pixel(hex):
                    x = size * sqrt(3) * (hex.q + hex.r/2)
                    y = size * 3/2 * hex.r
                    return Point(x, y)
                 */
                case Layout.Pointy:
                    x = size * Sqrt3 * (q + r / 2.0f);
                    y = size * 3 / 2 * r;
                    break;

                /*
                 function hex_to_pixel(hex):
                    x = size * 3/2 * hex.q
                    y = size * sqrt(3) * (hex.r + hex.q/2)
                    return Point(x, y)
                 */
                case Layout.Flat:
                    x = size * 3 / 2 * q;
                    y = size * Sqrt3 * (r + q / 2.0f);
                    break;
            }

            return new Vector3(x, 0, y);
        }

        public Coordinate calculateAxialCoordinate(Vector2 position)
        {
            float q = 0;
            float r = 0;

            switch (layout)
            {
                /*
                 function pixel_to_hex(x, y):
                    q = (x * sqrt(3)/3 - y / 3) / size
                    r = y * 2/3 / size
                    return hex_round(Hex(q, r)) 
                 */
                case Layout.Pointy:
                    q = (position.x * Sqrt3D3 - position.y / 3) / size;
                    r = position.y * 2 / 3 / size;
                    break;
                /*
                 function pixel_to_hex(x, y):
                    q = x * 2/3 / size
                    r = (-x / 3 + sqrt(3)/3 * y) / size
                    return hex_round(Hex(q, r))
                 */
                case Layout.Flat:
                    q = position.x * 2 / 3 / size;
                    r = (-position.x / 3 + position.y * Sqrt3D3) / size;
                    break;
            }

            return this.round(new Vector2(q, r));
        }

        private Vector3 createCorner(int index, Vector3 corner, float scale = 1.0f)
        {
            float angle_deg = 0;

            switch (layout)
            {
                case Layout.Pointy:
                    angle_deg = 60 * index + 30;
                    break;
                case Layout.Flat:
                    angle_deg = 60 * index;
                    break;
            }

            var angle_rad = Mathf.Deg2Rad * angle_deg;

            return new Vector3(
                corner.x + size * Mathf.Cos(angle_rad),
                corner.y,
                corner.z + size * Mathf.Sin(angle_rad)) * scale;
        }

        public HexMesh createHexMesh(Vector3 center)
        {
            HexMesh mesh = new HexMesh();
            mesh.vertices.Capacity = 7;

            mesh.vertices.Add(center);
            mesh.vertices.Add(this.createCorner(0, center));
            mesh.vertices.Add(this.createCorner(1, center));
            mesh.vertices.Add(this.createCorner(2, center));
            mesh.vertices.Add(this.createCorner(3, center));
            mesh.vertices.Add(this.createCorner(4, center));
            mesh.vertices.Add(this.createCorner(5, center));

            for (int i = 0; i < HexTriangleIndices.Length; i++)
            {
                mesh.indices.Add(HexTriangleIndices[i]);
            }

            return mesh;
        }

        public HexMesh createBorderMesh(Vector3 center, float border_scale = 0.8f)
        {
            HexMesh mesh = new HexMesh();
            mesh.vertices.Capacity = 12;

            for (int i = 0; i < 6; i++)
            {
                mesh.vertices.Add(this.createCorner(i, center));
            }

            for (int i = 0; i < 6; i++)
            {
                mesh.vertices.Add(this.createCorner(i, center, border_scale));
            }

            for (int i = 0; i < BorderTriangleIndices.Length; i++)
            {
                mesh.indices.Add(BorderTriangleIndices[i]);
            }

            return mesh;
        }

        public HexMesh createMesh(List<Vector3> center_list)
        {
            HexMesh mesh = new HexMesh();
            mesh.vertices.Capacity = 7 * center_list.Count;

            for (int i = 0; i < center_list.Count; i++)
            {
                var center = center_list[i];
                mesh.vertices.Add(center);
                mesh.vertices.Add(this.createCorner(0, center));
                mesh.vertices.Add(this.createCorner(1, center));
                mesh.vertices.Add(this.createCorner(2, center));
                mesh.vertices.Add(this.createCorner(3, center));
                mesh.vertices.Add(this.createCorner(4, center));
                mesh.vertices.Add(this.createCorner(5, center));

                var offset = 7 * i;
                for (int j = 0; j < HexTriangleIndices.Length; j++)
                {
                    mesh.indices.Add(HexTriangleIndices[j] + offset);
                }
            }

            return mesh;
        }
    }
}
