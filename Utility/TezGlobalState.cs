using System.Collections;
using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    public class TezGlobalState
    {
        static BitArray m_Mask = new BitArray(32);
        static Dictionary<string, int> m_StateWithName = new Dictionary<string, int>();

        static TezGlobalState()
        {
            m_Mask.SetAll(false);
        }

        static public int createState(string name)
        {
            int state;
            if(!m_StateWithName.TryGetValue(name, out state))
            {
                state = m_StateWithName.Count;
                m_StateWithName.Add(name, state);
                if(m_StateWithName.Count > m_Mask.Length)
                {
                    m_Mask.Length += 32;
                }
            }
            return state;
        }

        static public void set(int state)
        {
            m_Mask.Set(state, true);
        }

        static public void clear(int state)
        {
            m_Mask.Set(state, false);
        }

        static public bool anyOf(params int[] state_array)
        {
            foreach (var index in state_array)
            {
                if(m_Mask.Get(index))
                {
                    return true;
                }
            }

            return false;
        }

        static public bool noneOf(params int[] state_array)
        {
            foreach (var index in state_array)
            {
                if (m_Mask.Get(index))
                {
                    return false;
                }
            }

            return true;
        }

        static public bool allOf(params int[] state_array)
        {
            foreach (var index in state_array)
            {
                if (!m_Mask.Get(index))
                {
                    return false;
                }
            }

            return true;
        }

        static public bool has(int state)
        {
            return m_Mask.Get(state);
        }
    }
}