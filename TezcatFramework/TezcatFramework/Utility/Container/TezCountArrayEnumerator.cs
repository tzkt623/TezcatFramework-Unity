using System.Collections;
using System.Collections.Generic;

namespace tezcat.Framework.Utility
{
    public class TezCountArrayEnumerator<Array> : IEnumerator<Array>
    {
        public Array Current { get; private set; }
        object IEnumerator.Current => this.Current;

        private int m_Index;
        private int m_Count;
        private Array[] m_Array;

        public TezCountArrayEnumerator(Array[] array, int count)
        {
            m_Array = array;
            m_Count = count;
        }

        public void Dispose()
        {
            this.Current = default;
            m_Array = null;
        }

        public bool MoveNext()
        {
            if (m_Index < m_Count)
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
            m_Count = 0;
            this.Current = default;
        }
    }
}