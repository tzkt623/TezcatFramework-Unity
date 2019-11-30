namespace tezcat.Framework.Utility
{
    public struct TezBitMask_byte
    {
        byte m_Bit;
        public void set(byte value)
        {
            m_Bit |= value;
        }

        public void clear(byte value)
        {
            m_Bit &= (byte)~value;
        }

        public bool test(byte value)
        {
            return (m_Bit & value) == value;
        }

        public void reset()
        {
            m_Bit = 0;
        }
    }

    public struct TezBitMask_ushort
    {
        ushort m_Bit;
        public void set(ushort value)
        {
            m_Bit |= value;
        }

        public void clear(ushort value)
        {
            m_Bit &= (ushort)~value;
        }

        public bool test(ushort value)
        {
            return (m_Bit & value) == value;
        }

        public void reset()
        {
            m_Bit = 0;
        }
    }

    public struct TezBitMask_uint
    {
        uint m_Bit;
        public void set(uint value)
        {
            m_Bit |= value;
        }

        public void clear(uint value)
        {
            m_Bit &= ~value;
        }

        public bool test(uint value)
        {
            return (m_Bit & value) == value;
        }

        public void reset()
        {
            m_Bit = 0;
        }
    }
}