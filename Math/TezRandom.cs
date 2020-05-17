using tezcat.Framework.Core;

namespace tezcat.Framework.Math
{
    /// <summary>
    /// 
    /// 16807随机数发生器
    /// 分布率为63.xx%
    /// 
    /// </summary>
    public class TezRandom : ITezService
    {
        private const uint MAX_MASK_UINT = int.MaxValue;
        /// <summary>
        /// 因为flota精度问题 有效数字只有7位
        /// 所以int最大值 2147483647 用float表示应该为 2.147483E+09
        /// 那么要算出0-1之间的值 必须超过这个值 所以+1000 变成2.147484E+09
        /// 这样就保证可以算出0-0.99999x之间的数了
        /// </summary>
        private const float MAX_MASK_FLOAT = int.MaxValue + 1000f;
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

        public uint getSeed()
        {
            return m_Seed;
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
            return min + (int)((max - min) * this.nextFloat());
        }

        public float nextFloat()
        {
            return this.gen() / MAX_MASK_FLOAT;
        }

        public float nextFloat(float min, float max)
        {
            return min + (max - min) * this.nextFloat();
        }

        public bool nextBool()
        {
            ///0 2 4 6 8
            ///1 3 5 7 9
            return (this.nextInt(0, 10) & 1) == 0;
        }

        public void close(bool self_close = true)
        {

        }
    }
}