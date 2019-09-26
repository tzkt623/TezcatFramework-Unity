using tezcat.Framework.Core;
using UnityEngine;

namespace tezcat.Framework.Game
{
    public class TezPlanetGMO : TezGameMonoObject
    {
        [Header("Config")]
        [Range(2, 256)]
        public int resolution = 10;
        [Range(1, 1000)]
        public float radius = 1;
        public Color color = Color.white;

        [Header("Noise")]
        public float frequency = 8;
        [Range(1, 8)]
        public int octaves = 4;
        public float lacunarity = 1.2f;
        public float persistence = 0.8f;
        public Vector3 offset = Vector3.zero;
        public float roughness = 0;
        public float strength = 0;
        public float minLevel = 0;

        protected MeshFilter[] m_MeshFilters;
        protected GameObject[] m_Surface = new GameObject[6];

        TezPlanet m_Planet = new TezPlanet();

        protected override void initObject()
        {
            base.initObject();

            if (m_Planet == null)
            {
                m_Planet = new TezPlanet();
            }

            this.generate();
            m_Planet.createPlanet(frequency, octaves, lacunarity, persistence);
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

            m_Planet.radius = radius;
            m_Planet.resolution = resolution;
            m_Planet.offset = offset;
            m_Planet.roughness = roughness;
            m_Planet.strength = strength;
            m_Planet.minLevel = minLevel;

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
                m_Planet.createTerrain(i, m_MeshFilters[i].sharedMesh, directions[i]);
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