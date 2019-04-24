using Sirenix.OdinInspector;
using tezcat.Framework.Utility;
using UnityEngine;
using UnityEngine.UI;

public class NoiseTest : MonoBehaviour
{
    [Range(2, 1024)]
    public int resolution = 512;

    public float frequency = 1f;
    [Range(1, 8)]
    public int octaves = 1;
    [Range(1, 4)]
    public float lacunarity = 2f;
    [Range(0, 1)]
    public float persistence = 0.5f;

    public Vector2 offset = Vector2.zero;

    [SerializeField]
    RawImage m_RawImage = null;
    Texture2D m_Texture2D = null;

    [Button]
    public void generate()
    {
        if(m_Texture2D == null)
        {
            m_Texture2D = new Texture2D(resolution, resolution, TextureFormat.RGB24, true);
            m_Texture2D.name = "Procedural Texture";
            m_Texture2D.wrapMode = TextureWrapMode.Clamp;
            m_Texture2D.filterMode = FilterMode.Trilinear;
            m_Texture2D.anisoLevel = 9;
            m_RawImage.texture = m_Texture2D;
        }

        if (m_Texture2D.width != resolution)
        {
            m_Texture2D.Resize(resolution, resolution);
        }

        for (int y = 0; y < resolution; y++)
        {
            for (int x = 0; x < resolution; x++)
            {
                var sample = TezNoise.sum(TezNoise.Function.Perlin2D
                    , new Vector3(offset.x + (float)x / resolution, offset.y + (float)y / resolution)
                    , frequency
                    , octaves
                    , lacunarity
                    , persistence);

                sample = (sample + 1) * 0.5f;
                m_Texture2D.SetPixel(x, y, new Color(sample, sample, sample));
            }
        }
        m_Texture2D.Apply();
    }

    private void OnValidate()
    {
        this.generate();
    }
}
