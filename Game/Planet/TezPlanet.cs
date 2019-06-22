using UnityEngine;

namespace tezcat.Framework.Game
{
    public class TezPlanet
    {
        public float radius { get; set; }
        public int resolution { get; set; }

        public float roughness { get; set; }
        public float strength { get; set; }
        public Vector3 offset { get; set; }
        public float minLevel { get; set; }

        TezPlanetTerrainNoise m_Noise = new TezPlanetTerrainNoise();
        TezPlanetTerrain[] m_Terrains = new TezPlanetTerrain[6]
        {
            new TezPlanetTerrain(),
            new TezPlanetTerrain(),
            new TezPlanetTerrain(),
            new TezPlanetTerrain(),
            new TezPlanetTerrain(),
            new TezPlanetTerrain()
        };

        public Vector3 calculatePoint(Vector3 point_on_unit_sphere)
        {
            float noise = m_Noise.evaluate(point_on_unit_sphere * roughness + offset);
            noise = Mathf.Max(0, noise - minLevel);
            float elevation = noise * strength;
            return point_on_unit_sphere * radius * (elevation + 1);
        }

        public void createTerrain(int index, Mesh mesh, Vector3 local_up)
        {
            m_Terrains[index].set(mesh, resolution, local_up);
        }

        public void createPlanet()
        {
            foreach (TezPlanetTerrain terrain in m_Terrains)
            {
                terrain.createMesh(this);
            }
        }

        public void createPlanet(float frequency, int octaves, float lacunarity, float persistence)
        {
            m_Noise.m_Frequency = frequency;
            m_Noise.m_Octaves = octaves;
            m_Noise.m_Lacunarity = lacunarity;
            m_Noise.m_Persistence = persistence;

            foreach (TezPlanetTerrain terrain in m_Terrains)
            {
                terrain.createMesh(this);
            }
        }
    }
}