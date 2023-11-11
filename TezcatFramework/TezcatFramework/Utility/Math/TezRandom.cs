namespace tezcat.Framework.TMath
{
    /// <summary>
    /// 
    /// xorShift随机数
    /// 
    /// </summary>
    public class TezRandom
    {
        private static uint DefaultSeed = (uint)"DefaultSeed".GetHashCode();
        private const double MAX_MASK_DOUBLE = uint.MaxValue + 1d;

        private uint mSeed;

        public TezRandom()
        {
            mSeed = DefaultSeed;
        }

        public TezRandom(string seed)
        {
            mSeed = (uint)seed.GetHashCode();
        }

        public TezRandom(uint seed)
        {
            mSeed = seed;
        }

        public void setSeed(uint seed)
        {
            mSeed = seed;
        }

        public void setSeed(string seed)
        {
            mSeed = (uint)seed.GetHashCode();
        }

        public uint getSeed()
        {
            return mSeed;
        }

        /// <summary>
        /// 随机一个UInt值
        /// </summary>
        public uint nextUInt()
        {
            mSeed = this.xorShift32(mSeed);
            return mSeed;
        }

        /// <summary>
        /// [0.0, 1.0)
        /// </summary>
        public double nextDouble()
        {
            return this.nextUInt() / MAX_MASK_DOUBLE;
        }

        /// <summary>
        /// 随机一个Int值
        /// </summary>
        public int nextInt()
        {
            return (int)this.nextUInt();
        }


        /// <summary>
        /// [min, max)
        /// </summary>
        public int nextInt(int min, int max)
        {
            return min + (int)((max - min) * this.nextDouble());
        }

        /// <summary>
        /// [min, max)
        /// </summary>
        public float nextFloat(float min, float max)
        {
            return min + (max - min) * this.nextFloat();
        }

        /// <summary>
        /// [0.0, 1.0)
        /// </summary>
        public double nextDouble(double min, double max)
        {
            return min + (max - min) * this.nextDouble();
        }

        /// <summary>
        /// [0.0, 1.0)
        /// </summary>
        public float nextFloat()
        {
            return (float)this.nextDouble();
        }

        /// <summary>
        /// 随机一个Bool
        /// </summary>
        public bool nextBool()
        {
            ///0 2 4 6 8
            ///1 3 5 7 9
            return (this.nextInt(0, 10) & 1) == 0;
        }

        private uint xorShift32(uint seed)
        {
            seed ^= seed << 13;
            seed ^= seed >> 17;
            seed ^= seed << 5;
            return seed;
        }

        private ulong xorShift64Star(ulong seed)
        {
            seed ^= seed >> 12;
            seed ^= seed << 25;
            seed ^= seed >> 27;
            return seed * 0x2545F4914F6CDD1Du;
        }
    }
}