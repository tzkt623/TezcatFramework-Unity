using System;
using tezcat.Framework.Extension;
using UnityEngine;

namespace tezcat.Framework.Utility
{
    /// <summary>
    /// 噪声
    /// </summary>
    public static class TezNoise
    {
        static TezEventExtension.Function<float, float, float>[] s_F1Ds = new TezEventExtension.Function<float, float, float>[]
        {
            value1D,
            perlin1D,
        };

        static TezEventExtension.Function<float, Vector2, float>[] s_F2Ds = new TezEventExtension.Function<float, Vector2, float>[]
        {
            value2D,
            perlin2D
        };

        static TezEventExtension.Function<float, Vector3, float>[] s_F3Ds = new TezEventExtension.Function<float, Vector3, float>[]
        {
            value3D,
            perlin3D
        };

        static Function s_FunctionCache = Function.Error;
        static TezEventExtension.Function<float, float, float> s_F1DCache = null;
        static TezEventExtension.Function<float, Vector2, float> s_F2DCache = null;
        static TezEventExtension.Function<float, Vector3, float> s_F3DCache = null;


        public enum Function
        {
            Error = -1,
            Value1D = 0,
            Perlin1D = 1,
            Value2D = 2,
            Perlin2D = 3,
            Value3D = 4,
            Perlin3D = 5,
        }

        #region 工具
        /// <summary>
        /// 双倍(复制的内容)数组长度
        /// 用于优化value(X)D中的(point&Mask)运算
        /// 如果没有双倍长度
        /// 每次从Array中获取数值时
        /// 都需要(point&Mask)运算防止index越界
        /// 这样就优化掉了(point&Mask)运算
        /// 并且不会出现长度越界的情况
        /// </summary>
        private static int[] s_HashArray =
        {
            151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
            140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
            247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
             57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
             74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
             60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
             65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
            200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
             52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
            207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
            119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
            129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
            218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
             81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
            184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
            222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180,

            151,160,137, 91, 90, 15,131, 13,201, 95, 96, 53,194,233,  7,225,
            140, 36,103, 30, 69,142,  8, 99, 37,240, 21, 10, 23,190,  6,148,
            247,120,234, 75,  0, 26,197, 62, 94,252,219,203,117, 35, 11, 32,
             57,177, 33, 88,237,149, 56, 87,174, 20,125,136,171,168, 68,175,
             74,165, 71,134,139, 48, 27,166, 77,146,158,231, 83,111,229,122,
             60,211,133,230,220,105, 92, 41, 55, 46,245, 40,244,102,143, 54,
             65, 25, 63,161,  1,216, 80, 73,209, 76,132,187,208, 89, 18,169,
            200,196,135,130,116,188,159, 86,164,100,109,198,173,186,  3, 64,
             52,217,226,250,124,123,  5,202, 38,147,118,126,255, 82, 85,212,
            207,206, 59,227, 47, 16, 58, 17,182,189, 28, 42,223,183,170,213,
            119,248,152,  2, 44,154,163, 70,221,153,101,155,167, 43,172,  9,
            129, 22, 39,253, 19, 98,108,110, 79,113,224,232,178,185,112,104,
            218,246, 97,228,251, 34,242,193,238,210,144, 12,191,179,162,241,
             81, 51,145,235,249, 14,239,107, 49,192,214, 31,181,199,106,157,
            184, 84,204,176,115,121, 50, 45,127,  4,150,254,138,236,205, 93,
            222,114, 67, 29, 24, 72,243,141,128,195, 78, 66,215, 61,156,180
        };

        private const int m_HashMask = 255;
        private const float m_MaskRate = 1 / m_HashMask;

        private static float smooth(float t)
        {
            return t * t * t * (t * (t * 6f - 15f) + 10f);
        }

        private static float dot(Vector2 g, float x, float y)
        {
            return g.x * x + g.y * y;
        }

        private static float dot(Vector3 g, float x, float y, float z)
        {
            return g.x * x + g.y * y + g.z * z;
        }

        /// <summary>
        /// 更换随机Hash表的值
        /// hash表的长度一定是256
        /// 如果不明白原理
        /// 就不要使用这个函数
        /// </summary>
        public static void changeHashArray(int[] hashArray)
        {
            const int length = 256;
            if (hashArray.Length != length)
            {
                throw new Exception("Hash Array Must Have 256 Values!!");
            }

            s_HashArray = new int[length * 2];
            Array.Copy(hashArray, 0, s_HashArray, 0, length);
            Array.Copy(hashArray, 0, s_HashArray, length, length);
        }
        #endregion

        #region 值噪音(随机数)
        public static float value1D(float point, float frequency)
        {
            ///振动!!!!
            point *= frequency;

            ///获得当前点左边参考值的Index
            int i0 = Mathf.FloorToInt(point);

            ///计算当前点到左边参考值Index的距离
            ///用于作为插值的粗略系数
            var t = point - i0;

            ///将获得的参考值的Index归一化到HashArray的长度中
            i0 &= m_HashMask;

            ///获得右边参考值的Index
            int i1 = i0 + 1;

            ///取得左边点和右边点的参考值
            int h0 = s_HashArray[i0];
            int h1 = s_HashArray[i1];

            ///获得平滑的插值系数
            t = smooth(t);

            ///用当前点左右两边的参考值
            ///配合平滑插值系数进行插值
            ///取得当前点真正的参考值(Mathf.Lerp(h0, h1, t))
            ///并且转换到0-1之内
            ///便获得了平滑的一维随机数
            return Mathf.Lerp(h0, h1, t) * m_MaskRate;
        }

        public static float value1D(Vector3 point, float frequency)
        {
            return value1D(point.x, frequency);
        }

        public static float value2D(Vector2 point, float frequency)
        {
            point *= frequency;

            int ix0 = Mathf.FloorToInt(point.x);
            int iy0 = Mathf.FloorToInt(point.y);

            var tx = point.x - ix0;
            var ty = point.y - iy0;

            ix0 &= m_HashMask;
            iy0 &= m_HashMask;

            int ix1 = ix0 + 1;
            int iy1 = iy0 + 1;

            ///取得线上的值
            ///为了下一步计算面上的值
            int h0 = s_HashArray[ix0];
            int h1 = s_HashArray[ix1];

            ///计算面上的值
            ///用于插值计算
            int h00 = s_HashArray[h0 + iy0];
            int h10 = s_HashArray[h1 + iy0];
            int h01 = s_HashArray[h0 + iy1];
            int h11 = s_HashArray[h1 + iy1];

            tx = smooth(tx);
            ty = smooth(ty);

            return Mathf.Lerp(Mathf.Lerp(h00, h10, tx)
                , Mathf.Lerp(h01, h11, tx)
                , ty) * m_MaskRate;
        }

        public static float value2D(Vector3 point, float frequency)
        {
            return value2D(new Vector2(point.x, point.y), frequency);
        }

        public static float value3D(Vector3 point, float frequency)
        {
            point *= frequency;
            int ix0 = Mathf.FloorToInt(point.x);
            int iy0 = Mathf.FloorToInt(point.y);
            int iz0 = Mathf.FloorToInt(point.z);

            var tx = point.x - ix0;
            var ty = point.y - iy0;
            var tz = point.z - iz0;

            ix0 &= m_HashMask;
            iy0 &= m_HashMask;
            iz0 &= m_HashMask;

            int ix1 = ix0 + 1;
            int iy1 = iy0 + 1;
            int iz1 = iz0 + 1;

            ///计算线上的值
            ///为了下一步计算面上的值
            int h0 = s_HashArray[ix0];
            int h1 = s_HashArray[ix1];

            ///计算面上的值
            ///为了下一步计算体上的值
            int h00 = s_HashArray[h0 + iy0];
            int h10 = s_HashArray[h1 + iy0];
            int h01 = s_HashArray[h0 + iy1];
            int h11 = s_HashArray[h1 + iy1];

            ///计算体上的值
            ///用于插值计算
            int h000 = s_HashArray[h00 + iz0];
            int h100 = s_HashArray[h10 + iz0];
            int h010 = s_HashArray[h01 + iz0];
            int h110 = s_HashArray[h11 + iz0];
            int h001 = s_HashArray[h00 + iz1];
            int h101 = s_HashArray[h10 + iz1];
            int h011 = s_HashArray[h01 + iz1];
            int h111 = s_HashArray[h11 + iz1];

            tx = smooth(tx);
            ty = smooth(ty);
            tz = smooth(tz);

            return Mathf.Lerp(
                Mathf.Lerp(Mathf.Lerp(h000, h100, tx), Mathf.Lerp(h010, h110, tx), ty)
                , Mathf.Lerp(Mathf.Lerp(h001, h101, tx), Mathf.Lerp(h011, h111, tx), ty)
                , tz) * m_MaskRate;
        }
        #endregion

        #region 柏林噪音(随机数)
        private static float[] m_Gradients1D = { 1f, -1f };
        private const int m_GradientsMask1D = 1;


        private static Vector2[] m_Gradients2D =
        {
            new Vector2( 1f, 0f),
            new Vector2(-1f, 0f),
            new Vector2( 0f, 1f),
            new Vector2( 0f,-1f),
            new Vector2( 1f, 1f).normalized,
            new Vector2(-1f, 1f).normalized,
            new Vector2( 1f,-1f).normalized,
            new Vector2(-1f,-1f).normalized
        };
        private const int m_GradientsMask2D = 7;
        private static float Sqrt2 = Mathf.Sqrt(2f);


        private static Vector3[] m_Gradients3D =
        {
            new Vector3( 1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f),
            new Vector3( 1f,-1f, 0f),
            new Vector3(-1f,-1f, 0f),
            new Vector3( 1f, 0f, 1f),
            new Vector3(-1f, 0f, 1f),
            new Vector3( 1f, 0f,-1f),
            new Vector3(-1f, 0f,-1f),
            new Vector3( 0f, 1f, 1f),
            new Vector3( 0f,-1f, 1f),
            new Vector3( 0f, 1f,-1f),
            new Vector3( 0f,-1f,-1f),

            new Vector3( 1f, 1f, 0f),
            new Vector3(-1f, 1f, 0f),
            new Vector3( 0f,-1f, 1f),
            new Vector3( 0f,-1f,-1f)
        };
        private const int m_GradientsMask3D = 15;

        public static float perlin1D(float point, float frequency)
        {
            point *= frequency;
            int i0 = Mathf.FloorToInt(point);
            var t0 = point - i0;
            var t1 = t0 - 1;
            i0 &= m_HashMask;

            int i1 = i0 + 1;

            float g0 = m_Gradients1D[s_HashArray[i0] & m_GradientsMask1D];
            float g1 = m_Gradients1D[s_HashArray[i1] & m_GradientsMask1D];

            float v0 = g0 * t0;
            float v1 = g1 * t1;

            var t = smooth(t0);

            return Mathf.Lerp(v0, v1, t) * 2f;
        }

        public static float perlin1D(Vector3 point, float frequency)
        {
            return perlin1D(point.x, frequency);
        }

        public static float perlin2D(Vector2 point, float frequency)
        {
            point *= frequency;

            int ix0 = Mathf.FloorToInt(point.x);
            int iy0 = Mathf.FloorToInt(point.y);

            var tx0 = point.x - ix0;
            var ty0 = point.y - iy0;

            var tx1 = tx0 - 1f;
            var ty1 = ty0 - 1f;

            ix0 &= m_HashMask;
            iy0 &= m_HashMask;

            int ix1 = ix0 + 1;
            int iy1 = iy0 + 1;

            int h0 = s_HashArray[ix0];
            int h1 = s_HashArray[ix1];

            var g00 = m_Gradients2D[s_HashArray[h0 + iy0] & m_GradientsMask2D];
            var g10 = m_Gradients2D[s_HashArray[h1 + iy0] & m_GradientsMask2D];
            var g01 = m_Gradients2D[s_HashArray[h0 + iy1] & m_GradientsMask2D];
            var g11 = m_Gradients2D[s_HashArray[h1 + iy1] & m_GradientsMask2D];

            float v00 = dot(g00, tx0, ty0);
            float v10 = dot(g10, tx1, ty0);
            float v01 = dot(g01, tx0, ty1);
            float v11 = dot(g11, tx1, ty1);

            float tx = smooth(tx0);
            float ty = smooth(ty0);

            return Mathf.Lerp(Mathf.Lerp(v00, v10, tx)
                , Mathf.Lerp(v01, v11, tx)
                , ty) * Sqrt2;
        }

        public static float perlin2D(Vector3 point, float frequency)
        {
            return perlin2D(new Vector2(point.x, point.y), frequency);
        }

        public static float perlin3D(Vector3 point, float frequency)
        {
            point *= frequency;
            int ix0 = Mathf.FloorToInt(point.x);
            int iy0 = Mathf.FloorToInt(point.y);
            int iz0 = Mathf.FloorToInt(point.z);

            var tx0 = point.x - ix0;
            var ty0 = point.y - iy0;
            var tz0 = point.z - iz0;
            var tx1 = tx0 - 1f;
            var ty1 = ty0 - 1f;
            var tz1 = tz0 - 1f;

            ix0 &= m_HashMask;
            iy0 &= m_HashMask;
            iz0 &= m_HashMask;

            int ix1 = ix0 + 1;
            int iy1 = iy0 + 1;
            int iz1 = iz0 + 1;

            int h0 = s_HashArray[ix0];
            int h1 = s_HashArray[ix1];

            int h00 = s_HashArray[h0 + iy0];
            int h10 = s_HashArray[h1 + iy0];
            int h01 = s_HashArray[h0 + iy1];
            int h11 = s_HashArray[h1 + iy1];

            var g000 = m_Gradients3D[s_HashArray[h00 + iz0] & m_GradientsMask3D];
            var g100 = m_Gradients3D[s_HashArray[h10 + iz0] & m_GradientsMask3D];
            var g010 = m_Gradients3D[s_HashArray[h01 + iz0] & m_GradientsMask3D];
            var g110 = m_Gradients3D[s_HashArray[h11 + iz0] & m_GradientsMask3D];
            var g001 = m_Gradients3D[s_HashArray[h00 + iz1] & m_GradientsMask3D];
            var g101 = m_Gradients3D[s_HashArray[h10 + iz1] & m_GradientsMask3D];
            var g011 = m_Gradients3D[s_HashArray[h01 + iz1] & m_GradientsMask3D];
            var g111 = m_Gradients3D[s_HashArray[h11 + iz1] & m_GradientsMask3D];

            float v000 = dot(g000, tx0, ty0, tz0);
            float v100 = dot(g100, tx1, ty0, tz0);
            float v010 = dot(g010, tx0, ty1, tz0);
            float v110 = dot(g110, tx1, ty1, tz0);
            float v001 = dot(g001, tx0, ty0, tz1);
            float v101 = dot(g101, tx1, ty0, tz1);
            float v011 = dot(g011, tx0, ty1, tz1);
            float v111 = dot(g111, tx1, ty1, tz1);

            float tx = smooth(tx0);
            float ty = smooth(ty0);
            float tz = smooth(tz0);

            return Mathf.Lerp(
                Mathf.Lerp(Mathf.Lerp(v000, v100, tx), Mathf.Lerp(v010, v110, tx), ty),
                Mathf.Lerp(Mathf.Lerp(v001, v101, tx), Mathf.Lerp(v011, v111, tx), ty),
                tz);
        }
        #endregion

        #region 分形噪音
        public static void begin(Function method)
        {
            s_FunctionCache = method;
            switch (method)
            {
                case Function.Value1D:
                    s_F1DCache = value1D;
                    break;
                case Function.Perlin1D:
                    s_F1DCache = perlin1D;
                    break;
                case Function.Value2D:
                    s_F2DCache = value2D;
                    break;
                case Function.Perlin2D:
                    s_F2DCache = perlin2D;
                    break;
                case Function.Value3D:
                    s_F3DCache = value3D;
                    break;
                case Function.Perlin3D:
                    s_F3DCache = perlin3D;
                    break;
                default:
                    break;
            }
        }

        public static void end(Function method)
        {
            if (s_FunctionCache != method)
            {
                throw new Exception(string.Format("TezNoise End ERROR!![Current:{0}][You Want:{1}]", s_FunctionCache.ToString(), method.ToString()));
            }

            s_FunctionCache = Function.Error;
            s_F1DCache = null;
            s_F2DCache = null;
            s_F3DCache = null;
        }

        public static float sum1D(float point, float frequency, int octaves, float lacunarity, float persistence)
        {
            float sum = s_F1DCache(point, frequency);
            float amplitude = 1f;
            float range = 1f;
            for (int o = 1; o < octaves; o++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += s_F1DCache(point, frequency) * amplitude;
            }
            return sum / range;
        }

        public static float sum2D(Vector2 point, float frequency, int octaves, float lacunarity, float persistence)
        {
            float sum = s_F2DCache(point, frequency);
            float amplitude = 1f;
            float range = 1f;
            for (int o = 1; o < octaves; o++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += s_F2DCache(point, frequency) * amplitude;
            }
            return sum / range;
        }

        public static float sum3D(Vector3 point, float frequency, int octaves, float lacunarity, float persistence)
        {
            float sum = s_F3DCache(point, frequency);
            float amplitude = 1f;
            float range = 1f;
            for (int o = 1; o < octaves; o++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += s_F3DCache(point, frequency) * amplitude;
            }
            return sum / range;
        }

        /// <summary>
        /// 范围-1到1
        /// </summary>
        /// <param name="method"></param>
        /// <param name="point"></param>
        /// <param name="frequency"></param>
        /// <param name="octaves"></param>
        /// <param name="lacunarity"></param>
        /// <param name="persistence"></param>
        /// <returns></returns>
        public static float sum1D(Function method, float point, float frequency, int octaves, float lacunarity, float persistence)
        {
            var function = s_F1Ds[(int)method];

            float sum = function(point, frequency);
            float amplitude = 1f;
            float range = 1f;
            for (int o = 1; o < octaves; o++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += function(point, frequency) * amplitude;
            }
            return sum / range;
        }

        /// <summary>
        /// 分形2D
        /// </summary>
        /// <param name="method"></param>
        /// <param name="point"></param>
        /// <param name="frequency"></param>
        /// <param name="octaves"></param>
        /// <param name="lacunarity"></param>
        /// <param name="persistence"></param>
        /// <returns>-1到1</returns>
        public static float sum2D(Function method, Vector2 point, float frequency, int octaves, float lacunarity, float persistence)
        {
            var function = s_F2Ds[(int)method - 2];

            float sum = function(point, frequency);
            float amplitude = 1f;
            float range = 1f;
            for (int o = 1; o < octaves; o++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += function(point, frequency) * amplitude;
            }
            return sum / range;
        }

        public static float sum3D(Function method, Vector3 point, float frequency, int octaves, float lacunarity, float persistence)
        {
            var function = s_F3Ds[(int)method - 4];

            float sum = function(point, frequency);
            float amplitude = 1f;
            float range = 1f;
            for (int o = 1; o < octaves; o++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                sum += function(point, frequency) * amplitude;
            }
            return sum / range;
        }

        public static float sumWithUnityPerlinNoise(Vector3 point, float frequency, int octaves, float lacunarity, float persistence)
        {
            var fp = point * frequency;

            float sum = Mathf.PerlinNoise(fp.x, fp.y);
            float amplitude = 1f;
            float range = 1f;
            for (int o = 1; o < octaves; o++)
            {
                frequency *= lacunarity;
                amplitude *= persistence;
                range += amplitude;
                fp = point * frequency;
                sum += Mathf.PerlinNoise(fp.x, fp.y) * amplitude;
            }
            return sum / range;
        }
        #endregion
    }
}