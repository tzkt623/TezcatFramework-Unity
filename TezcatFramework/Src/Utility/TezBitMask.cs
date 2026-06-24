namespace tezcat.Framework.Utility
{
    public class TezBitMask8
    {
        byte mBit;
        public void set(byte value)
        {
            mBit |= value;
        }

        public void clear(byte value)
        {
            mBit &= (byte)~value;
        }

        public bool allOf(byte value)
        {
            return (mBit & value) == value;
        }

        public bool anyOf(byte value)
        {
            return (mBit & value) > 0;
        }

        public bool noneOf(byte value)
        {
            return (mBit & value) == 0;
        }

        public void reset()
        {
            mBit = 0;
        }
    }

    public class TezBitMask16
    {
        ushort mBit;
        public void set(ushort value)
        {
            mBit |= value;
        }

        public void clear(ushort value)
        {
            mBit &= (ushort)~value;
        }

        public bool allOf(ushort value)
        {
            return (mBit & value) == value;
        }

        public bool anyOf(ushort value)
        {
            return (mBit & value) > 0;
        }

        public bool noneOf(ushort value)
        {
            return (mBit & value) == 0;
        }

        public void reset()
        {
            mBit = 0;
        }
    }

    public class TezBitMask32
    {
        uint mBit;
        public void set(uint value)
        {
            mBit |= value;
        }

        public void clear(uint value)
        {
            mBit &= ~value;
        }

        public bool allOf(uint value)
        {
            return (mBit & value) == value;
        }

        public bool anyOf(uint value)
        {
            return (mBit & value) > 0;
        }

        public bool noneOf(uint value)
        {
            return (mBit & value) == 0;
        }

        public void reset()
        {
            mBit = 0;
        }
    }

    public class TezBitMask64
    {
        ulong mBit;
        public void set(ulong value)
        {
            mBit |= value;
        }

        public void clear(ulong value)
        {
            mBit &= ~value;
        }

        public bool allOf(ulong value)
        {
            return (mBit & value) == value;
        }

        public bool anyOf(ulong value)
        {
            return (mBit & value) > 0;
        }

        public bool noneOf(ulong value)
        {
            return (mBit & value) == 0;
        }

        public void reset()
        {
            mBit = 0;
        }
    }
}