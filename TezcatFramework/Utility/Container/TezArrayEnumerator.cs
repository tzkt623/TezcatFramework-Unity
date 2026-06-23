using System.Collections;
using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    public class TezArrayEnumerator<Array> : IEnumerator<Array>
    {
        public Array Current { get; private set; }
        object IEnumerator.Current => this.Current;

        private int m_Index;
        private Array[] m_Array;

        public TezArrayEnumerator(Array[] array)
        {
            m_Array = array;
        }

        public void Dispose()
        {
            this.Current = default;
            m_Array = null;
        }

        public bool MoveNext()
        {
            if (m_Index < m_Array.Length)
            {
                this.Current = m_Array[m_Index];
                m_Index++;
                return true;
            }
            else
            {
                this.Current = default;
                return false;
            }
        }

        public void Reset()
        {
            m_Index = 0;
            this.Current = default;
        }
    }
}