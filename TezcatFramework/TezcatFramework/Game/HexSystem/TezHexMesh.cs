using System.Collections.Generic;
using UnityEngine;

namespace tezcat.Framework.Game
{
    public class TezHexMesh
    {
        public List<Vector3> vertices = new List<Vector3>();
        public List<int> indices = new List<int>();
        public List<Vector2> uv = new List<Vector2>();

        public void combine(TezHexMesh data)
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

}
