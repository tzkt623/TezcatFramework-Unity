using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace tezcat.Framework.Game
{
    class TezPlanetTerrain
    {
        Mesh m_Mesh;

        int m_Resolution;
        Vector3 m_LocalUp;
        Vector3 m_AxisA;
        Vector3 m_AxisB;

        public TezPlanetTerrain() { }

        public TezPlanetTerrain(Mesh mesh, int resolution, Vector3 local_up)
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

        public void createMesh(TezPlanet planet)
        {
            Vector3[] vectices = new Vector3[m_Resolution * m_Resolution];
            int[] triangles = new int[(m_Resolution - 1) * (m_Resolution - 1) * 6];

            int index = 0;

            for (int y = 0; y < m_Resolution; y++)
            {
                for (int x = 0; x < m_Resolution; x++)
                {
                    int i = x + y * m_Resolution;
                    Vector2 percent = new Vector2(x, y) / (m_Resolution - 1);
                    Vector3 pointOnUnitCube = m_LocalUp + (percent.x - 0.5f) * 2 * m_AxisA + (percent.y - 0.5f) * 2 * m_AxisB;
                    Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;
                    vectices[i] = planet.calculatePoint(pointOnUnitSphere);

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
