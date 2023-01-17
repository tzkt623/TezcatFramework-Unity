using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    public class TezGlobalState
    {
        static Dictionary<string, uint> mStateWithName = new Dictionary<string, uint>(32);
        static uint mMask = 0;

        public static uint createOrGet(string name)
        {
            if (!mStateWithName.TryGetValue(name, out var state))
            {
                state = 1u << mStateWithName.Count;
                mStateWithName.Add(name, state);
            }

            return state;
        }

        public static void add(uint states)
        {
            mMask |= states;
        }

        public static void remove(uint states)
        {
            mMask &= ~states;
        }

        public static bool anyOf(uint states)
        {
            return (mMask & states) > 0;
        }

        public static bool noneOf(uint states)
        {
            return (mMask & states) == 0;
        }

        public static bool allOf(uint states)
        {
            return (mMask & states) == states;
        }

        public static void reset()
        {
            mMask = 0;
        }
    }
}