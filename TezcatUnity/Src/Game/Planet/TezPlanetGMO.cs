using UnityEngine;

namespace tezcat.Framework.Game
{
    public class TezPlanetGMO : MonoBehaviour
    {
        public Color color = Color.white;

        protected MeshFilter[] m_MeshFilters;
        protected GameObject[] m_Surface = new GameObject[6];

        protected TezPlanetGenerator m_Generator = new TezPlanetGenerator();
        public virtual TezPlanetData planetData { get; }

        private void Start()
        {
            if (m_Generator == null)
            {
                m_Generator = new TezPlanetGenerator();
            }

            this.generate();
            m_Generator.createPlanet(planetData);
            this.createColor();
        }

        protected void generate()
        {
            if (m_MeshFilters == null || m_MeshFilters.Length == 0)
            {
                m_MeshFilters = new MeshFilter[6];
            }

            Vector3[] directions =
            {
                Vector3.up, Vector3.down,
                Vector3.left, Vector3.right,
                Vector3.forward, Vector3.back
            };

            for (int i = 0; i < 6; i++)
            {
                if (m_MeshFilters[i] == null)
                {
                    GameObject surface = new GameObject("MeshChunk");
                    surface.transform.parent = this.transform;
                    surface.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                    var filter = surface.AddComponent<MeshFilter>();
                    filter.sharedMesh = new Mesh();

                    m_MeshFilters[i] = filter;
                    m_Surface[i] = surface;
                }
                m_Generator.createTerrain(i, m_MeshFilters[i].sharedMesh, directions[i]);
            }
        }

        protected void createColor()
        {
            foreach (MeshFilter filter in m_MeshFilters)
            {
                filter.GetComponent<MeshRenderer>().sharedMaterial.color = color;
            }
        }
    }
}