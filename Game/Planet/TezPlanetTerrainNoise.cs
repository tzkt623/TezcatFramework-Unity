using tezcat.Framework.Utility;
using UnityEngine;

namespace tezcat.Framework.Game
{
    public class TezPlanetTerrainNoise
    {
        public float m_Frequency = 8;
        public int m_Octaves = 4;
        public float m_Lacunarity = 1.2f;
        public float m_Persistence = 0.5f;


        public float evaluate(Vector3 point)
        {
            return (TezNoise.sum(TezNoise.Function.Perlin3D
                , point
                , m_Frequency
                , m_Octaves
                , m_Lacunarity
                , m_Persistence) + 1) * 0.5f;
        }
    }
}