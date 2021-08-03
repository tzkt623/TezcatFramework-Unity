using System.Collections.Generic;
using UnityEngine;

namespace tezcat.Framework.Game
{
    /// <summary>
    /// Hex位置计算器
    /// </summary>
    public class TezHexGrid
    {
        #region Config
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

        public Layout layout => m_Layout;
        Layout m_Layout = Layout.Pointy;


        public float size => m_Size;
        float m_Size = 1;

        public float cellHeight => m_CellHeight;
        float m_CellHeight = 0;

        public float cellWidth => m_CellWidth;
        float m_CellWidth = 0;

        public float vDistance => m_VDistance;
        float m_VDistance = 0;

        public float hDistance => m_HDistance;
        float m_HDistance = 0;


        public TezHexGrid(float size, Layout layout)
        {
            this.init(size, layout);
        }

        private void init(float size, Layout layout)
        {
            this.m_Size = size;
            this.m_Layout = layout;

            switch (layout)
            {
                case Layout.Pointy:
                    m_CellHeight = size * 2;
                    m_CellWidth = Sqrt3 / 2 * m_CellHeight;

                    m_VDistance = m_CellHeight * 3 / 4;
                    m_HDistance = m_CellWidth;
                    break;
                case Layout.Flat:
                    m_CellWidth = size * 2;
                    m_CellHeight = Sqrt3 / 2 * m_CellWidth;

                    m_VDistance = m_CellHeight;
                    m_HDistance = m_CellWidth * 3 / 4;
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

            switch (m_Layout)
            {
                /*
                 function hex_to_pixel(hex):
                    x = size * sqrt(3) * (hex.q + hex.r/2)
                    y = size * 3/2 * hex.r
                    return Point(x, y)
                 */
                case Layout.Pointy:
                    x = m_Size * Sqrt3 * (q + r / 2.0f);
                    y = m_Size * 3 / 2 * r;
                    break;

                /*
                 function hex_to_pixel(hex):
                    x = size * 3/2 * hex.q
                    y = size * sqrt(3) * (hex.r + hex.q/2)
                    return Point(x, y)
                 */
                case Layout.Flat:
                    x = m_Size * 3 / 2 * q;
                    y = m_Size * Sqrt3 * (r + q / 2.0f);
                    break;
            }

            return new Vector3(x, 0, y);
        }

        /// <summary>
        /// 注意X,Y,Z轴的取舍转换
        /// 
        /// 在3D坐标下需要的是Hex平面的平铺坐标
        /// 垂直的高度坐标是不需要的
        /// </summary>
        public TezHexCubeCoordinate calculateCoordinate(Vector2 position)
        {
            float q = 0;
            float r = 0;

            switch (m_Layout)
            {
                /*
                 function pixel_to_hex(x, y):
                    q = (x * sqrt(3)/3 - y / 3) / size
                    r = y * 2/3 / size
                    return hex_round(Hex(q, r)) 
                 */
                case Layout.Pointy:
                    q = (position.x * Sqrt3D3 - position.y / 3) / m_Size;
                    r = position.y * 2 / 3 / m_Size;
                    break;
                /*
                 function pixel_to_hex(x, y):
                    q = x * 2/3 / size
                    r = (-x / 3 + sqrt(3)/3 * y) / size
                    return hex_round(Hex(q, r))
                 */
                case Layout.Flat:
                    q = position.x * 2 / 3 / m_Size;
                    r = (-position.x / 3 + Sqrt3D3 * position.y) / m_Size;
                    break;
            }

            return this.round(new Vector2(q, r));
        }

        private Vector3 createCorner(int index, Vector3 corner, float scale = 1.0f)
        {
            float angle_deg = 0;

            switch (m_Layout)
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
                corner.x + m_Size * Mathf.Cos(angle_rad),
                corner.y,
                corner.z + m_Size * Mathf.Sin(angle_rad)) * scale;
        }
        #endregion

        #region Mesh
        public TezHexMesh createMesh(Vector3 center)
        {
            TezHexMesh mesh = new TezHexMesh();
            mesh.vertices.Capacity = 7;
            mesh.indices.Capacity = HexTriangleIndices.Length;

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

        public TezHexMesh createMesh(List<Vector3> centerList)
        {
            TezHexMesh mesh = new TezHexMesh();
            mesh.vertices.Capacity = 7 * centerList.Count;
            mesh.indices.Capacity = centerList.Count * HexTriangleIndices.Length;

            for (int i = 0; i < centerList.Count; i++)
            {
                var center = centerList[i];
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
        public TezHexMesh createBorderMesh(Vector3 center, float borderScale = 0.8f)
        {
            TezHexMesh mesh = new TezHexMesh();
            mesh.vertices.Capacity = 12;
            mesh.indices.Capacity = BorderTriangleIndices.Length;

            for (int i = 0; i < 6; i++)
            {
                mesh.vertices.Add(this.createCorner(i, center));
            }

            for (int i = 0; i < 6; i++)
            {
                mesh.vertices.Add(this.createCorner(i, center, borderScale));
            }

            for (int i = 0; i < BorderTriangleIndices.Length; i++)
            {
                mesh.indices.Add(BorderTriangleIndices[i]);
            }

            return mesh;
        }

        public TezHexMesh createBorderMesh(List<Vector3> centerList, float borderScale = 0.8f)
        {
            TezHexMesh mesh = new TezHexMesh();
            mesh.vertices.Capacity = centerList.Count * 12;
            mesh.indices.Capacity = centerList.Count * BorderTriangleIndices.Length;

            for (int center_index = 0; center_index < centerList.Count; center_index++)
            {
                var center = centerList[center_index];
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
                    mesh.vertices.Add(this.createCorner(i, Vector3.zero, borderScale) + center);
                }

                var offset = 12 * center_index;
                for (int i = 0; i < BorderTriangleIndices.Length; i++)
                {
                    mesh.indices.Add(BorderTriangleIndices[i] + offset);
                }
            }

            return mesh;
        }

        #endregion
    }
}
