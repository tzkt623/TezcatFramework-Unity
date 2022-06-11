using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    public class TezGlobalState
    {
        static Dictionary<string, uint> m_StateWithName = new Dictionary<string, uint>(32);
        static uint m_Mask = 0;

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
            m_Mask |= state_s;
        }

        public static void remove(uint state_s)
        {
            m_Mask &= ~state_s;
        }

        public static bool anyOf(uint state_s)
        {
            return (m_Mask & state_s) > 0;
        }

        public static bool noneOf(uint state_s)
        {
            return (m_Mask & state_s) == 0;
        }

        public static bool allOf(uint state_s)
        {
            return (m_Mask & state_s) == state_s;
        }

        public static void reset()
        {
            m_Mask = 0;
        }
    }
}