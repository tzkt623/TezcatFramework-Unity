using tezcat.Core;

namespace tezcat.Math
{
    public interface ITezRandom : ITezService
    {
        void setSeed(int seed);
        int nextInt();
        int nextInt(float min, float max);
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
        private const uint MAX_MASK_UINT = 2147483647u;
        private const float MAX_MASK_FLOAT = 2.14748365E+09f;
        private const int factor = 16807;

        private uint m_Seed;
        private static int DefaultSeed = "Tezcat".GetHashCode();

        public TezRandom()
        {
            m_Seed = (uint)DefaultSeed;
        }

        public void setSeed(int seed)
        {
            m_Seed = (uint)seed;
        }

        private uint gen()
        {
            return m_Seed = (m_Seed * factor & MAX_MASK_UINT);
        }

        public int nextInt()
        {
            return (int)this.gen();
        }

        public int nextInt(float min, float max)
        {
            return (int)(min + (max - min) * this.nextFloat() + 0.5f);
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