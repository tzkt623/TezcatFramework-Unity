namespace tezcat.Framework.Utility
{
    public class TezBitMask8
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

        public bool allOf(byte value)
        {
            return (m_Bit & value) == value;
        }

        public bool anyOf(byte value)
        {
            return (m_Bit & value) > 0;
        }

        public bool noneOf(byte value)
        {
            return (m_Bit & value) == 0;
        }

        public void reset()
        {
            m_Bit = 0;
        }
    }

    public struct TezBitMask16
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

        public bool allOf(ushort value)
        {
            return (m_Bit & value) == value;
        }

        public bool anyOf(ushort value)
        {
            return (m_Bit & value) > 0;
        }

        public bool noneOf(ushort value)
        {
            return (m_Bit & value) == 0;
        }

        public void reset()
        {
            m_Bit = 0;
        }
    }

    public struct TezBitMask32
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

        public bool allOf(uint value)
        {
            return (m_Bit & value) == value;
        }

        public bool anyOf(uint value)
        {
            return (m_Bit & value) > 0;
        }

        public bool noneOf(uint value)
        {
            return (m_Bit & value) == 0;
        }

        public void reset()
        {
            m_Bit = 0;
        }
    }

    public struct TezBitMask64
    {
        ulong m_Bit;
        public void set(ulong value)
        {
            m_Bit |= value;
        }

        public void clear(ulong value)
        {
            m_Bit &= ~value;
        }

        public bool allOf(ulong value)
        {
            return (m_Bit & value) == value;
        }

        public bool anyOf(ulong value)
        {
            return (m_Bit & value) > 0;
        }

        public bool noneOf(ulong value)
        {
            return (m_Bit & value) == 0;
        }

        public void reset()
        {
            m_Bit = 0;
        }
    }
}