namespace tezcat.Framework.Extension
{
    public static class TezBitMaskExtension
    {
        public static void setBitMask(this byte mask, byte value)
        {
            mask |= value;
        }

        public static void setBitMask(this ushort mask, ushort value)
        {
            mask |= value;
        }

        public static void setBitMask(this uint mask, uint value)
        {
            mask |= value;
        }

        public static void setBitMask(this ulong mask, ulong value)
        {
            mask |= value;
        }

        public static void clearBitMask(this byte mask, byte value)
        {
            mask &= (byte)~value;
        }

        public static void clearBitMask(this ushort mask, ushort value)
        {
            mask &= (ushort)~value;
        }

        public static void clearBitMask(this uint mask, uint value)
        {
            mask &= ~value;
        }

        public static void clearBitMask(this ulong mask, ulong value)
        {
            mask &= ~value;
        }

        public static bool allOfBitMask(this byte mask, byte value)
        {
            return (mask & value) == value;
        }

        public static bool allOfBitMask(this ushort mask, ushort value)
        {
            return (mask & value) == value;
        }

        public static bool allOfBitMask(this uint mask, uint value)
        {
            return (mask & value) == value;
        }

        public static bool allOfBitMask(this ulong mask, ulong value)
        {
            return (mask & value) == value;
        }

        public static bool anyOfBitMask(this byte mask, byte value)
        {
            return (mask & value) > 0;

        }

        public static bool anyOfBitMask(this ushort mask, ushort value)
        {
            return (mask & value) > 0;
        }

        public static bool anyOfBitMask(this uint mask, uint value)
        {
            return (mask & value) > 0;
        }

        public static bool anyOfBitMask(this ulong mask, ulong value)
        {
            return (mask & value) > 0;
        }


        public static bool noneOfBitMask(this byte mask, byte value)
        {
            return (mask & value) == 0;
        }

        public static bool noneOfBitMask(this ushort mask, ushort value)
        {
            return (mask & value) == 0;
        }

        public static bool noneOfBitMask(this uint mask, uint value)
        {
            return (mask & value) == 0;
        }

        public static bool noneOfBitMask(this ulong mask, ulong value)
        {
            return (mask & value) == 0;
        }

        public static void resetBitMask(this byte mask)
        {
            mask = 0;
        }

        public static void resetBitMask(this ushort mask)
        {
            mask = 0;
        }

        public static void resetBitMask(this uint mask)
        {
            mask = 0;
        }

        public static void resetBitMask(this ulong mask)
        {
            mask = 0;
        }
    }
}