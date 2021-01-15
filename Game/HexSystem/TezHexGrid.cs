using System.Collections.Generic;
using UnityEngine;

namespace tezcat.Framework.Game
{
    public class TezHexGrid
    {
        #region Tool
        public static readonly float Sqrt3 = Mathf.Sqrt(3);
        public static readonly float Sqrt3D2 = Mathf.Sqrt(3) / 2;
        public static readonly float Sqrt3D3 = Mathf.Sqrt(3) / 3;

        /// <summary>
        /// 朝向
        /// 格式为
        /// [零坐标]_[正增长坐标][负增长坐标]
        /// X左上
        /// Z右上
        /// Y中下
        /// </summary>
        public enum Direction : int
        {
            /// <summary>
            /// Z[0]_X[+]Y[-]
            /// </summary>
            Z_XY = 0,

            /// <summary>
            /// Y[0]_X[+]Z[-]
            /// </summary>
            Y_XZ,

            /// <summary>
            /// X[0]_Y[+]Z[-]
            /// </summary>
            X_YZ,

            /// <summary>
            /// Z[0]_Y[+]X[-]
            /// </summary>
            Z_YX,

            /// <summary>
            /// Y[0]_Z[+]X[-]
            /// </summary>
            Y_ZX,

            /// <summary>
            /// X[0]_Z[+]Y[-]
            /// </summary>
            X_ZY
        }

        /*
         * 坐标格式
         * 
         *  x   z
         *
         *    y
         */
        public static readonly TezHexCubeCoordinate[] Directions = new TezHexCubeCoordinate[6]
        {
            new TezHexCubeCoordinate(1, -1, 0),
            new TezHexCubeCoordinate(1, 0, -1),
            new TezHexCubeCoordinate(0, 1, -1),
            new TezHexCubeCoordinate(-1, 1, 0),
            new TezHexCubeCoordinate(-1, 0, 1),
            new TezHexCubeCoordinate(0, -1, 1)
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

        public static TezHexCubeCoordinate calculateCoordinate(TezHexCubeCoordinate center, TezHexCubeCoordinate dir)
        {
            return new TezHexCubeCoordinate(center.x + dir.x, center.z + dir.z);
        }

        public static TezHexCubeCoordinate neighbor(TezHexCubeCoordinate center, TezHexCubeCoordinate dir)
        {
            return new TezHexCubeCoordinate(center.x + dir.x, center.z + dir.z);
        }

        public static TezHexCubeCoordinate[] neighbors(TezHexCubeCoordinate center)
        {
            TezHexCubeCoordinate[] coordinates = new TezHexCubeCoordinate[6];
            for (int i = 0; i < Directions.Length; i++)
            {
                coordinates[i] = center + Directions[i];
            }

            return coordinates;
        }

        public static bool isNeighbor(TezHexCubeCoordinate center, TezHexCubeCoordinate other)
        {
            return center.getDistanceFrom(other) == 1;
        }

        public static TezHexCubeCoordinate getDirection(Direction direction)
        {
            return Directions[(int)direction];
        }

        /// <summary>
        /// 取得一个范围的块(包含center自己)
        /// </summary>
        public static List<TezHexCubeCoordinate> range(TezHexCubeCoordinate center, int range)
        {
            List<TezHexCubeCoordinate> list = new List<TezHexCubeCoordinate>();
            list.Capacity = (1 + range) * range / 2 * 6 + 1;

            for (int z = -range; z <= range; z++)
            {
                for (int y = -range; y <= range; y++)
                {
                    for (int x = -range; x <= range; x++)
                    {
                        if (x + y + z != 0)
                        {
                            continue;
                        }

                        list.Add(new TezHexCubeCoordinate(center.x + x, center.z + z));
                    }
                }
            }

            return list;
        }

        public static List<TezHexCubeCoordinate> ring(TezHexCubeCoordinate center, int radius)
        {
            List<TezHexCubeCoordinate> list = new List<TezHexCubeCoordinate>();
            list.Capacity = 6 * radius;

            var begin = Directions[(int)Direction.Y_ZX];
            begin.scale(radius);
            begin.add(center.x, center.y, center.z);

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    list.Add(begin);
                    begin = neighbor(begin, Directions[i]);
                }
            }

            return list;
        }
        #endregion

        #region Grid
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

        TezHexCubeCoordinate round(Vector3 position)
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

            return new TezHexCubeCoordinate(rx, ry, rz);
        }

        TezHexCubeCoordinate round(Vector2 position)
        {
            return this.round(new Vector3(position.x, -position.x - position.y, position.y));
        }

        TezHexCubeCoordinate cubeToAxial(TezHexCubeCoordinate cube)
        {
            return new TezHexCubeCoordinate(cube.x, cube.z);
        }

        public Vector3 calculatePosition(TezHexCubeCoordinate coordinate)
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

        public TezHexCubeCoordinate calculateCoordinate(Vector2 position)
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
                    r = (-position.x / 3 + Sqrt3D3 * position.y) / size;
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
        #endregion

        #region Mesh

        public class Mesh
        {
            public List<Vector3> vertices = new List<Vector3>();
            public List<int> indices = new List<int>();
            public List<Vector2> uv = new List<Vector2>();

            public void combine(Mesh data)
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

        public Mesh createHexMesh(Vector3 center)
        {
            Mesh mesh = new Mesh();
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

        public Mesh createBorderMesh(Vector3 center, float border_scale = 0.8f)
        {
            Mesh mesh = new Mesh();
            mesh.vertices.Capacity = 12;
            mesh.indices.Capacity = BorderTriangleIndices.Length;

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

        public Mesh createBorderMesh(List<Vector3> center_list, float border_scale = 0.8f)
        {
            Mesh mesh = new Mesh();
            mesh.vertices.Capacity = center_list.Count * 12;
            mesh.indices.Capacity = center_list.Count * BorderTriangleIndices.Length;

            for (int center_index = 0; center_index < center_list.Count; center_index++)
            {
                var center = center_list[center_index];
                for (int i = 0; i < 6; i++)
                {
                    mesh.vertices.Add(this.createCorner(i, Vector3.zero) + center);
                }

                for (int i = 0; i < 6; i++)
                {
                    ///将所有点移动到0点进行计算
                    ///可以方便进行Vector3的Scale运算
                    ///然后再把计算好的点移动到指定的center上
                    ///否则很难在指定的center上进行Vector3的Scale计算
                    mesh.vertices.Add(this.createCorner(i, Vector3.zero, border_scale) + center);
                }

                var offset = 12 * center_index;
                for (int i = 0; i < BorderTriangleIndices.Length; i++)
                {
                    mesh.indices.Add(BorderTriangleIndices[i] + offset);
                }
            }

            return mesh;
        }

        public Mesh createMesh(List<Vector3> center_list)
        {
            Mesh mesh = new Mesh();
            mesh.vertices.Capacity = 7 * center_list.Count;
            mesh.indices.Capacity = center_list.Count * HexTriangleIndices.Length;

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
        #endregion
    }
}
