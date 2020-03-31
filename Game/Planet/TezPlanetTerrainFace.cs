using UnityEngine;

namespace tezcat.Framework.Game
{
    class TezPlanetTerrainFace
    {
        Mesh m_Mesh;

        int m_Resolution;
        Vector3 m_LocalUp;
        Vector3 m_AxisA;
        Vector3 m_AxisB;

        public TezPlanetTerrainFace() { }

        public TezPlanetTerrainFace(Mesh mesh, int resolution, Vector3 local_up)
        {
            m_Mesh = mesh;
            m_Resolution = resolution;
            m_LocalUp = local_up;

            m_AxisA = new Vector3(local_up.y, local_up.z, local_up.x);
            m_AxisB = Vector3.Cross(m_LocalUp, m_AxisA);
        }

        public void set(Mesh mesh, int resolution, Vector3 local_up)
        {
            m_Mesh = mesh;
            m_Resolution = resolution;
            m_LocalUp = local_up;

            m_AxisA = new Vector3(local_up.y, local_up.z, local_up.x);
            m_AxisB = Vector3.Cross(m_LocalUp, m_AxisA);
        }

        public void createMesh(TezPlanetGenerator generator)
        {
            ///=================
            /// *-*-*-*
            /// |\|\|\|
            /// *-*-*-*
            /// |\|\|\|
            /// *-*-*-*
            /// |\|\|\|
            /// *-*-*-*
            /// 
            /// Resolution = 4
            /// SmallRect = (4-1)^2 = 9
            /// Triangle = SmallRect * 2 = 18
            /// Index = Triangle * 3 = 36
            /// =================
            /// 0-1-2-3
            /// *-*-*-*
            /// |\|\|\|
            /// *-*-*-*
            /// 4-5-6-7
            /// 
            /// Index1 = 0,5,4 = i,i+Resolution + 1,i+Resolution
            /// Index2 = 0,1,5 = i,i+1,i+Resolution + 1
            /// 注意最后一排/列的越界处理
            /// =================

            Vector3[] vectices = new Vector3[m_Resolution * m_Resolution];
            int[] triangles = new int[(m_Resolution - 1) * (m_Resolution - 1) * 2 * 3];

            int index = 0;

            for (int y = 0; y < m_Resolution; y++)
            {
                for (int x = 0; x < m_Resolution; x++)
                {
                    int i = x + y * m_Resolution;
                    Vector2 percent = new Vector2(x, y) / (m_Resolution - 1);
                    Vector3 pointOnUnitCube = m_LocalUp + (percent.x - 0.5f) * 2 * m_AxisA + (percent.y - 0.5f) * 2 * m_AxisB;
                    Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                    vectices[i] = generator.calculatePoint(pointOnUnitSphere);

                    if (x != m_Resolution - 1 && y != m_Resolution - 1)
                    {
                        triangles[index] = i;
                        triangles[index + 1] = i + m_Resolution + 1;
                        triangles[index + 2] = i + m_Resolution;

                        triangles[index + 3] = i;
                        triangles[index + 4] = i + 1;
                        triangles[index + 5] = i + m_Resolution + 1;
                        index += 6;
                    }
                }
            }

            m_Mesh.Clear();
            m_Mesh.vertices = vectices;
            m_Mesh.triangles = triangles;
            m_Mesh.RecalculateNormals();
        }
    }
}
