using System;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat.Game
{
    public class TezHexGrid
    {
        public static readonly float Sqrt3 = Mathf.Sqrt(3);
        public static readonly float Sqrt3D2 = Mathf.Sqrt(3) / 2;
        public static readonly float Sqrt3D3 = Mathf.Sqrt(3) / 3;

        public struct CubeCoordinate
        {
            public int x;
            public int y;
            public int z;

            bool m_Dirty;

            public CubeCoordinate(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                m_Dirty = true;
            }

            public CubeCoordinate(int q, int r)
            {
                this.x = q;
                this.z = r;
                this.y = -q - r;
                m_Dirty = true;
            }

            public static CubeCoordinate operator +(CubeCoordinate v1, CubeCoordinate v2)
            {
                return new CubeCoordinate(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
            }

            public static CubeCoordinate operator -(CubeCoordinate v1, CubeCoordinate v2)
            {
                return new CubeCoordinate(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
            }

            public static bool operator !=(CubeCoordinate v1, CubeCoordinate v2)
            {
                return v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
            }

            public static bool operator ==(CubeCoordinate v1, CubeCoordinate v2)
            {
                return v1.x == v2.x && v1.y == v2.y && v1.z == v2.z;
            }

            public void set(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                m_Dirty = true;
            }

            public AxialCoordinate toAxial()
            {
                return new AxialCoordinate(x, z);
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

            public int getDistanceFrom(CubeCoordinate other)
            {
                return (Mathf.Abs(x - other.x) + Mathf.Abs(y - other.y) + Mathf.Abs(z - other.z)) / 2;
            }
        }
        public struct AxialCoordinate : IEquatable<AxialCoordinate>
        {
            public static readonly AxialCoordinate zero = new AxialCoordinate(0, 0);
            public static readonly AxialCoordinate one = new AxialCoordinate(1, 1);

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

            public CubeCoordinate toCube()
            {
                return new CubeCoordinate(q, -q - r, r);
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
                unchecked
                {
                    int hash = 47;
                    hash = hash * 227 + q.GetHashCode();
                    hash = hash * 227 + r.GetHashCode();

                    return hash;
                }
            }

            public static int GetHashCode(int q, int r)
            {
                unchecked
                {
                    int hash = 47;
                    hash = hash * 227 + q.GetHashCode();
                    hash = hash * 227 + r.GetHashCode();

                    return hash;
                }
            }

            bool IEquatable<AxialCoordinate>.Equals(AxialCoordinate other)
            {
                return this.q == other.q && this.r == other.r;
            }
        }

        public class HexMesh
        {
            public List<Vector3> vertices = new List<Vector3>();
            public List<int> triangles = new List<int>();
            public List<Vector2> uv = new List<Vector2>();

            public void combine(HexMesh data)
            {
                int rate = vertices.Count > 0 ? vertices.Count / 4 : 0;

                for (int i = 0; i < data.vertices.Count; i++)
                {
                    vertices.Add(data.vertices[i]);
                }

                int add = rate * data.vertices.Count;
                for (int i = 0; i < data.triangles.Count; i++)
                {
                    triangles.Add(data.triangles[i] + add);
                }

                for (int i = 0; i < data.uv.Count; i++)
                {
                    uv.Add(data.uv[i]);
                }
            }
        }

        public static readonly CubeCoordinate[] CubeDirections = new CubeCoordinate[6]
        {
            new CubeCoordinate(1, -1, 0),
            new CubeCoordinate(1, 0, -1),
            new CubeCoordinate(0, 1, -1),
            new CubeCoordinate(-1, 1, 0),
            new CubeCoordinate(-1, 0, 1),
            new CubeCoordinate(0, -1, 1)
        };

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

        public TezHexGrid()
        {

        }

        public TezHexGrid(float size, Layout layout)
        {
            this.init(size, layout);
        }

        public void init(float size, Layout layout)
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

        CubeCoordinate round(Vector3 position)
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

            return new CubeCoordinate(rx, ry, rz);
        }

        CubeCoordinate round(Vector2 position)
        {
            return this.round(new Vector3(position.x, -position.x - position.y, position.y));
        }

        AxialCoordinate cubeToaxial(CubeCoordinate cube)
        {
            return new AxialCoordinate(cube.x, cube.z);
        }

        public Vector3 calculatePosition(CubeCoordinate coordinate)
        {
            return this.calculatePosition(coordinate.x, coordinate.z);
        }

        public Vector3 calculatePosition(AxialCoordinate coordinate)
        {
            return this.calculatePosition(coordinate.q, coordinate.r);
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

        public AxialCoordinate calculateAxialCoordinate(Vector2 position)
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

            return this.round(new Vector2(q, r)).toAxial();
        }

        Vector3 createCorner(int index, Vector3 corner)
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
                corner.z + size * Mathf.Sin(angle_rad));
        }

        public HexMesh createMesh(List<Vector3> corner_list)
        {
            var triangles_index = new List<int>()
            {
                0, 6, 5,
                0, 5, 4,
                0, 4, 3,
                0, 3, 2,
                0, 2, 1,
                0, 1, 6
            };

            HexMesh mesh = new HexMesh();

            for (int i = 0; i < corner_list.Count; i++)
            {
                var corner = corner_list[i];
                mesh.vertices.Add(corner);
                mesh.vertices.Add(this.createCorner(0, corner));
                mesh.vertices.Add(this.createCorner(1, corner));
                mesh.vertices.Add(this.createCorner(2, corner));
                mesh.vertices.Add(this.createCorner(3, corner));
                mesh.vertices.Add(this.createCorner(4, corner));
                mesh.vertices.Add(this.createCorner(5, corner));

                var offset = 7 * i;
                for (int j = 0; j < triangles_index.Count; j++)
                {
                    mesh.triangles.Add(triangles_index[j] + offset);
                }
            }

            return mesh;
        }
    }

}
