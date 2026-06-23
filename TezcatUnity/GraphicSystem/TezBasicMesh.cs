using UnityEngine;
using System.Collections;

namespace tezcat.Unity.GraphicSystem
{
    public class TezBasicMesh
    {
        public Vector3[] vertices;
        public int[] triangles;
        public Vector2[] uv;
        public Vector2[] uv2;
        public Vector3[] normals;
        public Vector3[] tangents;
        public Color32[] colors;


        public TezBasicMesh rotate(Quaternion rotation)
        {
            if (this.vertices != null)
            {
                for (int i = 0; i < this.vertices.Length; i++)
                {
                    this.vertices[i] = rotation * this.vertices[i];
                }
            }
            if (this.normals != null)
            {
                for (int j = 0; j < this.normals.Length; j++)
                {
                    this.normals[j] = rotation * this.normals[j];
                }
            }
            if (this.tangents != null)
            {
                for (int k = 0; k < this.tangents.Length; k++)
                {
                    this.tangents[k] = rotation * this.tangents[k];
                }
            }
            return this;
        }

        public TezBasicMesh translate(Vector3 translation)
        {
            if (this.vertices != null)
            {
                for (int i = 0; i < this.vertices.Length; i++)
                {
                    this.vertices[i] += translation;
                }
            }
            return this;
        }

        public TezBasicMesh scale(float scale)
        {
            if (this.vertices != null)
            {
                for (int i = 0; i < this.vertices.Length; i++)
                {
                    this.vertices[i] *= scale;
                }
            }
            return this;
        }
    }
}