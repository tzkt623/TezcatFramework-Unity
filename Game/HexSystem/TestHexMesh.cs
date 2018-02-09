using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace tezcat
{
    public class TestHexMesh : MonoBehaviour
    {
        MeshRenderer m_MR;
        MeshFilter m_MF;
        MeshCollider m_MC;

        HexGrid m_HexGrid = new HexGrid();

        void Start()
        {
            m_MR = this.GetComponent<MeshRenderer>();
            m_MF = this.GetComponent<MeshFilter>();
            m_MC = this.GetComponent<MeshCollider>();


        }

        private void onCellCreated(HexCell obj)
        {
            var data = obj.getHexMesh();

            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            for (int i = 0; i < m_MF.mesh.vertices.Length; i++)
            {
                vertices.Add(m_MF.mesh.vertices[i]);
            }

            for (int i = 0; i < data.vertices.Count; i++)
            {
                vertices.Add(data.vertices[i]);
            }

            for (int i = 0; i < m_MF.mesh.triangles.Length; i++)
            {
                triangles.Add(m_MF.mesh.triangles[i]);
            }

            var add = m_MF.mesh.triangles.Length / 18 * 7;
            for (int i = 0; i < data.triangles.Count; i++)
            {
                triangles.Add(data.triangles[i] + add);
            }

            m_MF.mesh.Clear();
            m_MF.mesh.vertices = vertices.ToArray();
            m_MF.mesh.triangles = triangles.ToArray();
            m_MF.mesh.RecalculateNormals();

            m_MR.material = new Material(Shader.Find("Standard"));

            Mesh mesh = new Mesh();
            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles.ToArray();
            m_MC.sharedMesh = mesh;
        }

        private void Update()
        {
            if(Input.GetMouseButtonUp(0))
            {
                RaycastHit hit;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
//                    Debug.Log(hit.point);
                }

            }


        }
    }
}

