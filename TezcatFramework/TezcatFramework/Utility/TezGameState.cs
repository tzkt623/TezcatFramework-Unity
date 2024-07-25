using System.Collections.Generic;
using tezcat.Framework.Extension;

namespace tezcat.Framework.Utility
{
    public class TezGameState<Descriptor>
    {
        public class Mask
        {
            public readonly string name;
            public readonly ulong maskID;
            public readonly string[] mutexMaskNames = null;
            public readonly ulong[] mutexMasks = null;

            public Mask(string name, ulong maskID, string[] mutexMaskNames)
            {
                this.name = name;
                this.maskID = maskID;
                this.mutexMaskNames = mutexMaskNames;
                if (this.mutexMaskNames != null)
                {
                    this.mutexMasks = new ulong[this.mutexMaskNames.Length];
                }
            }

            /// <summary>
            /// 隐式转换
            /// </summary>
            public static implicit operator ulong(Mask mask) => mask.maskID;
        }

        static Dictionary<string, Mask> mStateWithName = new Dictionary<string, Mask>(64);
        static ulong mMask = 0;
        static Mask[] mMaskArray = new Mask[64];

        public static Mask createOrGet(string name, string[] mutexMaskNames = null)
        {
            if (!mStateWithName.TryGetValue(name, out var mask))
            {
                mask = new Mask(name, 1ul << mStateWithName.Count, mutexMaskNames);
                mMaskArray[mStateWithName.Count] = mask;
                mStateWithName.Add(mask.name, mask);
            }

            return mask;
        }

        public static void init()
        {
            foreach (var pair in mStateWithName)
            {
                var mask = pair.Value;
                if (mask.mutexMaskNames != null)
                {
                    for (int i = 0; i < mask.mutexMaskNames.Length; i++)
                    {
                        mask.mutexMasks[i] = mStateWithName[mask.mutexMaskNames[i]].maskID;
                    }
                }
            }
        }

        public static void printState(TezEventExtension.Action<Mask> onPrintFunc)
        {
            for (int i = 0; i < mStateWithName.Count; i++)
            {
                if ((mMask & mMaskArray[i].maskID) == mMaskArray[i].maskID)
                {
                    onPrintFunc(mMaskArray[i]);
                }
            }
        }

        public static void add(ulong states)
        {
            mMask |= states;
        }

        public static void remove(ulong states)
        {
            mMask &= ~states;
        }

        /// <summary>
        /// 只有当前状态
        /// </summary>
        public static bool onlyOf(ulong states)
        {
            return mMask == states;
        }

        /// <summary>
        /// 有其中一种或多钟状态
        /// </summary>
        public static bool anyOf(ulong states)
        {
            return (mMask & states) > 0;
        }

        /// <summary>
        /// 完全没有此状态
        /// </summary>
        public static bool noneOf(ulong states)
        {
            return (mMask & states) == 0;
        }

        /// <summary>
        /// 包含全部状态
        /// </summary>
        public static bool allOf(ulong states)
        {
            return (mMask & states) == states;
        }

        public static void reset()
        {
            mMask = 0;
        }
    }
}