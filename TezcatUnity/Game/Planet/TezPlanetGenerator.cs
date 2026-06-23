using UnityEngine;

namespace tezcat.Framework.Game
{
    public class TezPlanetGenerator
    {
        TezPlanetTerrainFace[] m_Terrains = new TezPlanetTerrainFace[6]
        {
            new TezPlanetTerrainFace(),
            new TezPlanetTerrainFace(),
            new TezPlanetTerrainFace(),
            new TezPlanetTerrainFace(),
            new TezPlanetTerrainFace(),
            new TezPlanetTerrainFace()
        };

        TezPlanetData m_Data = null;

        public void setPlanetData(TezPlanetData data)
        {
            m_Data = data;
        }

        public Vector3 calculatePoint(Vector3 point_on_unit_sphere)
        {
            float noise = m_Data.evaluate(point_on_unit_sphere);
            //             noise = Mathf.Max(0, noise - minLevel);
            //             float elevation = noise * strength;
            return point_on_unit_sphere * m_Data.radius * noise;
        }

        public void createTerrain(int index, Mesh mesh, Vector3 local_up)
        {
            m_Terrains[index].set(mesh, m_Data.resolution, local_up);
        }

        public void createPlanet(TezPlanetData data)
        {
            m_Data = data;
            foreach (TezPlanetTerrainFace terrain in m_Terrains)
            {
                terrain.createMesh(this);
            }
        }

        public void createPlanet()
        {
            foreach (TezPlanetTerrainFace terrain in m_Terrains)
            {
                terrain.createMesh(this);
            }
        }
    }
}