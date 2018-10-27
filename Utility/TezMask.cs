using System;
using tezcat.Framework.Core;

namespace tezcat.Framework.Utility
{
    public interface ITezMask
        : ITezCloseable
    {
        ulong masks { get; }
        void set(params int[] indices);
        bool this[int index] { get; set; }
        int length { get; }
    }

    public struct TezMask32
        : ITezMask
        , IEquatable<TezMask32>
    {
        private const int BitCount = 32;
        private uint m_Data;

        public ulong masks
        {
            get { return this.m_Data; }
        }

        public int length
        {
            get { return BitCount; }
        }

        public bool this[int index]
        {
            get
            {
                if (index >= BitCount)
                {
                    throw new ArgumentOutOfRangeException(string.Format("index must less than {0}", BitCount));
                }

                uint num = 1u << index;
                return (this.m_Data & num) == num;
            }
            set
            {
                if (index >= BitCount)
                {
                    throw new ArgumentOutOfRangeException(string.Format("index must less than {0}", BitCount));
                }

                uint num = 1u << index;

                if (value)
                {
                    this.m_Data |= num;
                }
                else
                {
                    this.m_Data &= ~num;
                }
            }
        }

        public TezMask32(uint mask)
        {
            this.m_Data = mask;
        }

        public void set(params int[] indices)
        {
            uint num = 0u;
            for (int i = 0; i < indices.Length; i++)
            {
                num |= 1u << indices[i];
            }
            this.m_Data |= num;
        }

        public bool test(TezMask32 other)
        {
            return (m_Data & other.m_Data) == other.m_Data;
        }

        public void close()
        {
            this.m_Data = 0u;
        }

        public override string ToString()
        {
            return m_Data.ToString("X8");
        }

        public override int GetHashCode()
        {
            return m_Data.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return other is TezMask32 && this.Equals((TezMask32)other);
        }

        public bool Equals(TezMask32 other)
        {
            return other.m_Data == m_Data;
        }

        public static TezMask32 operator |(TezMask32 a, TezMask32 b)
        {
            return new TezMask32(a.m_Data | b.m_Data);
        }

        public static TezMask32 operator &(TezMask32 a, TezMask32 b)
        {
            return new TezMask32(a.m_Data & b.m_Data);
        }

        public static TezMask32 operator ^(TezMask32 a, TezMask32 b)
        {
            return new TezMask32(a.m_Data ^ b.m_Data);
        }

        public static bool operator ==(TezMask32 a, TezMask32 b)
        {
            return a.m_Data == b.m_Data;
        }

        public static bool operator !=(TezMask32 a, TezMask32 b)
        {
            return a.m_Data != b.m_Data;
        }
    }

    public struct TezMask64
        : ITezMask
        , IEquatable<TezMask64>
    {
        private const int BitCount = 64;
        private ulong m_Data;

        public ulong masks
        {
            get { return this.m_Data; }
        }

        public int length
        {
            get { return BitCount; }
        }

        public bool this[int index]
        {
            get
            {
                if (index >= BitCount)
                {
                    throw new ArgumentOutOfRangeException(string.Format("index must less than {0}", BitCount));
                }

                ulong num = 1UL << index;

                return (this.m_Data & num) == num;
            }
            set
            {
                if (index >= BitCount)
                {
                    throw new ArgumentOutOfRangeException(string.Format("index must less than {0}", BitCount));
                }

                ulong num = 1UL << index;
                if (value)
                {
                    this.m_Data |= num;
                }
                else
                {
                    this.m_Data &= ~num;
                }
            }
        }

        public TezMask64(ulong mask)
        {
            m_Data = mask;
        }

        public void set(params int[] indices)
        {
            ulong num = 0UL;
            for (int i = 0; i < indices.Length; i++)
            {
                num |= 1UL << indices[i];
            }
            this.m_Data |= num;
        }

        public bool test(TezMask64 other)
        {
            return (m_Data & other.m_Data) == other.m_Data;
        }

        public void close()
        {
            this.m_Data = 0UL;
        }

        public override string ToString()
        {
            return this.m_Data.ToString("X8");
        }

        public override int GetHashCode()
        {
            return this.m_Data.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return other is TezMask64 && this.Equals((TezMask64)other);
        }

        public bool Equals(TezMask64 other)
        {
            return other.m_Data == this.m_Data;
        }

        public static TezMask64 operator |(TezMask64 a, TezMask64 b)
        {
            return new TezMask64(a.m_Data | b.m_Data);
        }

        public static TezMask64 operator &(TezMask64 a, TezMask64 b)
        {
            return new TezMask64(a.m_Data & b.m_Data);
        }

        public static TezMask64 operator ^(TezMask64 a, TezMask64 b)
        {
            return new TezMask64(a.m_Data ^ b.m_Data);
        }

        public static bool operator ==(TezMask64 a, TezMask64 b)
        {
            return a.m_Data == b.m_Data;
        }

        public static bool operator !=(TezMask64 a, TezMask64 b)
        {
            return a.m_Data != b.m_Data;
        }
    }
}
