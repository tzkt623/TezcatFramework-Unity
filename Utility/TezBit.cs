namespace tezcat.Framework.Utility
{
    public struct TezBit
    {
        uint m_Bit;
        public uint bit
        {
            get { return m_Bit; }
        }

        public TezBit(uint value)
        {
            m_Bit = value;
        }

        public void add(uint value)
        {
            m_Bit |= value;
        }

        public void remove(uint value)
        {
            m_Bit &= ~value;
        }

        public bool test(uint value)
        {
            return (m_Bit & value) == value;
        }

        public static implicit operator TezBit(uint value)
        {
            return new TezBit(value);
        }

        public static implicit operator uint(TezBit bit)
        {
            return bit.m_Bit;
        }
    }
}