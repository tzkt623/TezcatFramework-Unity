using tezcat.Framework.Utility;
using UnityEngine;

namespace tezcat.Framework.Game
{
    [System.Serializable]
    public class TezPlanetData
    {
        [Header("NoiseData")]
        public float frequency = 8;
        public int layer = 4;
        public float lacunarity = 1.2f;
        public float persistence = 0.5f;
        public Vector3 offset;

        [Header("PlanetData")]
        public int resolution;
        public float radius;
        public float roughness;
        public float strength = 1;
        public float minLevel;

        public float evaluate(Vector3 point)
        {
            float nosie = 0;
            float frequency = this.frequency;
            float amplitude = 1;

            for (int i = 0; i < layer; i++)
            {
                float temp = TezNoise.perlin3D(point + offset, frequency);
                nosie = (temp + 1) * 0.5f * amplitude;
                frequency *= lacunarity;
                amplitude *= persistence;
            }

            return nosie * strength;
        }
    }
}