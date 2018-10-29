using tezcat.Framework.Core;

namespace tezcat.Framework.Math
{
    public interface ITezRandom : ITezService
    {
        void setSeed(int seed);
        int nextInt();
        int nextInt(int min, int max);
        float nextFloat();
        float nextFloat(float min, float max);
    }

    public interface ITezMath : ITezRandom
    {

    }

    /// <summary>
    /// 
    /// 16807随机数发生器
    /// 分布率为63.xx%
    /// 
    /// </summary>
    public class TezRandom : ITezRandom
    {
        private const uint MAX_MASK_UINT = int.MaxValue;
        //        private const float MAX_MASK_FLOAT = 2.14748365E+09f;
        private const float MAX_MASK_FLOAT = MAX_MASK_UINT;
        private const uint factor = 16807;

        private uint m_Seed;
        private static int DefaultSeed = "DefaultSeed".GetHashCode();

        public TezRandom()
        {
            m_Seed = (uint)DefaultSeed;
        }

        public TezRandom(string seed)
        {
            m_Seed = (uint)seed.GetHashCode();
        }

        public void setSeed(int seed)
        {
            m_Seed = (uint)seed;
        }

        private uint gen()
        {
            uint lo = factor * (m_Seed & 0xFFFF);
            uint hi = factor * (m_Seed >> 16);

            lo += (hi & 0x7FFF) << 16;
            lo += hi >> 15;

            if (lo > 0x7FFFFFFF)
            {
                lo -= 0x7FFFFFFF;
            }

            m_Seed = (uint)(int)lo;

            return m_Seed;

            //            return m_Seed = (m_Seed * factor) & MAX_MASK_UINT;
        }

        public int nextInt()
        {
            return (int)this.gen();
        }

        public int nextInt(int min, int max)
        {
            return (int)(min + (max - min) * this.nextFloat());
        }

        public float nextFloat()
        {
            return this.gen() / MAX_MASK_FLOAT;
        }

        public float nextFloat(float min, float max)
        {
            return min + (max - min) * this.nextFloat();
        }

        public void close()
        {

        }
    }
}