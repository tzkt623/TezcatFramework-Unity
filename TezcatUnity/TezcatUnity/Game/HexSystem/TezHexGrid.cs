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
        /// <summary>
        /// 朝向
        /// 格式为
        /// [零坐标]_[正增长坐标][负增长坐标]
        /// X左上
        /// Z右上
        /// Y中下
        /// </summary>
        public enum Direction : byte
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
        public static readonly float Sqrt3 = Mathf.Sqrt(3);
        public static readonly float Sqrt3D2 = Mathf.Sqrt(3) / 2;
        public static readonly float Sqrt3D3 = Mathf.Sqrt(3) / 3;

        public static readonly TezHexCubeCoordinate[] sDirections = new TezHexCubeCoordinate[6]
        {
            new TezHexCubeCoordinate(1, -1, 0),
            new TezHexCubeCoordinate(1, 0, -1),
            new TezHexCubeCoordinate(0, 1, -1),
            new TezHexCubeCoordinate(-1, 1, 0),
            new TezHexCubeCoordinate(-1, 0, 1),
            new TezHexCubeCoordinate(0, -1, 1)
        };

        public static readonly int[] sHexTriangleIndices = new int[]
        {
            0, 6, 5,
            0, 5, 4,
            0, 4, 3,
            0, 3, 2,
            0, 2, 1,
            0, 1, 6
        };

        public static readonly int[] sBorderTriangleIndices = new int[]
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


        const int cPoolCount = 20;
        static List<TezHexCubeCoordinate>[] sRangePool = new List<TezHexCubeCoordinate>[cPoolCount];
        static List<TezHexCubeCoordinate>[] sRangeWithoutSelfPool = new List<TezHexCubeCoordinate>[cPoolCount];
        static List<TezHexCubeCoordinate>[] sRingPool = new List<TezHexCubeCoordinate>[cPoolCount];

        static TezHexGrid()
        {
            var center = TezHexCubeCoordinate.zero;
            ///缓存RangePool
            for (int i = 0; i < cPoolCount; i++)
            {
                int range = i;
                sRangePool[i] = new List<TezHexCubeCoordinate>(getRangeBlockCount(range));
                sRangePool[i].Add(center);
                getRangeWithStepRing(ref center, ref range, ref sRangePool[i]);
            }

            ///缓存RangeWithoutSelfPool
            for (int i = 0; i < cPoolCount; i++)
            {
                int range = i;
                sRangeWithoutSelfPool[i] = new List<TezHexCubeCoordinate>(getRangeWithoutSelfBlockCount(range));
                getRangeWithStepRing(ref center, ref range, ref sRangeWithoutSelfPool[i]);
            }

            ///缓存RingPool
            for (int i = 0; i < cPoolCount; i++)
            {
                int radius = i;
                sRingPool[i] = new List<TezHexCubeCoordinate>(6 * radius);
                getRing(ref center, ref radius, ref sRingPool[i]);
            }
        }



        public static TezHexCubeCoordinate calculateCoordinate(TezHexCubeCoordinate center, TezHexCubeCoordinate dir)
        {
            return new TezHexCubeCoordinate(center.x + dir.x, center.z + dir.z);
        }

        public static TezHexCubeCoordinate neighbor(TezHexCubeCoordinate center, TezHexCubeCoordinate dir)
        {
            return new TezHexCubeCoordinate(center.x + dir.x, center.z + dir.z);
        }

        public static TezHexCubeCoordinate neighbor(ref TezHexCubeCoordinate center, ref TezHexCubeCoordinate dir)
        {
            return new TezHexCubeCoordinate(center.x + dir.x, center.z + dir.z);
        }

        public static TezHexCubeCoordinate[] neighbors(TezHexCubeCoordinate center)
        {
            TezHexCubeCoordinate[] coordinates = new TezHexCubeCoordinate[6];
            for (int i = 0; i < sDirections.Length; i++)
            {
                coordinates[i] = center + sDirections[i];
            }

            return coordinates;
        }

        public static bool isNeighbor(TezHexCubeCoordinate center, TezHexCubeCoordinate other)
        {
            return center.getDistanceFrom(other) == 1;
        }

        public static TezHexCubeCoordinate getDirection(Direction direction)
        {
            return sDirections[(int)direction];
        }


        /// <summary>
        /// 取得一个范围的块(包含center自己)
        /// </summary>
        public static List<TezHexCubeCoordinate> range(TezHexCubeCoordinate center, int range)
        {
            if (range < cPoolCount)
            {
                var pool = sRangePool[range];
                List<TezHexCubeCoordinate> list = new List<TezHexCubeCoordinate>(pool.Capacity);
                foreach (var item in pool)
                {
                    list.Add(item + center);
                }
                return list;
            }
            else
            {
                List<TezHexCubeCoordinate> list = new List<TezHexCubeCoordinate>(getRangeBlockCount(range));
                list.Add(center);
                getRangeWithStepRing(ref center, ref range, ref list);

                /*
                for (init z = -range; z <= range; z++)
                {
                    for (init y = -range; y <= range; y++)
                    {
                        for (init x = -range; x <= range; x++)
                        {
                            if (x + y + z != 0)
                            {
                                continue;
                            }

                            list.Add(new TezHexCubeCoordinate(center.x + x, center.z + z));
                        }
                    }
                }
                */

                return list;
            }
        }

        /// <summary>
        /// 取得一个范围的块(不包含center自己)
        /// </summary>
        public static List<TezHexCubeCoordinate> rangeWithoutSelf(TezHexCubeCoordinate center, int range)
        {
            if (range < cPoolCount)
            {
                var pool = sRangeWithoutSelfPool[range];
                List<TezHexCubeCoordinate> list = new List<TezHexCubeCoordinate>(pool.Capacity);
                foreach (var item in pool)
                {
                    list.Add(item + center);
                }
                return list;
            }
            else
            {
                List<TezHexCubeCoordinate> list = new List<TezHexCubeCoordinate>(getRangeWithoutSelfBlockCount(range));
                getRangeWithStepRing(ref center, ref range, ref list);

                /*
                for (init range_step = 1; range_step <= range; range_step++)
                {
                    var begin = Directions[(init)Direction.Y_ZX].share();
                    begin.scale(range_step);
                    begin.add(center.x, center.y, center.z);

                    for (init edge_index = 0; edge_index < 6; edge_index++)
                    {
                        for (init j = 0; j < range_step; j++)
                        {
                            list.Add(begin);
                            begin = neighbor(begin, Directions[edge_index]);
                        }
                    }
                }
                */


                /*
                for (init z = -range; z <= range; z++)
                {
                    for (init y = -range; y <= range; y++)
                    {
                        for (init x = -range; x <= range; x++)
                        {
                            if (x + y + z != 0)
                            {
                                continue;
                            }

                            if (x == 0 && y == 0 && z == 0)
                            {
                                continue;
                            }

                            list.Add(new TezHexCubeCoordinate(center.x + x, center.z + z));
                        }
                    }
                }
                */
                return list;
            }
        }



        /// <summary>
        /// 取得一个环
        /// </summary>
        public static List<TezHexCubeCoordinate> ring(TezHexCubeCoordinate center, int radius)
        {
            if (radius < cPoolCount)
            {
                var pool = sRingPool[radius];
                List<TezHexCubeCoordinate> list = new List<TezHexCubeCoordinate>(pool.Capacity);
                foreach (var item in pool)
                {
                    list.Add(item + center);
                }
                return list;
            }
            else
            {
                List<TezHexCubeCoordinate> list = new List<TezHexCubeCoordinate>(6 * radius);
                getRing(ref center, ref radius, ref list);
                return list;
            }
        }

        private static int getRangeBlockCount(int range)
        {
            return (1 + range) * range / 2 * 6 + 1;
        }

        private static int getRangeWithoutSelfBlockCount(int range)
        {
            return (1 + range) * range / 2 * 6;
        }

        private static void getRangeWithStepRing(ref TezHexCubeCoordinate center, ref int range, ref List<TezHexCubeCoordinate> list)
        {
            for (int range_step = 1; range_step <= range; range_step++)
            {
                getRing(ref center, ref range_step, ref list);
                /*
                var begin = Directions[(init)Direction.Y_ZX].share();
                begin.scale(range_step);
                begin.add(center.x, center.y, center.z);

                for (init edge_index = 0; edge_index < 6; edge_index++)
                {
                    for (init j = 0; j < range_step; j++)
                    {
                        list.Add(begin);
                        begin = neighbor(begin, Directions[edge_index]);
                    }
                }
                */
            }
        }

        private static void getRing(ref TezHexCubeCoordinate center, ref int radius, ref List<TezHexCubeCoordinate> list)
        {
            var begin = sDirections[(int)Direction.Y_ZX];
            begin.scale(radius);
            begin.add(center.x, center.y, center.z);

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    list.Add(begin);
                    begin = neighbor(ref begin, ref sDirections[i]);
                }
            }
        }
        #endregion

        #region Grid
        public enum Layout
        {
            Pointy,
            Flat
        }

        public Layout layout => mLayout;
        Layout mLayout = Layout.Pointy;


        public float size => mSize;
        float mSize = 1;

        public float cellHeight => mCellHeight;
        float mCellHeight = 0;

        public float cellWidth => mCellWidth;
        float mCellWidth = 0;

        public float vDistance => mVDistance;
        float mVDistance = 0;

        public float hDistance => mHDistance;
        float mHDistance = 0;


        public TezHexGrid(float size, Layout layout)
        {
            this.init(size, layout);
        }

        private void init(float size, Layout layout)
        {
            mSize = size;
            mLayout = layout;

            switch (layout)
            {
                case Layout.Pointy:
                    mCellHeight = size * 2;
                    mCellWidth = Sqrt3 / 2 * mCellHeight;

                    mVDistance = mCellHeight * 3 / 4;
                    mHDistance = mCellWidth;
                    break;
                case Layout.Flat:
                    mCellWidth = size * 2;
                    mCellHeight = Sqrt3 / 2 * mCellWidth;

                    mVDistance = mCellHeight;
                    mHDistance = mCellWidth * 3 / 4;
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

            switch (mLayout)
            {
                /*
                 function hex_to_pixel(hex):
                    x = size * sqrt(3) * (hex.q + hex.r/2)
                    y = size * 3/2 * hex.r
                    return Point(x, y)
                 */
                case Layout.Pointy:
                    x = mSize * Sqrt3 * (q + r / 2.0f);
                    y = mSize * 3 / 2 * r;
                    break;

                /*
                 function hex_to_pixel(hex):
                    x = size * 3/2 * hex.q
                    y = size * sqrt(3) * (hex.r + hex.q/2)
                    return Point(x, y)
                 */
                case Layout.Flat:
                    x = mSize * 3 / 2 * q;
                    y = mSize * Sqrt3 * (r + q / 2.0f);
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

            switch (mLayout)
            {
                /*
                 function pixel_to_hex(x, y):
                    q = (x * sqrt(3)/3 - y / 3) / size
                    r = y * 2/3 / size
                    return hex_round(Hex(q, r)) 
                 */
                case Layout.Pointy:
                    q = (position.x * Sqrt3D3 - position.y / 3) / mSize;
                    r = position.y * 2 / 3 / mSize;
                    break;
                /*
                 function pixel_to_hex(x, y):
                    q = x * 2/3 / size
                    r = (-x / 3 + sqrt(3)/3 * y) / size
                    return hex_round(Hex(q, r))
                 */
                case Layout.Flat:
                    q = position.x * 2 / 3 / mSize;
                    r = (-position.x / 3 + Sqrt3D3 * position.y) / mSize;
                    break;
            }

            return this.round(new Vector2(q, r));
        }

        private Vector3 createCorner(int index, Vector3 corner, float scale = 1.0f)
        {
            float angle_deg = 0;

            switch (mLayout)
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
                corner.x + mSize * Mathf.Cos(angle_rad),
                corner.y,
                corner.z + mSize * Mathf.Sin(angle_rad)) * scale;
        }
        #endregion

        #region Mesh
        public TezHexMesh createMesh(Vector3 center)
        {
            TezHexMesh mesh = new TezHexMesh();
            mesh.vertices.Capacity = 7;
            mesh.indices.Capacity = sHexTriangleIndices.Length;

            mesh.vertices.Add(center);
            mesh.vertices.Add(this.createCorner(0, center));
            mesh.vertices.Add(this.createCorner(1, center));
            mesh.vertices.Add(this.createCorner(2, center));
            mesh.vertices.Add(this.createCorner(3, center));
            mesh.vertices.Add(this.createCorner(4, center));
            mesh.vertices.Add(this.createCorner(5, center));

            for (int i = 0; i < sHexTriangleIndices.Length; i++)
            {
                mesh.indices.Add(sHexTriangleIndices[i]);
            }

            return mesh;
        }

        public TezHexMesh createMesh(List<Vector3> centerList)
        {
            TezHexMesh mesh = new TezHexMesh();
            mesh.vertices.Capacity = 7 * centerList.Count;
            mesh.indices.Capacity = centerList.Count * sHexTriangleIndices.Length;

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
                for (int j = 0; j < sHexTriangleIndices.Length; j++)
                {
                    mesh.indices.Add(sHexTriangleIndices[j] + offset);
                }
            }

            return mesh;
        }
        public TezHexMesh createBorderMesh(Vector3 center, float borderScale = 0.8f)
        {
            TezHexMesh mesh = new TezHexMesh();
            mesh.vertices.Capacity = 12;
            mesh.indices.Capacity = sBorderTriangleIndices.Length;

            for (int i = 0; i < 6; i++)
            {
                mesh.vertices.Add(this.createCorner(i, center));
            }

            for (int i = 0; i < 6; i++)
            {
                mesh.vertices.Add(this.createCorner(i, center, borderScale));
            }

            for (int i = 0; i < sBorderTriangleIndices.Length; i++)
            {
                mesh.indices.Add(sBorderTriangleIndices[i]);
            }

            return mesh;
        }

        public TezHexMesh createBorderMesh(List<Vector3> centerList, float borderScale = 0.8f)
        {
            TezHexMesh mesh = new TezHexMesh();
            mesh.vertices.Capacity = centerList.Count * 12;
            mesh.indices.Capacity = centerList.Count * sBorderTriangleIndices.Length;

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
                for (int i = 0; i < sBorderTriangleIndices.Length; i++)
                {
                    mesh.indices.Add(sBorderTriangleIndices[i] + offset);
                }
            }

            return mesh;
        }

        #endregion
    }
}
