using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public struct Coordinate3I
    {
        public int x, y, z;

        public Coordinate3I(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public bool positionNotEqual(Position3I other)
        {
            return x != other.x || y != other.y || z != other.z;
        }

        public static Coordinate3I operator +(Coordinate3I v1, Coordinate3I v2)
        {
            return new Coordinate3I(v1.x + v2.x, v1.y + v2.y, v1.z + v2.z);
        }

        public static Coordinate3I operator -(Coordinate3I v1, Coordinate3I v2)
        {
            return new Coordinate3I(v1.x - v2.x, v1.y - v2.y, v1.z - v2.z);
        }

        public static bool operator != (Coordinate3I v1, Coordinate3I v2)
        {
            return v1.x != v2.x || v1.y != v2.y || v1.z != v2.z;
        }

        public static bool operator ==(Coordinate3I v1, Coordinate3I v2)
        {
            return !(v1.x != v2.x || v1.y != v2.y || v1.z != v2.z);
        }

        public override bool Equals(object obj)
        {
            return (Coordinate3I)obj == this;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public string toString()
        {
            return "[" + x + "," + y + "," + z + "]";
        }
    }


    public static class HexInfo
    {
        //  z   0   1
        //
        //y   5   x   2
        //
        //  x   4   3

        // offset
        //           z
        //         (-1,1,0)   (0,1,-1)
        //
        // y  (-1,0,1)   (0,0,0)   (1,0,-1)
        //
        //         (0,-1,1)   (1,-1,0)
        //           x


        //0 0 000000 
        //  E 543210
        public enum NeighbourFlag
        {
            N_Empty = 0,
            N_0 = 1,
            N_1 = 2,
            N_2 = 4,
            N_3 = 8,
            N_4 = 16,
            N_5 = 32,
            N_All = 63,
            N_Edge = 64
        }

        //1 : x = 1
        //3 : x = -1
        //4 : y = 1
        //12 : y = -1
        //16 : z = 1
        //48 : z = -1

        public enum RelativePositionXYZ
        {
            X_POS_ONE = 1,  //1
            X_NEG_ONE = 3,  //-1
            Y_POS_ONE = 4,  //1
            Y_NEG_ONE = 12, //-1
            Z_POS_ONE = 16, //1
            Z_NEG_ONE = 48, //-1
        }

        public enum RelativePosition
        {
            RP_E = -1,
            RP_0 = RelativePositionXYZ.X_NEG_ONE + RelativePositionXYZ.Y_POS_ONE,   //-1 1 0 => 3 + 4 + 0
            RP_1 = RelativePositionXYZ.Y_POS_ONE + RelativePositionXYZ.Z_NEG_ONE,  //0 1 -1 => 0 + 4 + 48
            RP_2 = RelativePositionXYZ.X_POS_ONE + RelativePositionXYZ.Z_NEG_ONE,  //1 0 -1 => 1 + 0 + 48
            RP_3 = RelativePositionXYZ.X_POS_ONE + RelativePositionXYZ.Y_NEG_ONE,  //1 -1 0 => 1 + 12 + 0
            RP_4 = RelativePositionXYZ.Y_NEG_ONE + RelativePositionXYZ.Z_POS_ONE,  //0 -1 1 => 0 + 12 + 16
            RP_5 = RelativePositionXYZ.X_NEG_ONE + RelativePositionXYZ.Z_POS_ONE,  //-1 0 1 => 3 + 0 + 16
        }

        public static readonly int RelativeValue = 3;

        public static readonly List<Position3I> offset = new List<Position3I>
        {
            new Position3I(-1, 1, 0), new Position3I(0, 1, -1),
            new Position3I(1, 0, -1), new Position3I(1, -1, 0),
            new Position3I(0, -1, 1), new Position3I(-1, 0, 1)
        };

        //         *
        //      *  *
        //   *     *
        //*--------*
        //    ^
        //    |

        public static readonly float sqrt3 = Mathf.Sqrt(3.0f);
        /// <summary>
        /// 特定参数 0.866025404f;
        /// </summary>
        public static readonly float sqrt3D2 = Mathf.Sqrt(3.0f) / 2;
        public static readonly float sqrt3D3 = Mathf.Sqrt(3.0f) / 3;
        /// <summary>
        /// 外接圆半径 与内切圆半径比例为 1 : Mathf.Sqrt(3.0f) / 2
        /// </summary>
        public static readonly float outerRadiusRatio = 1f;
        /// <summary>
        /// 内切圆半径 与外接圆半径比例为 Mathf.Sqrt(3.0f) / 2 : 1
        /// </summary>
        public static readonly float innerRadiusRatio = outerRadiusRatio * sqrt3D2;
        /// <summary>
        /// 立方体六边形高度
        /// </summary>
        public static float height = 1f;

        public static List<Vector3> getVertex3D(float radius, float height)
        {
            float minRadius = radius * sqrt3D2;

            return new List<Vector3>
            {
                new Vector3(0f, 0f, height * 0.5f),                         //x
                new Vector3(0f, radius, height * 0.5f),                     //0
                new Vector3(minRadius, 0.5f * radius, height * 0.5f),       //1
                new Vector3(minRadius, -0.5f * radius, height * 0.5f),      //2
                new Vector3(0f, -radius, height * 0.5f),                    //3
                new Vector3(-minRadius, -0.5f * radius, height * 0.5f),     //4
                new Vector3(-minRadius, 0.5f * radius, height * 0.5f),      //5

                new Vector3(0f, 0f, -height * 0.5f),                        //-x
                new Vector3(0f, radius, -height * 0.5f),                    //8
                new Vector3(minRadius, 0.5f * radius, -height * 0.5f),      //9
                new Vector3(minRadius, -0.5f * radius, -height * 0.5f),     //10
                new Vector3(0f, -radius, -height * 0.5f),                   //11
                new Vector3(-minRadius, -0.5f * radius, -height * 0.5f),    //12
                new Vector3(-minRadius, 0.5f * radius, -height * 0.5f)      //13
            };
        }

        public static List<Vector3> vertex3D = new List<Vector3>
        {
            new Vector3(0f, 0f, height * 0.5f),                                //x
            new Vector3(0f, outerRadiusRatio, height * 0.5f),                       //0
            new Vector3(innerRadiusRatio, 0.5f * outerRadiusRatio, height * 0.5f),       //1
            new Vector3(innerRadiusRatio, -0.5f * outerRadiusRatio, height * 0.5f),      //2
            new Vector3(0f, -outerRadiusRatio, height * 0.5f),                      //3
            new Vector3(-innerRadiusRatio, -0.5f * outerRadiusRatio, height * 0.5f),     //4
            new Vector3(-innerRadiusRatio, 0.5f * outerRadiusRatio, height * 0.5f),      //5

            new Vector3(0f, 0f, -height * 0.5f),                                //-x
            new Vector3(0f, outerRadiusRatio, -height * 0.5f),                       //8
            new Vector3(innerRadiusRatio, 0.5f * outerRadiusRatio, -height * 0.5f),       //9
            new Vector3(innerRadiusRatio, -0.5f * outerRadiusRatio, -height * 0.5f),      //10
            new Vector3(0f, -outerRadiusRatio, -height * 0.5f),                      //11
            new Vector3(-innerRadiusRatio, -0.5f * outerRadiusRatio, -height * 0.5f),     //12
            new Vector3(-innerRadiusRatio, 0.5f * outerRadiusRatio, -height * 0.5f)       //13
        };

        public static List<int> index3D = new List<int>
        {
            0, 2, 1,
            0, 3, 2,
            0, 4, 3,
            0, 5, 4,
            0, 6, 5,
            0, 1, 6,

            7, 8, 9,
            7, 9, 10,
            7, 10, 11,
            7, 11, 12,
            7, 12, 13,
            7, 13, 8,

            1, 2, 8,
            2, 9, 8,

            2, 3, 9,
            3, 10, 9,

            3, 4, 10,
            4, 11, 10,

            4, 5, 11,
            5, 12, 11,

            5, 6, 12,
            6, 13, 12,

            6, 1, 13,
            1, 8, 13
        };
    }
}

