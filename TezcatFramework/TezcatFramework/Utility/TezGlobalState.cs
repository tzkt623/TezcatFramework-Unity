using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    public class TezGlobalState
    {
        static Dictionary<string, uint> m_StateWithName = new Dictionary<string, uint>(32);
        static uint mMask = 0;

        public static uint createOrGet(string name)
        {
            uint state;
            if (!m_StateWithName.TryGetValue(name, out state))
            {
                state = 1u << m_StateWithName.Count;
                m_StateWithName.Add(name, state);
            }

            return state;
        }

        public static void add(uint state_s)
        {
            mMask |= state_s;
        }

        public static void remove(uint state_s)
        {
            mMask &= ~state_s;
        }

        public static bool anyOf(uint state_s)
        {
            return (mMask & state_s) > 0;
        }

        public static bool noneOf(uint state_s)
        {
            return (mMask & state_s) == 0;
        }

        public static bool allOf(uint state_s)
        {
            return (mMask & state_s) == state_s;
        }

        public static void reset()
        {
            mMask = 0;
        }
    }
}